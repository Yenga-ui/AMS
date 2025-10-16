namespace AssetManagementSystem.Services
{
    public interface IAdAuthenticationService
    {
      Task<bool> AuthenticateAsync(string username, string password);
    }
}
