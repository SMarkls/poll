namespace Poll.Core.Configuration.Models;

public class LdapSettings
{
    public required string Domain { get; init; }
    public required string Server { get; init; }
    public required int Port { get; init; }
    public required bool UseSsl { get; init; }
    public required string UserCatalogRDN { get; init; }
    public required string ServiceAccountLogin { get; init; }
    public required string ServiceAccountPassword { get; init; }
}