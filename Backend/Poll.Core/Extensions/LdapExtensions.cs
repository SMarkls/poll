using Novell.Directory.Ldap;

namespace Poll.Core.Extensions;

public static class LdapExtensions
{
    public static LdapAttribute? GetAttributeOrNull(this LdapAttributeSet attributeSet, string attributeName)
        => attributeSet.GetValueOrDefault(attributeName);

    public static void UseSsl(this LdapConnection connection, bool skipCertificateValidation = false)
    {
        connection.SecureSocketLayer = true;
        if (skipCertificateValidation)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            connection.UserDefinedServerCertValidationDelegate += (sender, certificate, chain, sslPolicyErrors) => true;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}