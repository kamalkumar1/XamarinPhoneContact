using System;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact.Model;

// public class SqlName
// {
//     public string? Prefix { get; set; }
//     public string? Suffix { get; set; }
//     public string? FirstName { get; set; }
//     public string? MiddleName { get; set; }
//     public string? LastName { get; set; }
// }
// public class SqlEmailids
// {
//     // public string id { get; set; }
//     public string? Emailid { get; set; }
//     public string? Type { get; set; }
// }
// public class SqlUrl
// {
//     public string URL { get; set; }
// }
// public class SqlPhone
// {
//     // public string Phoneid { get; set;}
//     public string PhoneNumber { get; set; }
//     public string Type { get; set; }
// }
// public class SqlCompany
// {
//     public string CompanyName { get; set; }
//     public string Role { get; set; }
// }
// public class SqlAddress
// {
//     public string Type { get; set; }
//     public string FullAddress { get; set; }

// }
public class SqlDateList
{
    public string Date { get; set; }
    public string type { get; set; }
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
