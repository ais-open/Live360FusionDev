using System;
using System.Net;
using Live360.Integration.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Live360.Integration;

public class AddEmployee
{
    private readonly ILogger<AddEmployee> _logger;

    public AddEmployee(ILogger<AddEmployee> log)
    {
        _logger = log;
    }

    [FunctionName(nameof(AddEmployee))]
    [OpenApiOperation(operationId: nameof(AddEmployee))]
    [OpenApiParameter(name: "first", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **First** parameter")]
    [OpenApiParameter(name: "last", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Last** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req)
    {
        string first = req.Query["first"];
        string last = req.Query["last"];
        string salary = req.Query["salary"];
        
        if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(last) || !Decimal.TryParse(salary, out var salaryDecimal)) 
        {
            throw new ArgumentException("'first','last' and 'salary' are required and salary must be a decimal");
        }

        var connStr = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        var service = new EmployeeStorageService(connStr);
        var employee = service.AddEmployee(first, last, salaryDecimal);

        return new OkObjectResult(employee.RowKey);
    }
}

