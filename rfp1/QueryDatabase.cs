// Quick database query tool
// Run with: dotnet script QueryDatabase.cs

using Microsoft.Data.Sqlite;
using System;

var dbPath = @"C:\Users\Ryan\Desktop\empauer\EmpauerLocal\src\EmpauerLocal.Web\empauer.db";

if (!File.Exists(dbPath))
{
    Console.WriteLine($"ERROR: Database not found at: {dbPath}");
    return;
}

Console.WriteLine("=== Empauer Database Query ===");
Console.WriteLine($"Database: {dbPath}");
Console.WriteLine($"Size: {new FileInfo(dbPath).Length} bytes");
Console.WriteLine("");

using var connection = new SqliteConnection($"Data Source={dbPath}");
connection.Open();

// Count assessments
using var cmd1 = connection.CreateCommand();
cmd1.CommandText = "SELECT COUNT(*) FROM Assessments";
var count = cmd1.ExecuteScalar();
Console.WriteLine($"Total Assessments: {count}");
Console.WriteLine("");

// List all assessments
using var cmd2 = connection.CreateCommand();
cmd2.CommandText = @"
    SELECT Code, Description, TypeCode, Status, CompanyName, 
           datetime(CreatedDate, 'localtime') as CreatedDate 
    FROM Assessments 
    ORDER BY CreatedDate DESC 
    LIMIT 20";
using var reader = cmd2.ExecuteReader();

Console.WriteLine("=== All Assessments ===");
Console.WriteLine($"{"Code",-12} {"Description",-30} {"Type",-15} {"Status",-12} {"Company",-20} {"Created Date"}");
Console.WriteLine(new string('-', 120));

while (reader.Read())
{
    Console.WriteLine($"{reader["Code"],-12} {reader["Description"],-30} {reader["TypeCode"],-15} {reader["Status"],-12} {reader["CompanyName"],-20} {reader["CreatedDate"]}");
}
reader.Close();
Console.WriteLine("");

// List lifecycle stages
using var cmd3 = connection.CreateCommand();
cmd3.CommandText = @"
    SELECT AssessmentCode, Title, Visible, SortOrder 
    FROM AssessmentLifecycleStages 
    ORDER BY AssessmentCode, SortOrder 
    LIMIT 50";
using var reader2 = cmd3.ExecuteReader();

Console.WriteLine("=== Lifecycle Stages ===");
Console.WriteLine($"{"Assessment Code",-15} {"Title",-30} {"Visible",-8} {"SortOrder"}");
Console.WriteLine(new string('-', 70));

while (reader2.Read())
{
    Console.WriteLine($"{reader2["AssessmentCode"],-15} {reader2["Title"],-30} {reader2["Visible"],-8} {reader2["SortOrder"]}");
}
reader2.Close();
Console.WriteLine("");

// Check recent activity
using var cmd4 = connection.CreateCommand();
cmd4.CommandText = @"
    SELECT Id, UserId, Action, EntityType, EntityId, 
           datetime(Timestamp, 'localtime') as Timestamp 
    FROM ActivityLogs 
    WHERE Action LIKE '%Assessment%' 
    ORDER BY Timestamp DESC 
    LIMIT 10";
using var reader3 = cmd4.ExecuteReader();

Console.WriteLine("=== Recent Assessment Activity ===");
while (reader3.Read())
{
    Console.WriteLine($"ID: {reader3["Id"]}, User: {reader3["UserId"]}, Action: {reader3["Action"]}, Entity: {reader3["EntityType"]} {reader3["EntityId"]}, Time: {reader3["Timestamp"]}");
}
reader3.Close();

Console.WriteLine("");
Console.WriteLine("=== Done ===");




