using MongoDB.Bson.Serialization.Attributes;

namespace Poll.Core.Entities.Ldap;

[BsonIgnoreExtraElements]
public class LdapUser
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public string ObjectGuid { get; set; }

    /// <summary>
    /// Отличительное имя.
    /// </summary>
    public string? DistinguishedName { get; set; }

    /// <summary>
    /// Логин.
    /// </summary>
    public string? Login { get; set; }

    /// <summary>
    /// Отображаемое имя.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Отчество.
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Должность.
    /// </summary>
    public string Position { get; set; }

    /// <summary>
    /// Отдел.
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// Руководитель.
    /// </summary>
    public string? Manager { get; set; }

    /// <summary>
    /// Роль пользователя
    /// </summary>
    public UserRole Role { get; set; }
}