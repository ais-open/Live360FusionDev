
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Live360.Integration.Services;
public class Oauth2TokenService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public async Task<string> GetToken(string clientId, string clientSecret, string baseUrl, string resource = null)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("ClientId", clientId);

        var contents = $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}&resource={resource}";

        var response = await _httpClient.PostAsync(baseUrl, new StringContent(contents, Encoding.UTF8, "application/x-www-form-urlencoded"));

        string accessToken = string.Empty;
        if (response?.StatusCode == System.Net.HttpStatusCode.OK && response.Content is not null)
        {
            var oauthResponse = JsonConvert.DeserializeObject<Oauth2TokenResponse>(await response.Content.ReadAsStringAsync());
            if (oauthResponse is not null)
            {
                accessToken = oauthResponse.access_token;
            }
        }
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("No Access Token returned!");
        }
        return accessToken;
    }

    // only bothering spelling out the single field we care about
    record Oauth2TokenResponse (string access_token);
}

