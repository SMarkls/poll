namespace Poll.Core.Entities.Ldap;

public class LdapUser
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public Guid ObjectGuid { get; set; }

    /// <summary>
    /// Отличительное имя.
    /// </summary>
    public string? DistinguishedName { get; set; }

    /// <summary>
    /// Логин.
    /// </summary>
    public string? Login { get; set; }

    /// <summary>
    /// Электронная почта.
    /// </summary>
    public string? Email { get; set; }

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
    /// Внутренний телефонный номер.
    /// </summary>
    public string InternalPhone { get; set; }

    /// <summary>
    /// Сотовый номер телефона.
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// Контроль учетных записей пользователей.
    /// </summary>
    public string? UserAccountControl { get; set; }
}