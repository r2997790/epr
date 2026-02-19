using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using OfficeOpenXml;

namespace EPR.Web.Scripts;

/// <summary>
/// Script to import M&S UK Network distribution group from Excel file
/// Creates a Distribution Group with Manufacturer -> Hub -> Store hierarchy
/// </summary>
public class ImportMSNetworkDistributionGroup
{
    public static async Task<string> RunAsync(string excelFilePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        if (!File.Exists(excelFilePath))
        {
            throw new FileNotFoundException($"Excel file not found: {excelFilePath}");
        }

        var projectData = new
        {
            projectName = "M&S UK Network",
            savedAt = DateTime.UtcNow.ToString("O"),
            nodes = new List<object>(),
            connections = new List<object>()
        };

        var nodes = new List<Dictionary<string, object>>();
        var connections = new List<Dictionary<string, object>>();
        var nodeIdCounter = 1;
        var connectionIdCounter = 1;

        // Node ID generators
        string GenerateNodeId() => $"node_{nodeIdCounter++}";
        string GenerateConnectionId() => $"conn_{connectionIdCounter++}";

        using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
        {
            var worksheet = package.Workbook.Worksheets[0];
            if (worksheet == null)
            {
                throw new Exception("Worksheet not found");
            }

            var startRow = worksheet.Dimension?.Start.Row ?? 1;
            var endRow = worksheet.Dimension?.End.Row ?? 1;

            // Find header row (usually row 1)
            int headerRow = 1;
            var storeNameCol = -1;
            var hubNameCol = -1;
            var nearestStoreToStoreCol = -1; // Column G
            var nearestStoreToStoreNameCol = -1; // Column H
            var nearestStoreToStoreDistanceCol = -1; // Column I
            var nearestStoreToHubCol = -1; // Column J
            var nearestStoreToHubNameCol = -1; // Column K
            var nearestStoreToHubDistanceCol = -1; // Column L

            // Use standard column positions as specified:
            // Columns G, H, I: Nearest store to store (nearest store name in H, distance in I)
            // Columns J, K, L: Hub to store (store name in K, distance in L)
            // Typically: Column A = Store Name, Column B = Hub Name (or similar)
            
            // Store and Hub name columns (adjust if needed based on actual Excel structure)
            storeNameCol = 1; // Column A - Store Name
            hubNameCol = 2; // Column B - Hub Name (or find it dynamically)
            
            // Columns G, H, I: Store -> Nearest Store connection
            nearestStoreToStoreCol = 7; // Column G - nearest store identifier (may not be used)
            nearestStoreToStoreNameCol = 8; // Column H - nearest store name
            nearestStoreToStoreDistanceCol = 9; // Column I - distance to nearest store
            
            // Columns J, K, L: Hub -> Store connection  
            nearestStoreToHubCol = 10; // Column J - hub/store identifier (may not be used)
            nearestStoreToHubNameCol = 11; // Column K - store name (for hub connection)
            nearestStoreToHubDistanceCol = 12; // Column L - distance from hub to store

            // Create Distribution Group node
            var distributionGroupId = GenerateNodeId();
            var distributionGroupNode = new Dictionary<string, object>
            {
                ["id"] = distributionGroupId,
                ["type"] = "distribution-group",
                ["name"] = "M&S UK Network",
                ["icon"] = "bi-collection",
                ["x"] = 100,
                ["y"] = 100,
                ["containedItems"] = new List<string>(),
                ["locked"] = false
            };
            nodes.Add(distributionGroupNode);

            // Create Manufacturer node
            var manufacturerId = GenerateNodeId();
            var manufacturerNode = new Dictionary<string, object>
            {
                ["id"] = manufacturerId,
                ["type"] = "distribution",
                ["name"] = "M&S Food Supplies Ltd",
                ["icon"] = "bi-building",
                ["x"] = 200,
                ["y"] = 200,
                ["groupId"] = distributionGroupId,
                ["parameters"] = new Dictionary<string, object>
                {
                    ["address"] = "Unit 2 Craven Court, Winwick Quay, Warrington, Cheshire, WA2 8QU",
                    ["locationType"] = "Manufacturer"
                },
                ["locked"] = false
            };
            nodes.Add(manufacturerNode);
            ((List<string>)distributionGroupNode["containedItems"]).Add(manufacturerId);

            // Dictionary to track stores and hubs
            var storeNodes = new Dictionary<string, Dictionary<string, object>>();
            var hubNodes = new Dictionary<string, Dictionary<string, object>>();
            var storeToHubMap = new Dictionary<string, string>(); // Store name -> Hub name
            var hubPositions = new Dictionary<string, (double x, double y)>();
            var storePositions = new Dictionary<string, (double x, double y)>();

            // First pass: collect all unique hubs and stores
            var hubSet = new HashSet<string>();
            var storeSet = new HashSet<string>();
            var storeToHubMapping = new Dictionary<string, string>(); // Store -> Hub mapping

            for (int row = startRow + 1; row <= endRow; row++)
            {
                var storeName = worksheet.Cells[row, storeNameCol].Text?.Trim();
                var hubName = worksheet.Cells[row, hubNameCol].Text?.Trim();
                
                if (!string.IsNullOrEmpty(storeName))
                {
                    storeSet.Add(storeName);
                    if (!string.IsNullOrEmpty(hubName))
                    {
                        storeToHubMapping[storeName] = hubName;
                        hubSet.Add(hubName);
                    }
                }
            }

            // Create Hub nodes - nested under manufacturer
            int hubIndex = 0;
            double hubStartX = 400;
            double hubStartY = 200;
            double hubSpacing = 300;

            foreach (var hubName in hubSet.OrderBy(h => h))
            {
                var hubId = GenerateNodeId();
                var hubX = hubStartX + (hubIndex % 3) * hubSpacing;
                var hubY = hubStartY + (hubIndex / 3) * 400;
                
                var hubNode = new Dictionary<string, object>
                {
                    ["id"] = hubId,
                    ["type"] = "distribution",
                    ["name"] = hubName,
                    ["icon"] = "bi-box-seam",
                    ["x"] = hubX,
                    ["y"] = hubY,
                    ["groupId"] = manufacturerId, // Hubs are nested under manufacturer, not distribution group
                    ["containedItems"] = new List<string>(), // Hubs can contain stores
                    ["parameters"] = new Dictionary<string, object>
                    {
                        ["locationType"] = "Hub"
                    },
                    ["locked"] = false
                };
                
                nodes.Add(hubNode);
                hubNodes[hubName] = hubNode;
                hubPositions[hubName] = (hubX, hubY);
                
                // Add hub to manufacturer's containedItems (if manufacturer has containedItems)
                // Otherwise, manufacturer will find hubs via groupId
                // Note: Manufacturer doesn't need containedItems array, hubs reference it via groupId
                
                // Connect Manufacturer to Hub
                connections.Add(new Dictionary<string, object>
                {
                    ["from"] = manufacturerId,
                    ["to"] = hubId,
                    ["fromPort"] = "right",
                    ["toPort"] = "left"
                });
                
                hubIndex++;
            }

            // Create Store nodes - but don't assign groupId yet, we'll do it based on nearest hub
            int storeIndex = 0;
            double storeStartX = 100;
            double storeStartY = 600;
            double storeSpacingX = 250;
            double storeSpacingY = 100;
            int storesPerRow = 5;
            
            // Track which hub each store belongs to (store name -> hub ID)
            var storeToHubIdMap = new Dictionary<string, string>();

            foreach (var storeName in storeSet.OrderBy(s => s))
            {
                var storeId = GenerateNodeId();
                var storeX = storeStartX + (storeIndex % storesPerRow) * storeSpacingX;
                var storeY = storeStartY + (storeIndex / storesPerRow) * storeSpacingY;
                
                // Determine which hub this store belongs to (from column B or from storeToHubMapping)
                string? assignedHubId = null;
                string? assignedHubName = null;
                if (storeToHubMapping.ContainsKey(storeName))
                {
                    assignedHubName = storeToHubMapping[storeName];
                    if (hubNodes.ContainsKey(assignedHubName))
                    {
                        assignedHubId = hubNodes[assignedHubName]["id"].ToString();
                        storeToHubIdMap[storeName] = assignedHubId;
                    }
                }
                
                // ALL stores must be assigned to a hub - if no hub found, assign to first hub as fallback
                if (assignedHubId == null && hubNodes.Count > 0)
                {
                    // Fallback: assign to first hub (this shouldn't happen if data is correct)
                    var firstHub = hubNodes.Values.First();
                    assignedHubId = firstHub["id"].ToString();
                    assignedHubName = firstHub["name"].ToString();
                    Console.WriteLine($"Warning: Store '{storeName}' has no hub assigned, assigning to '{assignedHubName}'");
                }
                
                var storeNode = new Dictionary<string, object>
                {
                    ["id"] = storeId,
                    ["type"] = "distribution",
                    ["name"] = storeName,
                    ["icon"] = "bi-shop",
                    ["x"] = storeX,
                    ["y"] = storeY,
                    ["groupId"] = assignedHubId ?? manufacturerId, // Nest under hub if available, otherwise under manufacturer (never distribution group)
                    ["parameters"] = new Dictionary<string, object>
                    {
                        ["locationType"] = "Store"
                    },
                    ["locked"] = false
                };
                
                nodes.Add(storeNode);
                storeNodes[storeName] = storeNode;
                storePositions[storeName] = (storeX, storeY);
                
                // Add store to hub's containedItems (ALWAYS assign to hub, never to distribution group)
                if (assignedHubId != null && assignedHubName != null && hubNodes.ContainsKey(assignedHubName))
                {
                    var hubNode = hubNodes[assignedHubName];
                    if (hubNode.ContainsKey("containedItems") && hubNode["containedItems"] is List<string> hubContainedItems)
                    {
                        if (!hubContainedItems.Contains(storeId))
                        {
                            hubContainedItems.Add(storeId);
                        }
                    }
                }
                
                storeIndex++;
            }

            // Track which connections we've already created to avoid duplicates
            var hubToStoreConnections = new HashSet<(string hubId, string storeId)>();
            var storeToStoreConnections = new HashSet<(string storeId1, string storeId2)>();

            // Second pass: process rows to create connections
            for (int row = startRow + 1; row <= endRow; row++)
            {
                var storeName = worksheet.Cells[row, storeNameCol].Text?.Trim();
                
                if (string.IsNullOrEmpty(storeName))
                    continue;

                // Get hub name from column B (the hub this store belongs to)
                var hubName = worksheet.Cells[row, hubNameCol].Text?.Trim();
                
                // Connect Hub to Store using columns J, K, L
                // Column K contains the store name that should be connected to the hub
                // Column L contains the distance from hub to that store
                var hubStoreName = worksheet.Cells[row, nearestStoreToHubNameCol].Text?.Trim();
                var hubStoreDistanceText = worksheet.Cells[row, nearestStoreToHubDistanceCol].Text?.Trim();
                
                if (!string.IsNullOrEmpty(hubName) && !string.IsNullOrEmpty(hubStoreName) && 
                    hubNodes.ContainsKey(hubName) && storeNodes.ContainsKey(hubStoreName))
                {
                    var hubId = hubNodes[hubName]["id"].ToString();
                    var hubStoreId = storeNodes[hubStoreName]["id"].ToString();
                    
                    if (hubId == null || hubStoreId == null)
                        continue;
                    
                    // Ensure store is nested under this hub (update groupId and containedItems)
                    // Note: Store should already be assigned to a hub from first pass, but ensure it's correct
                    var hubNode = hubNodes[hubName];
                    var hubStoreNodeDict = storeNodes[hubStoreName];
                    
                    // Update store's groupId to point to this hub (if not already correct)
                    if (hubStoreNodeDict["groupId"].ToString() != hubId)
                    {
                        hubStoreNodeDict["groupId"] = hubId;
                        
                        // Remove store from any other container (distribution group or wrong hub)
                        var distGroupContainedItems = (List<string>)distributionGroupNode["containedItems"];
                        distGroupContainedItems.Remove(hubStoreId);
                        
                        // Remove from any other hub's containedItems
                        foreach (var otherHub in hubNodes.Values)
                        {
                            if (otherHub["id"].ToString() != hubId && otherHub.ContainsKey("containedItems") && otherHub["containedItems"] is List<string> otherHubContainedItems)
                            {
                                otherHubContainedItems.Remove(hubStoreId);
                            }
                        }
                    }
                    
                    // Add store to this hub's containedItems
                    if (hubNode.ContainsKey("containedItems") && hubNode["containedItems"] is List<string> hubContainedItems)
                    {
                        if (!hubContainedItems.Contains(hubStoreId))
                        {
                            hubContainedItems.Add(hubStoreId);
                        }
                    }
                    
                    // Skip if already connected
                    if (hubToStoreConnections.Contains((hubId, hubStoreId)))
                    {
                        // Already connected, skip
                    }
                    else
                    {
                        // Parse distance from column L
                        double distance = 0;
                        if (!string.IsNullOrEmpty(hubStoreDistanceText))
                        {
                            var cleanText = hubStoreDistanceText.Replace("km", "").Replace("miles", "").Replace("mi", "").Trim();
                            double.TryParse(cleanText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out distance);
                        }

                        // Create transport node for Hub -> Store connection
                        if (distance > 0)
                        {
                            var transportNodeId = GenerateNodeId();
                            var hubPos = hubPositions[hubName];
                            var hubStorePos = storePositions[hubStoreName];
                            var transportX = (hubPos.x + hubStorePos.x) / 2;
                            var transportY = (hubPos.y + hubStorePos.y) / 2;

                            var transportNode = new Dictionary<string, object>
                            {
                                ["id"] = transportNodeId,
                                ["type"] = "transport",
                                ["name"] = $"Transport ({distance} km)",
                                ["icon"] = "bi-truck",
                                ["x"] = transportX,
                                ["y"] = transportY,
                                ["groupId"] = distributionGroupId,
                                ["transportTypes"] = new List<Dictionary<string, object>>
                                {
                                    new Dictionary<string, object>
                                    {
                                        ["type"] = "Truck",
                                        ["distance"] = distance
                                    }
                                },
                                ["locked"] = false
                            };
                            
                            nodes.Add(transportNode);
                            ((List<string>)distributionGroupNode["containedItems"]).Add(transportNodeId);

                            // Connect Hub -> Transport -> Store
                            connections.Add(new Dictionary<string, object>
                            {
                                ["from"] = hubId,
                                ["to"] = transportNodeId,
                                ["fromPort"] = "right",
                                ["toPort"] = "left"
                            });
                            
                            connections.Add(new Dictionary<string, object>
                            {
                                ["from"] = transportNodeId,
                                ["to"] = hubStoreId,
                                ["fromPort"] = "right",
                                ["toPort"] = "left"
                            });
                        }
                        else
                        {
                            // Direct connection without transport node
                            connections.Add(new Dictionary<string, object>
                            {
                                ["from"] = hubId,
                                ["to"] = hubStoreId,
                                ["fromPort"] = "right",
                                ["toPort"] = "left"
                            });
                        }
                        
                        hubToStoreConnections.Add((hubId, hubStoreId));
                    }
                }
                
                // Now handle store-to-store connections for the current store (column A)
                if (!storeNodes.ContainsKey(storeName))
                    continue;

                var storeNode = storeNodes[storeName];
                var storeId = storeNode["id"].ToString();

                // Connect Store to nearest Store (from columns G, H, I)
                // Column H contains the name of the nearest store to the store in column A
                // Column I contains the distance from store (column A) to nearest store (column H)
                var nearestStoreName = worksheet.Cells[row, nearestStoreToStoreNameCol].Text?.Trim();
                var nearestStoreDistanceText = worksheet.Cells[row, nearestStoreToStoreDistanceCol].Text?.Trim();
                
                if (!string.IsNullOrEmpty(nearestStoreName) && storeNodes.ContainsKey(nearestStoreName) && nearestStoreName != storeName)
                {
                    var nearestStoreId = storeNodes[nearestStoreName]["id"].ToString();
                    
                    // Skip if already connected (avoid duplicate bidirectional connections)
                    var connectionKey1 = (storeId, nearestStoreId);
                    var connectionKey2 = (nearestStoreId, storeId);
                    if (storeToStoreConnections.Contains(connectionKey1) || storeToStoreConnections.Contains(connectionKey2))
                        continue;
                    
                    double nearestStoreDistance = 0;
                    if (!string.IsNullOrEmpty(nearestStoreDistanceText))
                    {
                        // Try to parse number, removing units
                        var cleanText = nearestStoreDistanceText.Replace("km", "").Replace("miles", "").Replace("mi", "").Trim();
                        double.TryParse(cleanText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out nearestStoreDistance);
                    }

                    // Create transport node for Store -> Store connection
                    if (nearestStoreDistance > 0)
                    {
                        var transportNodeId = GenerateNodeId();
                        var storePos = storePositions[storeName];
                        var nearestStorePos = storePositions[nearestStoreName];
                        var transportX = (storePos.x + nearestStorePos.x) / 2;
                        var transportY = (storePos.y + nearestStorePos.y) / 2;

                        var transportNode = new Dictionary<string, object>
                        {
                            ["id"] = transportNodeId,
                            ["type"] = "transport",
                            ["name"] = $"Transport ({nearestStoreDistance} km)",
                            ["icon"] = "bi-truck",
                            ["x"] = transportX,
                            ["y"] = transportY,
                            ["groupId"] = distributionGroupId,
                            ["transportTypes"] = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    ["type"] = "Truck",
                                    ["distance"] = nearestStoreDistance
                                }
                            },
                            ["locked"] = false
                        };
                        
                        nodes.Add(transportNode);
                        ((List<string>)distributionGroupNode["containedItems"]).Add(transportNodeId);

                        // Connect Store -> Transport -> Nearest Store
                        connections.Add(new Dictionary<string, object>
                        {
                            ["from"] = storeId,
                            ["to"] = transportNodeId,
                            ["fromPort"] = "right",
                            ["toPort"] = "left"
                        });
                        
                        connections.Add(new Dictionary<string, object>
                        {
                            ["from"] = transportNodeId,
                            ["to"] = nearestStoreId,
                            ["fromPort"] = "right",
                            ["toPort"] = "left"
                        });
                    }
                    else
                    {
                        // Direct connection without transport node
                        connections.Add(new Dictionary<string, object>
                        {
                            ["from"] = storeId,
                            ["to"] = nearestStoreId,
                            ["fromPort"] = "right",
                            ["toPort"] = "left"
                        });
                    }
                    
                    storeToStoreConnections.Add(connectionKey1);
                }
            }
        }

        // Create final project data structure
        var finalProjectData = new Dictionary<string, object>
        {
            ["projectName"] = "M&S UK Network",
            ["savedAt"] = DateTime.UtcNow.ToString("O"),
            ["nodes"] = nodes,
            ["connections"] = connections
        };

        // Convert to JSON
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        var json = JsonSerializer.Serialize(finalProjectData, options);
        
        // Save to file
        var outputPath = Path.Combine(Path.GetDirectoryName(excelFilePath) ?? "", "MSNetwork_DistributionGroup.json");
        await File.WriteAllTextAsync(outputPath, json);
        
        Console.WriteLine($"✅ Distribution Group created successfully!");
        Console.WriteLine($"   Total Nodes: {nodes.Count}");
        Console.WriteLine($"   Total Connections: {connections.Count}");
        Console.WriteLine($"   JSON saved to: {outputPath}");
        Console.WriteLine($"\nTo load this project:");
        Console.WriteLine($"1. Open Visual Editor");
        Console.WriteLine($"2. Open browser console (F12)");
        Console.WriteLine($"3. Run: localStorage.setItem('eprSavedProjects', '[{json}]');");
        Console.WriteLine($"4. Click 'Load Project' button");
        
        return json;
    }
}

