using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Novell.Directory.Ldap;

namespace AssetManagementSystem.Services
{
    public class AdAuthenticationService : IAdAuthenticationService
    {
        private readonly string _ldapServer = "CidrzDC1.cidrz.org"; // e.g., ldap.mycompany.com
        private readonly int _ldapPort = 389; // Use 636 for LDAPS
        private readonly string _domain = "cidrz.org"; // e.g., MYCOMPANY

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            try
            {
                using var connection = new LdapConnection();
                await connection.ConnectAsync(_ldapServer, _ldapPort);
                await connection.BindAsync(username, password);

                return connection.Bound;
            }
            catch
            {
                return false;
            }
        }

    }
}
