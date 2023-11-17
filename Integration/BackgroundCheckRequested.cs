using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Live360.Integration.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;

namespace Live360.Integration;

public class BackgroundCheckRequested
{
    private readonly IHttpClientFactory httpClientFactory;

    public BackgroundCheckRequested(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    [FunctionName("BackgroundCheckRequested")]
    public async Task Run(
        [ServiceBusTrigger("check-requested", Connection = "ServiceBusConnectionString")]
        RemoteExecutionContext context, // <<< DATAVERSE TYPE
        ILogger log)
    {
        // we're going to get a BackgroundCheck Entity in the message's "target" prop
        var entity = (Entity)context.InputParameters["Target"];

        var name = entity.GetAttributeValue<string>("ais_name");

        log.LogInformation(
            "GOT A MESSAGE: {CorrelationId}, BG Check ID: {BackgroundCheckID} Name: {Name}",
            context.CorrelationId, entity.Id, name);

        var isSafe = IsSafeToHire(name);

        // Save to Table Storage
        var service = new EmployeeStorageService(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        service.AddBackgroundCheck(entity.Id, name, isSafe);

        try
        {
            log.LogInformation("Saving Results - Name: {Name}, IsSafe? {IsSafe}", name, isSafe);
            await SaveResults(entity.Id, isSafe);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error Saving to Dataverse");
        }
    }

    private async Task SaveResults(Guid entityID, bool isSafe)
    {
        var tokenService = new Oauth2TokenService();

        // TODO: Configuration file
        var dataverseUrl = Environment.GetEnvironmentVariable("DataverseUrl");
        var tenant = "1b37c04e-260e-4189-a11b-0b8609ed3df0";
        var clientId = "5c9bc9e8-16d7-42e3-820d-b9220b293b33";
        var clientSecret = "WHf8Q~3Ip2gnBzAFRoEo2N-cE0Sz3HFEGSc~JcyI";
        var oauthUrl = $"https://login.microsoftonline.com/{tenant}/oauth2/token";

        var token = await tokenService.GetToken(clientId, clientSecret, oauthUrl, dataverseUrl);

        using var httpClient = httpClientFactory.CreateClient();

        // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors#web-api-url-and-versions
        httpClient.BaseAddress = new Uri(dataverseUrl + "/api/data/v9.2/");

        // Default headers for each Web API call.
        // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors#http-headers
        HttpRequestHeaders headers = httpClient.DefaultRequestHeaders;
        headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        headers.Add("OData-MaxVersion", "4.0");
        headers.Add("OData-Version", "4.0");
        headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var path = Environment.GetEnvironmentVariable("ApiName");
        await httpClient.PostAsJsonAsync(path, new
        {
            // NOTE: These must be the EXACT names as the Custom API (case-sensitive)
            BackgroundCheckID = entityID,
            IsSafeToHire = isSafe
        });
    }


    private bool IsSafeToHire(string name)
    {
        // Simulates a call to the external company's background check service.
        // turns out our background check company is sketchy too!
        return true;
    }
}
