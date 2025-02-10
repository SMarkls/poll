using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Novell.Directory.Ldap;
using Poll.Core.Configuration.Models;
using Poll.Core.Entities.Ldap;
using Poll.Core.Extensions;
using Poll.Core.Interfaces;
using Poll.Infrastructure.MongoConnection;

namespace Poll.Infrastructure.Ldap;

public class LdapService : ILdapService
{
    private static LdapSettings? _settings;
    private readonly IMongoCollection<LdapUser> _collection;
#if DEBUG
    private static TestingSettings? _testingSettings;
#endif

    private static readonly string[] UserAttributeNames =
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

    public LdapService(IOptions<LdapSettings> settings, IMongoCollectionFactory<LdapUser> factory,
#if DEBUG
         IOptions<TestingSettings> testingSettings
#endif
       )
    {
        _settings ??= settings.Value;
        _collection = factory.GetCollection();
#if DEBUG
         _testingSettings ??= testingSettings.Value;
#endif
    }

    public async Task<LdapUser?> Login(string login, string password)
    {
#if DEBUG
        if (TryDebugLogin(login, out var testUser))
        {
            return testUser;
        }
#endif
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
                UserAttributeNames, false, null, null);

            if (search.GetResponse() is not LdapSearchResult searchResult)
            {
                continue;
            }

            var userEntry = searchResult.Entry;
            string? userId = userEntry.GetAttributeSet().TryGetValue("objectguid", out var guidAttr)
                ? new Guid(guidAttr.ByteValue).ToString()
                : null;
            if (userId is null)
            {
                throw new Exception("В сущности из LDAP не представлено поле с идентификатором.");
            }

            var user = await GetFromDb(userId);
            if (user is not null)
            {
                return user;
            }

            user = ParseUserEntry(userEntry);
            user.Role = UserRole.User;
            await AddToDb(user);

            return user;
        }

        return null;
    }

    private Task AddToDb(LdapUser user)
    {
        return _collection.InsertOneAsync(user);
    }

    public async Task<LdapUser?> GetFromDb(string userId)
    {
        var cursor = await _collection.FindAsync(x => x.ObjectGuid == userId);
        return await cursor.FirstOrDefaultAsync();
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
                ? new Guid(guidAttr.ByteValue).ToString()
                : throw new Exception("ObjectGuid пуст"),
            DistinguishedName = attrSet.GetAttributeOrNull("distinguishedname")?.StringValue,
            DisplayName = attrSet.GetAttributeOrNull("displayname")?.StringValue,
            FirstName = attrSet.GetAttributeOrNull("givenname")?.StringValue,
            LastName = attrSet.GetAttributeOrNull("sn")?.StringValue,
            MiddleName = attrSet.GetAttributeOrNull("middlename")?.StringValue,
            Login = attrSet.GetAttributeOrNull("samaccountname")?.StringValue,
            Manager = attrSet.GetAttributeOrNull("manager")?.StringValue,
        };
    }

    private static async Task Bind(LdapConnection connection, string login, string password)
    {
        await connection.ConnectAsync(_settings!.Server, _settings.Port);
        var domainComponents = GetDomainComponents();
        var subDomainName = domainComponents[0].ToLower();
        await connection.BindAsync($"{subDomainName}\\{login}", password);
    }

    private static string GetSearchBaseDN(string rdn)
        => $"{rdn}," + GetServerDN();

    private static string GetServerDN()
    {
        var domainComponents = GetDomainComponents();
        return string.Join(',', domainComponents.Select(dc => $"DC={dc.ToLower()}"));
    }

    private static string[] GetDomainComponents()
    {
        var domainComponents = _settings!.Domain.Split('.');
        if (domainComponents == null || domainComponents.Length == 0)
            throw new LdapException("Имя домена не задано");

        return domainComponents;
    }

    private static string[] GetUserCatalogRdNs()
    {
        return _settings!.UserCatalogRDN.Split(';');
    }

#if DEBUG
    private static bool TryDebugLogin(string login, [NotNullWhen(true)] out LdapUser? user)
    {
        if (_testingSettings.Users.Contains(login))
        {
            user = new LdapUser
            {
                ObjectGuid = "d54fb117-2a43-4f89-a6f6-c2b3dd5dffa3",
                DisplayName = login,
                FirstName = "Тестер",
                LastName = "Тестовый",
                MiddleName = "Тестович",
                Login = login,
                Position = "Тестировщик",
                Role = UserRole.User,
            };

            return true;
        }

        if (_testingSettings.Pollers.Contains(login))
        {
             user = new LdapUser
             {
                 ObjectGuid = "d54fb117-2a43-4f89-a6f6-c2b3dd5dffa3",
                 DisplayName = login,
                 FirstName = "Тестер",
                 LastName = "Тестовый",
                 MiddleName = "Тестович",
                 Login = login,
                 Position = "Тестировщик",
                 Role = UserRole.Poller,
             };
 
             return true;
        }

        if (_testingSettings.Admins.Contains(login))
        {
              user = new LdapUser
              {
                  ObjectGuid = "d54fb117-2a43-4f89-a6f6-c2b3dd5dffa3",
                  DisplayName = login,
                  FirstName = "Тестер",
                  LastName = "Тестовый",
                  MiddleName = "Тестович",
                  Login = login,
                  Position = "Тестировщик",
                  Role = UserRole.Admin,
              };
  
              return true;           
        }

        user = null;
        return false;
    }
#endif
}