using Poll.Core.Entities.Ldap;

namespace Poll.Core.Interfaces;

public interface ILdapService
{
    Task<LdapUser?> Login(string login, string password);
    Task<LdapUser?> GetFromDb(string userId);
}