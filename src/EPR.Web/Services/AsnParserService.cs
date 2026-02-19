using System.Xml.Linq;
using EPR.Domain.Entities;

namespace EPR.Web.Services;

/// <summary>
/// Service for parsing ASN data from various formats (GS1 XML, EDI 856, DESADV)
/// </summary>
public class AsnParserService
{
    /// <summary>
    /// Parse GS1 XML ASN format
    /// </summary>
    public AsnShipment ParseGS1Xml(string xmlContent)
    {
        try
        {
            var doc = XDocument.Parse(xmlContent);
            
            // Try to find the namespace - check if root has namespace
            XNamespace ns = XNamespace.None;
            
            if (doc.Root != null)
            {
                // Get the default namespace or the 'sh' prefix namespace
                ns = doc.Root.GetDefaultNamespace();
                
                // If no default namespace, try to get 'sh' prefix namespace
                if (ns == XNamespace.None)
                {
                    var shNs = doc.Root.GetNamespaceOfPrefix("sh");
                    if (shNs != null)
                    {
                        ns = shNs;
                    }
                }
            }
            
            // Try to find despatchAdvice with or without namespace
            var despatchAdvice = doc.Descendants(ns + "despatchAdvice").FirstOrDefault();
            
            // If not found with namespace, try without namespace
            if (despatchAdvice == null && ns != XNamespace.None)
            {
                despatchAdvice = doc.Descendants("despatchAdvice").FirstOrDefault();
                if (despatchAdvice != null)
                {
                    ns = XNamespace.None;
                }
            }
            
            // If still not found, try looking for any element with local name "despatchAdvice"
            if (despatchAdvice == null)
            {
                despatchAdvice = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "despatchAdvice");
                if (despatchAdvice != null)
                {
                    ns = despatchAdvice.Name.Namespace;
                }
            }
            
            if (despatchAdvice == null)
            {
                throw new InvalidOperationException("Invalid GS1 XML: despatchAdvice element not found");
            }

            var shipment = new AsnShipment
            {
                SourceFormat = "GS1_XML",
                RawData = xmlContent,
                ImportedAt = DateTime.UtcNow,
                Status = "PENDING"
            };

            // Parse header information
            var asnId = despatchAdvice.Element(ns + "despatchAdviceIdentification");
            shipment.AsnNumber = asnId?.Element(ns + "entityIdentification")?.Value ?? string.Empty;

            // Parse shipper
            var shipper = despatchAdvice.Element(ns + "shipper");
            if (shipper != null)
            {
                shipment.ShipperGln = shipper.Element(ns + "gln")?.Value ?? string.Empty;
                shipment.ShipperName = shipper.Element(ns + "organizationName")?.Value ?? string.Empty;
                
                var shipperAddress = shipper.Element(ns + "address");
                if (shipperAddress != null)
                {
                    shipment.ShipperAddress = shipperAddress.Element(ns + "streetAddressOne")?.Value;
                    shipment.ShipperCity = shipperAddress.Element(ns + "city")?.Value;
                    shipment.ShipperPostalCode = shipperAddress.Element(ns + "postalCode")?.Value;
                    shipment.ShipperCountryCode = shipperAddress.Element(ns + "countryCode")?.Value;
                }
            }

            // Parse receiver
            var receiver = despatchAdvice.Element(ns + "receiver");
            if (receiver != null)
            {
                shipment.ReceiverGln = receiver.Element(ns + "gln")?.Value ?? string.Empty;
                shipment.ReceiverName = receiver.Element(ns + "organizationName")?.Value ?? string.Empty;
            }

            // Parse dates
            var despatchInfo = despatchAdvice.Element(ns + "despatchInformation");
            if (despatchInfo != null)
            {
                var despatchDate = despatchInfo.Element(ns + "despatchDateTime")?.Value;
                if (!string.IsNullOrEmpty(despatchDate) && DateTime.TryParse(despatchDate, out var shipDate))
                {
                    shipment.ShipDate = shipDate;
                }
                else
                {
                    shipment.ShipDate = DateTime.UtcNow;
                }

                var deliveryDate = despatchInfo.Element(ns + "estimatedDeliveryDateTime")?.Value;
                if (!string.IsNullOrEmpty(deliveryDate) && DateTime.TryParse(deliveryDate, out var estDelivery))
                {
                    shipment.DeliveryDate = estDelivery;
                }
            }

            // Parse transport information
            var transportInfo = despatchAdvice.Element(ns + "transportInformation");
            if (transportInfo != null)
            {
                shipment.TransportMode = transportInfo.Element(ns + "transportMode")?.Value;
                shipment.CarrierName = transportInfo.Element(ns + "carrierName")?.Value;
            }

            // Parse despatch units (pallets)
            var despatchUnits = despatchAdvice.Elements(ns + "despatchUnit").ToList();
            shipment.TotalPackages = despatchUnits.Count;
            decimal totalWeight = 0;

            int palletSequence = 1;
            foreach (var unit in despatchUnits)
            {
                var pallet = new AsnPallet
                {
                    SequenceNumber = palletSequence++
                };

                // Parse SSCC
                var logisticUnit = unit.Element(ns + "logisticUnitIdentification");
                pallet.Sscc = logisticUnit?.Element(ns + "sscc")?.Value ?? string.Empty;

                // Parse package type and weight
                pallet.PackageTypeCode = unit.Element(ns + "packageTypeCode")?.Value;
                
                var weightStr = unit.Element(ns + "grossWeight")?.Value;
                if (!string.IsNullOrEmpty(weightStr) && decimal.TryParse(weightStr, out var weight))
                {
                    pallet.GrossWeight = weight;
                    totalWeight += weight;
                }

                // Parse destination
                var finalShipTo = unit.Element(ns + "finalShipTo");
                if (finalShipTo != null)
                {
                    pallet.DestinationGln = finalShipTo.Element(ns + "gln")?.Value ?? string.Empty;
                    pallet.DestinationName = finalShipTo.Element(ns + "organizationName")?.Value ?? string.Empty;
                    
                    var destAddress = finalShipTo.Element(ns + "address");
                    if (destAddress != null)
                    {
                        pallet.DestinationAddress = destAddress.Element(ns + "streetAddressOne")?.Value;
                        pallet.DestinationCity = destAddress.Element(ns + "city")?.Value;
                        pallet.DestinationPostalCode = destAddress.Element(ns + "postalCode")?.Value;
                        pallet.DestinationCountryCode = destAddress.Element(ns + "countryCode")?.Value;
                    }
                }

                // Parse line items
                var itemDetails = unit.Elements(ns + "despatchUnitItemDetail").ToList();
                foreach (var itemDetail in itemDetails)
                {
                    var lineItem = new AsnLineItem();

                    var lineNumStr = itemDetail.Element(ns + "lineItemNumber")?.Value;
                    if (!string.IsNullOrEmpty(lineNumStr) && int.TryParse(lineNumStr, out var lineNum))
                    {
                        lineItem.LineNumber = lineNum;
                    }

                    // Parse GTIN
                    var tradeItem = itemDetail.Element(ns + "tradeItemIdentification");
                    lineItem.Gtin = tradeItem?.Element(ns + "gtin")?.Value ?? string.Empty;

                    // Parse description
                    var descInfo = itemDetail.Element(ns + "tradeItemDescriptionInformation");
                    lineItem.Description = descInfo?.Element(ns + "descriptionShort")?.Value ?? string.Empty;

                    // Parse quantity
                    var qtyStr = itemDetail.Element(ns + "quantityDespatched")?.Value;
                    if (!string.IsNullOrEmpty(qtyStr) && decimal.TryParse(qtyStr, out var qty))
                    {
                        lineItem.Quantity = qty;
                    }

                    var qtyUnit = itemDetail.Element(ns + "quantityDespatched")?.Attribute("unitCode")?.Value;
                    lineItem.UnitOfMeasure = qtyUnit ?? "PCE";

                    // Parse transactional data (batch, expiry)
                    var transData = itemDetail.Element(ns + "transactionalItemData");
                    if (transData != null)
                    {
                        lineItem.BatchNumber = transData.Element(ns + "batchNumber")?.Value;
                        
                        var expiryStr = transData.Element(ns + "itemExpirationDate")?.Value;
                        if (!string.IsNullOrEmpty(expiryStr) && DateTime.TryParse(expiryStr, out var expiry))
                        {
                            lineItem.BestBeforeDate = expiry;
                        }
                    }

                    // Parse order information
                    var orderInfo = itemDetail.Element(ns + "orderInformation");
                    if (orderInfo != null)
                    {
                        lineItem.PoLineReference = orderInfo.Element(ns + "orderLineNumber")?.Value;
                        if (string.IsNullOrEmpty(shipment.PoReference))
                        {
                            shipment.PoReference = orderInfo.Element(ns + "referenceNumber")?.Value;
                        }
                    }

                    pallet.LineItems.Add(lineItem);
                }

                shipment.Pallets.Add(pallet);
            }

            shipment.TotalWeight = totalWeight;

            return shipment;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse GS1 XML: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Parse EDI 856 (ANSI X12) ASN format
    /// </summary>
    public AsnShipment ParseEdi856(string content)
    {
        try
        {
            var shipment = new AsnShipment
            {
                SourceFormat = "EDI_856",
                RawData = content,
                ImportedAt = DateTime.UtcNow,
                Status = "PENDING"
            };

            // Split into segments (separated by ~)
            var segments = content.Split('~').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            
            // Parse hierarchical structure
            var hierarchicalItems = new Dictionary<string, HierarchicalItem>();
            var currentPallet = (AsnPallet?)null;
            string? shipperName = null;
            string? receiverName = null;
            
            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i].Trim();
                var elements = segment.Split('*');
                var segmentId = elements[0];
                
                switch (segmentId)
                {
                    case "BSN": // Beginning Segment for Ship Notice
                        if (elements.Length > 2)
                        {
                            shipment.AsnNumber = elements[2];
                        }
                        if (elements.Length > 3 && DateTime.TryParseExact(elements[3], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var bsnDate))
                        {
                            shipment.ShipDate = bsnDate;
                        }
                        break;
                        
                    case "TD1": // Carrier Details (Quantity and Weight)
                        if (elements.Length > 2)
                        {
                            if (int.TryParse(elements[2], out var packageCount))
                            {
                                shipment.TotalPackages = packageCount;
                            }
                        }
                        if (elements.Length > 7 && decimal.TryParse(elements[7], out var weight))
                        {
                            // Convert weight based on unit (elements[8] might be unit)
                            var unit = elements.Length > 8 ? elements[8] : "LB";
                            if (unit == "LB")
                            {
                                shipment.TotalWeight = weight * 0.453592m; // Convert lbs to kg
                            }
                            else
                            {
                                shipment.TotalWeight = weight;
                            }
                        }
                        break;
                        
                    case "TD5": // Carrier Details (Routing Sequence)
                        if (elements.Length > 4)
                        {
                            shipment.CarrierName = elements[4];
                        }
                        if (elements.Length > 5)
                        {
                            shipment.TransportMode = elements[5] == "M" ? "ROAD" : elements[5];
                        }
                        break;
                        
                    case "REF": // Reference Identification
                        if (elements.Length > 2)
                        {
                            if (elements[1] == "BM") // Bill of Lading
                            {
                                // Could be used for additional reference
                            }
                            else if (elements[1] == "PO") // Purchase Order
                            {
                                shipment.PoReference = elements[2];
                            }
                        }
                        break;
                        
                    case "DTM": // Date/Time Reference
                        if (elements.Length > 2)
                        {
                            var dateQualifier = elements[1];
                            if (DateTime.TryParseExact(elements[2], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dtmDate))
                            {
                                if (dateQualifier == "011") // Ship Date
                                {
                                    shipment.ShipDate = dtmDate;
                                }
                                else if (dateQualifier == "067") // Estimated Delivery
                                {
                                    shipment.DeliveryDate = dtmDate;
                                }
                            }
                        }
                        break;
                        
                    case "N1": // Name
                        if (elements.Length > 1)
                        {
                            var entityIdentifier = elements[1];
                            var name = elements.Length > 2 ? elements[2] : "";
                            var idCode = elements.Length > 4 ? elements[4] : "";
                            
                            if (entityIdentifier == "SF") // Ship From
                            {
                                shipperName = name;
                                shipment.ShipperGln = idCode.Length == 13 ? idCode : idCode.PadLeft(13, '0');
                                shipment.ShipperName = name;
                            }
                            else if (entityIdentifier == "ST") // Ship To
                            {
                                receiverName = name;
                                if (currentPallet != null)
                                {
                                    currentPallet.DestinationName = name;
                                    currentPallet.DestinationGln = idCode.Length == 13 ? idCode : idCode.PadLeft(13, '0');
                                }
                                else if (string.IsNullOrEmpty(shipment.ReceiverGln))
                                {
                                    shipment.ReceiverGln = idCode.Length == 13 ? idCode : idCode.PadLeft(13, '0');
                                    shipment.ReceiverName = name;
                                }
                            }
                        }
                        break;
                        
                    case "N3": // Address Information
                        if (elements.Length > 1)
                        {
                            var address = elements[1];
                            if (shipperName != null && string.IsNullOrEmpty(shipment.ShipperAddress))
                            {
                                shipment.ShipperAddress = address;
                                shipperName = null; // Reset flag
                            }
                            else if (currentPallet != null)
                            {
                                currentPallet.DestinationAddress = address;
                            }
                        }
                        break;
                        
                    case "N4": // Geographic Location
                        if (elements.Length > 1)
                        {
                            var city = elements[1];
                            var state = elements.Length > 2 ? elements[2] : "";
                            var postalCode = elements.Length > 3 ? elements[3] : "";
                            var country = elements.Length > 4 ? elements[4] : "US";
                            
                            if (receiverName != null && string.IsNullOrEmpty(shipment.ShipperCity))
                            {
                                shipment.ShipperCity = city;
                                shipment.ShipperPostalCode = postalCode;
                                shipment.ShipperCountryCode = country;
                            }
                            else if (currentPallet != null)
                            {
                                currentPallet.DestinationCity = city;
                                currentPallet.DestinationPostalCode = postalCode;
                                currentPallet.DestinationCountryCode = country;
                            }
                        }
                        break;
                        
                    case "HL": // Hierarchical Level
                        if (elements.Length > 3)
                        {
                            var hlId = elements[1];
                            var parentId = elements[2];
                            var hlCode = elements[3];
                            
                            hierarchicalItems[hlId] = new HierarchicalItem
                            {
                                Id = hlId,
                                ParentId = parentId,
                                LevelCode = hlCode
                            };
                            
                            // P = Pack/Pallet level
                            if (hlCode == "P")
                            {
                                currentPallet = new AsnPallet
                                {
                                    SequenceNumber = shipment.Pallets.Count + 1
                                };
                                shipment.Pallets.Add(currentPallet);
                                hierarchicalItems[hlId].Pallet = currentPallet;
                            }
                            // I = Item level
                            else if (hlCode == "I")
                            {
                                var lineItem = new AsnLineItem
                                {
                                    LineNumber = GetItemSequence(hierarchicalItems, hlId)
                                };
                                hierarchicalItems[hlId].LineItem = lineItem;
                                
                                // Find parent pallet
                                var parentPallet = FindParentPallet(hierarchicalItems, parentId);
                                if (parentPallet != null)
                                {
                                    parentPallet.LineItems.Add(lineItem);
                                }
                            }
                        }
                        break;
                        
                    case "MAN": // Marks and Numbers (SSCC)
                        if (elements.Length > 2 && currentPallet != null)
                        {
                            if (elements[1] == "GM") // SSCC-18
                            {
                                currentPallet.Sscc = elements[2];
                            }
                        }
                        break;
                        
                    case "MEA": // Measurements
                        if (elements.Length > 4 && currentPallet != null)
                        {
                            if (elements[1] == "PD" && elements[2] == "G") // Pack Weight
                            {
                                if (decimal.TryParse(elements[3], out var palletWeight))
                                {
                                    var unit = elements.Length > 4 ? elements[4] : "LB";
                                    if (unit == "LB")
                                    {
                                        currentPallet.GrossWeight = palletWeight * 0.453592m;
                                    }
                                    else
                                    {
                                        currentPallet.GrossWeight = palletWeight;
                                    }
                                }
                            }
                        }
                        break;
                        
                    case "LIN": // Item Identification
                        var currentItem = GetCurrentLineItem(hierarchicalItems, segments, i);
                        if (currentItem != null && elements.Length > 3)
                        {
                            if (elements[2] == "SK") // Stock Keeping Unit (GTIN)
                            {
                                currentItem.Gtin = elements[3];
                            }
                        }
                        break;
                        
                    case "SN1": // Item Detail (Shipment)
                        currentItem = GetCurrentLineItem(hierarchicalItems, segments, i);
                        if (currentItem != null && elements.Length > 2)
                        {
                            if (decimal.TryParse(elements[2], out var qty))
                            {
                                currentItem.Quantity = qty;
                            }
                            if (elements.Length > 3)
                            {
                                currentItem.UnitOfMeasure = elements[3];
                            }
                        }
                        break;
                        
                    case "PID": // Product/Item Description
                        currentItem = GetCurrentLineItem(hierarchicalItems, segments, i);
                        if (currentItem != null && elements.Length > 5)
                        {
                            currentItem.Description = elements[5];
                        }
                        break;
                        
                    case "PRF": // Purchase Order Reference
                        if (elements.Length > 1)
                        {
                            shipment.PoReference = elements[1];
                        }
                        break;
                }
            }

            return shipment;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse EDI 856: {ex.Message}", ex);
        }
    }
    
    // Helper class for hierarchical parsing
    private class HierarchicalItem
    {
        public string Id { get; set; } = "";
        public string ParentId { get; set; } = "";
        public string LevelCode { get; set; } = "";
        public AsnPallet? Pallet { get; set; }
        public AsnLineItem? LineItem { get; set; }
    }
    
    private AsnPallet? FindParentPallet(Dictionary<string, HierarchicalItem> items, string startId)
    {
        if (string.IsNullOrEmpty(startId) || !items.ContainsKey(startId))
            return null;
            
        var item = items[startId];
        if (item.Pallet != null)
            return item.Pallet;
            
        return FindParentPallet(items, item.ParentId);
    }
    
    private int GetItemSequence(Dictionary<string, HierarchicalItem> items, string itemId)
    {
        return items.Values.Count(v => v.LevelCode == "I" && string.Compare(v.Id, itemId, StringComparison.Ordinal) <= 0);
    }
    
    private AsnLineItem? GetCurrentLineItem(Dictionary<string, HierarchicalItem> items, List<string> segments, int currentIndex)
    {
        // Look backwards for the most recent HL segment with level code I
        for (int i = currentIndex; i >= 0; i--)
        {
            var seg = segments[i].Trim();
            if (seg.StartsWith("HL*"))
            {
                var elements = seg.Split('*');
                if (elements.Length > 3 && elements[3] == "I")
                {
                    var hlId = elements[1];
                    if (items.ContainsKey(hlId))
                    {
                        return items[hlId].LineItem;
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Parse DESADV (EDIFACT) ASN format
    /// </summary>
    public AsnShipment ParseDesadv(string content)
    {
        try
        {
            var shipment = new AsnShipment
            {
                SourceFormat = "DESADV",
                RawData = content,
                ImportedAt = DateTime.UtcNow,
                Status = "PENDING"
            };

            // Split into segments (separated by ')
            var segments = content.Split('\'').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            
            var currentPallet = (AsnPallet?)null;
            var currentLineItem = (AsnLineItem?)null;
            
            for (int i = 0; i < segments.Count; i++)
            {
                var segment = segments[i].Trim();
                var elements = segment.Split('+');
                var segmentId = elements[0];
                
                switch (segmentId)
                {
                    case "UNB": // Interchange Header
                        if (elements.Length > 2)
                        {
                            var senderParts = elements[2].Split(':');
                            if (senderParts.Length > 0)
                            {
                                shipment.ShipperGln = senderParts[0].Length == 13 ? senderParts[0] : senderParts[0].PadLeft(13, '0');
                            }
                        }
                        break;
                        
                    case "BGM": // Beginning of Message
                        if (elements.Length > 2)
                        {
                            shipment.AsnNumber = elements[2];
                        }
                        break;
                        
                    case "DTM": // Date/Time/Period
                        if (elements.Length > 1)
                        {
                            var dtmParts = elements[1].Split(':');
                            if (dtmParts.Length > 1)
                            {
                                var qualifier = dtmParts[0];
                                var dateStr = dtmParts[1];
                                
                                if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dtmDate))
                                {
                                    if (qualifier == "137") // Document Date
                                    {
                                        shipment.ShipDate = dtmDate;
                                    }
                                    else if (qualifier == "11") // Despatch Date
                                    {
                                        shipment.ShipDate = dtmDate;
                                    }
                                    else if (qualifier == "361") // Best Before Date (for line items)
                                    {
                                        if (currentLineItem != null)
                                        {
                                            currentLineItem.BestBeforeDate = dtmDate;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                        
                    case "MEA": // Measurements
                        if (elements.Length > 3)
                        {
                            var meaParts = elements[3].Split(':');
                            if (meaParts.Length > 1)
                            {
                                if (decimal.TryParse(meaParts[1], out var measValue))
                                {
                                    var unit = meaParts[0];
                                    if (unit == "KGM") // Kilograms
                                    {
                                        if (currentPallet != null)
                                        {
                                            currentPallet.GrossWeight = measValue;
                                        }
                                        else if (shipment.TotalWeight == null)
                                        {
                                            shipment.TotalWeight = measValue;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                        
                    case "NAD": // Name and Address
                        if (elements.Length > 1)
                        {
                            var qualifier = elements[1];
                            var idParts = elements.Length > 2 ? elements[2].Split(':') : new string[0];
                            var name = elements.Length > 4 ? elements[4] : "";
                            var address = elements.Length > 5 ? elements[5] : "";
                            var city = elements.Length > 6 ? elements[6] : "";
                            var postalCode = elements.Length > 8 ? elements[8] : "";
                            var country = elements.Length > 9 ? elements[9] : "";
                            
                            var gln = idParts.Length > 0 ? idParts[0] : "";
                            gln = gln.Length == 13 ? gln : gln.PadLeft(13, '0');
                            
                            if (qualifier == "SU") // Supplier
                            {
                                shipment.ShipperGln = gln;
                                shipment.ShipperName = name;
                                shipment.ShipperAddress = address;
                                shipment.ShipperCity = city;
                                shipment.ShipperPostalCode = postalCode;
                                shipment.ShipperCountryCode = country;
                            }
                            else if (qualifier == "BY") // Buyer
                            {
                                shipment.ReceiverGln = gln;
                                shipment.ReceiverName = name;
                            }
                            else if (qualifier == "DP") // Delivery Party (destination for pallet)
                            {
                                if (currentPallet != null)
                                {
                                    currentPallet.DestinationGln = gln;
                                    currentPallet.DestinationName = name;
                                    currentPallet.DestinationAddress = address;
                                    currentPallet.DestinationCity = city;
                                    currentPallet.DestinationPostalCode = postalCode;
                                    currentPallet.DestinationCountryCode = country;
                                }
                            }
                        }
                        break;
                        
                    case "TOD": // Terms of Delivery
                        if (elements.Length > 3)
                        {
                            var modeCode = elements[3];
                            shipment.TransportMode = modeCode == "CAR" ? "ROAD" : modeCode;
                        }
                        break;
                        
                    case "CPS": // Consignment Packing Sequence
                        if (elements.Length > 1)
                        {
                            // CPS indicates a packing level
                            // CPS+1 = top level, CPS+2+1 = nested in CPS 1, etc.
                            var cpsId = elements[1];
                            var parentCps = elements.Length > 2 ? elements[2] : "";
                            
                            // If this is a nested CPS (has parent), it's likely a pallet
                            if (!string.IsNullOrEmpty(parentCps))
                            {
                                // This CPS represents a pallet
                                currentPallet = null; // Will be created by PAC
                                currentLineItem = null;
                            }
                        }
                        break;
                        
                    case "PAC": // Package
                        // PAC indicates physical packaging unit (pallet)
                        if (elements.Length > 1)
                        {
                            var packageCount = elements[1];
                            var packagingType = elements.Length > 3 ? elements[3] : "";
                            
                            // Create new pallet
                            currentPallet = new AsnPallet
                            {
                                SequenceNumber = shipment.Pallets.Count + 1,
                                PackageTypeCode = packagingType == "201" ? "PLT" : packagingType
                            };
                            shipment.Pallets.Add(currentPallet);
                            currentLineItem = null;
                        }
                        break;
                        
                    case "PCI": // Package Identification
                        // Usually followed by GIN for SSCC
                        break;
                        
                    case "GIN": // Goods Identity Number (SSCC)
                        if (elements.Length > 2 && currentPallet != null)
                        {
                            if (elements[1] == "BJ") // Serial Shipping Container Code
                            {
                                currentPallet.Sscc = elements[2];
                            }
                        }
                        break;
                        
                    case "LIN": // Line Item
                        if (elements.Length > 3)
                        {
                            var lineNumber = elements[1];
                            var gtinParts = elements[3].Split(':');
                            var gtin = gtinParts[0];
                            
                            currentLineItem = new AsnLineItem
                            {
                                LineNumber = int.TryParse(lineNumber, out var ln) ? ln : currentPallet?.LineItems.Count + 1 ?? 1,
                                Gtin = gtin
                            };
                            
                            if (currentPallet != null)
                            {
                                currentPallet.LineItems.Add(currentLineItem);
                            }
                        }
                        break;
                        
                    case "QTY": // Quantity
                        if (elements.Length > 1 && currentLineItem != null)
                        {
                            var qtyParts = elements[1].Split(':');
                            if (qtyParts.Length > 1)
                            {
                                var qtyType = qtyParts[0];
                                if (qtyType == "12") // Despatch Quantity
                                {
                                    if (decimal.TryParse(qtyParts[1], out var qty))
                                    {
                                        currentLineItem.Quantity = qty;
                                    }
                                }
                            }
                        }
                        break;
                        
                    case "PIA": // Additional Product ID
                        if (elements.Length > 2 && currentLineItem != null)
                        {
                            var piaParts = elements[2].Split(':');
                            if (piaParts.Length > 0)
                            {
                                currentLineItem.SupplierArticleNumber = piaParts[0];
                            }
                        }
                        break;
                        
                    case "IMD": // Item Description
                        if (elements.Length > 3 && currentLineItem != null)
                        {
                            // IMD+F++:::Description
                            var descParts = elements[3].Split(':');
                            var description = descParts.Length > 3 ? descParts[3] : (descParts.Length > 0 ? descParts[descParts.Length - 1] : "");
                            currentLineItem.Description = description;
                        }
                        break;
                        
                    case "RFF": // Reference
                        if (elements.Length > 1)
                        {
                            var refParts = elements[1].Split(':');
                            if (refParts.Length > 1)
                            {
                                var refQualifier = refParts[0];
                                var refValue = refParts[1];
                                
                                if (refQualifier == "BT") // Batch Number
                                {
                                    if (currentLineItem != null)
                                    {
                                        currentLineItem.BatchNumber = refValue;
                                    }
                                }
                                else if (refQualifier == "ON") // Order Number
                                {
                                    if (string.IsNullOrEmpty(shipment.PoReference))
                                    {
                                        shipment.PoReference = refValue;
                                    }
                                }
                            }
                        }
                        break;
                        
                    case "CNT": // Control Total
                        if (elements.Length > 1)
                        {
                            var cntParts = elements[1].Split(':');
                            if (cntParts.Length > 1 && cntParts[0] == "2") // Total line items
                            {
                                // Could use for validation
                            }
                        }
                        break;
                }
            }
            
            // Calculate total packages if not set
            if (shipment.TotalPackages == null || shipment.TotalPackages == 0)
            {
                shipment.TotalPackages = shipment.Pallets.Count;
            }

            return shipment;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse DESADV: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Detect ASN format from content
    /// </summary>
    public string DetectFormat(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return "UNKNOWN";

        content = content.TrimStart();

        if (content.StartsWith("<?xml") || content.StartsWith("<sh:despatchAdviceMessage"))
            return "GS1_XML";
        
        if (content.StartsWith("ISA"))
            return "EDI_856";
        
        if (content.StartsWith("UNB") || content.StartsWith("UNH"))
            return "DESADV";

        return "UNKNOWN";
    }

    /// <summary>
    /// Parse ASN from any supported format
    /// </summary>
    public AsnShipment ParseAsn(string content)
    {
        var format = DetectFormat(content);
        
        return format switch
        {
            "GS1_XML" => ParseGS1Xml(content),
            "EDI_856" => ParseEdi856(content),
            "DESADV" => ParseDesadv(content),
            _ => throw new InvalidOperationException($"Unknown or unsupported ASN format")
        };
    }
}
