

using System.Security.Cryptography;
using SilkClient.api;
using System.Text;

namespace SilkClient.Components;
public class LoginSession
{
    public string BearerToken { get; set; } = "";
    public bool LoggedIn { get; set; } = false;
    public string User { get; set; } = "";

    // This is the event that will be called when the login state changes
    public event Action OnChange;

    public void NotifyLogin() {
        OnChange?.Invoke();
    }
    
}