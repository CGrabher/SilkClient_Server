

using System.Security.Cryptography;
using SilkClient.api;
using System.Text;

namespace SilkClient.Components;
public class LoginService
{

    public LoginService(LoginSession session, ApiClientFactory apiClientFactory){
        Session = session;
        ApiClientFactory = apiClientFactory;
    }

    public LoginSession Session { get; }
    public ApiClientFactory ApiClientFactory { get; }

    public async Task<string> HashPassword(string username, string password)
    {
        // perform call to ask for server's salt
        var salt = await ApiClientFactory.CreateClient().LoginSaltAsync(new LoginSaltRequest() { Username = username });

        StringBuilder hashBuilder = new StringBuilder();
        using (SHA256 sha = SHA256.Create())
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            // convert byte array to a hex string
            foreach (byte b in data){
                hashBuilder.AppendFormat("{0:x2}", b);
            }
        }
        return hashBuilder.ToString();
    }

    public async Task Logout()
    {
        await ApiClientFactory.CreateClient().LogoutAsync();
        Session.LoggedIn = false;
        Session.User = "";
        Session.BearerToken = "";
        Session.NotifyLogin();
    }
    
}