using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using Poll.Core.Configuration.Models;
using Poll.Core.Entities.Ldap;
using Poll.Core.Extensions;
using Poll.Core.Interfaces;

namespace Poll.Infrastructure.Ldap;

public class LdapService : ILdapService
{
    private readonly LdapSettings _settings;

    private readonly string[] _userAttributeNames =
    [
        "objectguid",
        "distinguishedname",
        "samaccountname",
        "mail",
        "sn",
        "givenname",
        "middlename",
        "displayname",
        "manager",
        "useraccountcontrol"
    ];

    public LdapService(IOptions<LdapSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<LdapUser?> Login(string login, string password)
    {
        var connection = new LdapConnection();
        connection.UseSsl(true);
        await Bind(connection, login, password);

        if (!connection.Bound)
        {
            return null;
        }

        string searchFilter = $"(samaccountname={login})";
        foreach (var rdn in GetUserCatalogRdNs())
        {
            var search = await connection.SearchAsync(GetSearchBaseDN(rdn), LdapConnection.ScopeSub, searchFilter,
                _userAttributeNames, false, null, null);

            if (search.GetResponse() is not LdapSearchResult searchResult)
            {
                continue;
            }

            var userEntry = searchResult.Entry;
            return ParseUserEntry(userEntry);
        }

        return null;
    }

    /// <summary>
    /// Парсинг записи пользователя.
    /// </summary>
    /// <param name="ldapUser"></param>
    /// <returns>Пользователь LDAP.</returns>
    private static LdapUser ParseUserEntry(LdapEntry ldapUser)
    {
        var attrSet = ldapUser.GetAttributeSet();

        return new LdapUser
        {
            ObjectGuid = attrSet.TryGetValue("objectguid", out var guidAttr)
                ? new Guid(guidAttr.ByteValue)
                : new Guid(),
            DistinguishedName = attrSet.GetAttributeOrNull("distinguishedname")?.StringValue,
            Email = attrSet.GetAttributeOrNull("mail")?.StringValue,
            DisplayName = attrSet.GetAttributeOrNull("displayname")?.StringValue,
            FirstName = attrSet.GetAttributeOrNull("givenname")?.StringValue,
            LastName = attrSet.GetAttributeOrNull("sn")?.StringValue,
            MiddleName = attrSet.GetAttributeOrNull("middlename")?.StringValue,
            Login = attrSet.GetAttributeOrNull("samaccountname")?.StringValue,
            Manager = attrSet.GetAttributeOrNull("manager")?.StringValue,
            UserAccountControl = attrSet.GetAttributeOrNull("userAccountControl")?.StringValue,
        };
    }

    private async Task Bind(LdapConnection connection, string login, string password)
    {
        await connection.ConnectAsync(_settings.Server, _settings.Port);
        var domainComponents = GetDomainComponents();
        var subDomainName = domainComponents[0].ToLower();
        await connection.BindAsync($"{subDomainName}\\{login}", password);
    }

    private string GetSearchBaseDN(string rdn)
        => $"{rdn}," + GetServerDN();

    private string GetServerDN()
    {
        var domainComponents = GetDomainComponents();
        return string.Join(',', domainComponents.Select(dc => $"DC={dc.ToLower()}"));
    }

    private string[] GetDomainComponents()
    {
        var domainComponents = _settings.Domain.Split('.');
        if (domainComponents == null || domainComponents.Length == 0)
            throw new LdapException("Имя домена не задано");

        return domainComponents;
    }

    private string[] GetUserCatalogRdNs()
    {
        return _settings.UserCatalogRDN.Split(';');
    }
}