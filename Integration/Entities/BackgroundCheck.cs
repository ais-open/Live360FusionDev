using System;
using Azure;
using Azure.Data.Tables;

namespace Live360.Integration.Entities;

public class BackgroundCheck : ITableEntity
{
    public Guid EntityID { get; set; }
    public string Name { get; set; }
    public bool IsSafeToHire { get; set; }

    // entity properties
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
