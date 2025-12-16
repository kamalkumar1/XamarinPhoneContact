using System;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact.Model;


public class SqlDateList
{
    public string? Date { get; set; }
    public string? type { get; set; }
}
public class KKSqlTableForContact
{
    [PrimaryKey, AutoIncrement]             // Real PK for SQLite
    public int Id { get; set; }
    /// <summary>
    /// Unique id of contact
    /// </summary>
    public string? ContactID { get; set; }
    /// <summary>
    /// user Birthday date
    /// </summary>
    public string? Birthday { get; set; }
    public string? DisplayName { get; set; }
    public string? NameList { get; set; }
    public string? Emaillist { get; set; }
    public string? Urlslist { get; set; }
    public string? Phoneslist { get; set; }
    public string? Companylist { get; set; }
    public string? Addresslist { get; set; }
    public string? Datelist { get; set; }
}
