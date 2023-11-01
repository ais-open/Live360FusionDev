using Azure.Data.Tables;

namespace Live360.Accounting;

public class EmployeeStorageService
{
    private TableServiceClient _serviceClient;

    public EmployeeStorageService(string storageConnectionString)
    {
        _serviceClient = new TableServiceClient(storageConnectionString);

        // Create a new table. The TableItem class stores properties of the created table.
        _serviceClient.CreateTableIfNotExists("employees");
    }

    public Employee AddEmployee(string firstName, string lastName)
    {
        var tableClient = _serviceClient.GetTableClient("employees");

        var entity = new Employee {
            PartitionKey ="all_employees", 
            RowKey = $"{lastName}, {firstName}",
            FirstName = firstName,
            LastName = lastName
        };

        tableClient.UpsertEntity(entity);

        return entity;
    }
}