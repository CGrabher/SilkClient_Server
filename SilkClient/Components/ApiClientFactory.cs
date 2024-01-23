

using System.Security.Cryptography;
using SilkClient.api;
using System.Text;

namespace SilkClient.Components;
public class ApiClientFactory
{
    // the current session, shared by all components
    LoginSession _session;

    public ApiClientFactory(LoginSession session)
    {
        _session = session;
    }
    /// <summary>
    /// Returns a configured Client so everbyod can use this.
    /// 1.This way we have only one location where we have to change the serveport if necessary.
    /// 2.The Cleint hold the Bearertoken so we dont have to take care og this in every page.
    /// </summary>
    /// <returns></returns>
    public Client CreateClient()
    {
        var httpClient = new HttpClient();
        if (_session.LoggedIn){
            // if we currently logged in, add the bearer token to the client
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.BearerToken);
        }
        return new Client("http://localhost:5293", httpClient);
    }
    
}