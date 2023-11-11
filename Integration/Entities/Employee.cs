using System;
using Azure;
using Azure.Data.Tables;

namespace Live360.Integration.Entities;

public class Employee : ITableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    // entity properties
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
