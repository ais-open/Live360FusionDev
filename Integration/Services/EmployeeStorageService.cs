using System;
using Azure.Data.Tables;
using Live360.Integration.Entities;

namespace Live360.Integration.Services;

public class EmployeeStorageService
{
    const string EMPLOYEE_TABLE_NAME = "employees";
    const string BACKGROUND_CHECK_TABLE_NAME = "background-checks";

    private readonly TableServiceClient _serviceClient;

    public EmployeeStorageService(string storageConnectionString)
    {
        _serviceClient = new TableServiceClient(storageConnectionString);
    }

    public Employee AddEmployee(string firstName, string lastName)
    {
        _serviceClient.CreateTableIfNotExists(EMPLOYEE_TABLE_NAME);
        var tableClient = _serviceClient.GetTableClient(EMPLOYEE_TABLE_NAME);

        var entity = new Employee {
            PartitionKey ="all_employees", 
            RowKey = $"{lastName}, {firstName}",
            FirstName = firstName,
            LastName = lastName
        };

        tableClient.UpsertEntity(entity);

        return entity;
    }

    public BackgroundCheck AddBackgroundCheck(Guid entityId, string name, bool isSafe)
    {
        _serviceClient.CreateTableIfNotExists(BACKGROUND_CHECK_TABLE_NAME);
        var tableClient = _serviceClient.GetTableClient(BACKGROUND_CHECK_TABLE_NAME);

        var entity = new BackgroundCheck {
            PartitionKey = "all_checks",
            RowKey = entityId.ToString(),
            EntityID = entityId,
            Name = name,
            IsSafeToHire = isSafe
        };

        tableClient.UpsertEntity(entity);

        return entity;
    }
} 