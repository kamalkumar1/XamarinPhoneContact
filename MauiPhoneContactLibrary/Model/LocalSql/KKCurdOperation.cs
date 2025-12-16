using System;
using System.Diagnostics;
using SQLite;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Interface;
using MauiPhoneContactLibrary.Interface.LocalDB;
using MauiPhoneContactLibrary.Model.LocalSql;
using MauiPhoneContactLibrary.Model.LocalSql.Sqltable;

namespace MauiPhoneContactLibrary.Model;

public class KKCurdOperation : KKContactControlDbOperation, IKKCurdOperation
{

  public KKCurdOperation(ISqlLiteSetup sqlLiteSetup) : base(sqlLiteSetup)
  {
  }
  public async Task<int> InsertContactData(List<KKSqlTableForContact> contactModels)
  {
    try
    {
      var result = await GetSQLiteAsyncConnection().InsertAllAsync(contactModels);
      return result;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in InsertContactData: " + ex.Message);
    }
  }

  public async Task<int> UpsertContactData(KKSqlTableForContact contactModel)
  {
    try
    {
      var conn = GetSQLiteAsyncConnection();

      // Check if ContactID exists
      var existingContact = await conn.Table<KKSqlTableForContact>()
        .Where(c => c.ContactID == contactModel.ContactID)
        .FirstOrDefaultAsync();

      if (existingContact != null)
      {
        // Update existing record
        contactModel.Id = existingContact.Id; // Preserve the primary key
        return await conn.UpdateAsync(contactModel);
      }
      else
      {
        // Insert new record
        return await conn.InsertAsync(contactModel);
      }
    }
    catch (Exception ex)
    {
      throw new Exception("Error in UpsertContactData: " + ex.Message);
    }
  }

  public async Task<int> UpsertContactDataBulk(List<KKSqlTableForContact> contactModels)
  {
    try
    {
      var conn = GetSQLiteAsyncConnection();
      int totalAffected = 0;

      // Process each contact
      foreach (var contactModel in contactModels)
      {
        var existingContact = await conn.Table<KKSqlTableForContact>()
          .Where(c => c.ContactID == contactModel.ContactID)
          .FirstOrDefaultAsync();

        if (existingContact != null)
        {
          // Update existing record
          contactModel.Id = existingContact.Id;
          totalAffected += await conn.UpdateAsync(contactModel);
        }
        else
        {
          // Insert new record
          totalAffected += await conn.InsertAsync(contactModel);
        }
      }

      return totalAffected;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in UpsertContactDataBulk: " + ex.Message);
    }
  }
  public async Task<List<KKSqlTableForContact>> ReadContactData(int pageIndex)
  {
    try
    {

      Debug.WriteLine("pageIndex: " + pageIndex);
      int pageSize = ContactConfig.Instance.PageSize;
      Debug.WriteLine("_currentPageSize: " + pageSize);
      var skipCount = pageIndex * pageSize;
      Debug.WriteLine("skipCount: " + skipCount);


      var result = await GetSQLiteAsyncConnection()
    .QueryAsync<KKSqlTableForContact>(
        @"SELECT * FROM KKSqlTableForContact 
            ORDER BY 
              -- Level 1: A-Z first, then numbers, symbols, others
              CASE 
                WHEN DisplayName GLOB '[A-Za-z]*' THEN 0 
                WHEN DisplayName GLOB '[0-9]*' THEN 1
                WHEN DisplayName GLOB '[!@#$%^&*]*' THEN 2
                ELSE 3 
              END ASC,
              -- Level 2: Case-insensitive sort within each group
              upper(DisplayName) COLLATE NOCASE ASC 
            LIMIT ? OFFSET ?",
        pageSize, skipCount);

      //   var result = await GetSQLiteAsyncConnection()
      // .QueryAsync<KKSqlTableForContact>(
      //     @"SELECT * FROM KKSqlTableForContact 
      //       ORDER BY 
      //         CASE 
      //           WHEN DisplayName GLOB '[A-Za-z]*' THEN 0 
      //           ELSE 1 
      //         END ASC,
      //         upper(DisplayName) COLLATE NOCASE ASC 
      //       LIMIT ? OFFSET ?",
      //     pageSize, skipCount);

      //   var result = await GetSQLiteAsyncConnection()
      // .QueryAsync<KKSqlTableForContact>(
      //     "SELECT * FROM KKSqlTableForContact " +
      //     "ORDER BY upper(DisplayName) ASC " +
      //     "LIMIT ? OFFSET ?",
      //     pageSize, skipCount);
      // var result = await GetSQLiteAsyncConnection()
      //       .Table<KKSqlTableForContact>().OrderBy(c => c.DisplayName.ToUpperInvariant() ?? string.Empty)
      //       .Skip(skipCount)
      //       .Take(pageSize)
      //       .ToListAsync();
      return result;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in ReadContactData: " + ex.Message);
    }
  }
  public async Task<List<KKSqlTableForContact>> SearchAndReadContactData(string query, int pageIndex)
  {
    try
    {
      int pageSize = ContactConfig.Instance.PageSize;
      var skipCount = pageIndex * pageSize;
      var searchPattern = $"%{query}%";
      var startsWithPattern = $"{query}%";

      var result = await GetSQLiteAsyncConnection()
            .QueryAsync<KKSqlTableForContact>(
                @"SELECT * FROM KKSqlTableForContact 
                  WHERE DisplayName LIKE ? COLLATE NOCASE
                  ORDER BY 
                    CASE 
                      WHEN upper(DisplayName) = upper(?) THEN 0
                      WHEN upper(DisplayName) LIKE upper(?) THEN 1
                      ELSE 2
                    END ASC,
                    upper(DisplayName) COLLATE NOCASE ASC
                  LIMIT ? OFFSET ?",
                searchPattern,     // WHERE clause - contains anywhere
                query,             // Exact match (highest priority)
                startsWithPattern, // Starts with query
                pageSize,
                skipCount);

      return result;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in SearchAndReadContactData: " + ex.Message);
    }
  }

  public async Task<bool> GetFullSyncUpdate()
  {
    try
    {
      var result = await GetSQLiteAsyncConnection()
        .Table<KKSqlTableUpdate>()
        .Where(c => c.IsDataFullyLoaded == true)
        .FirstOrDefaultAsync();
      return result != null && result.IsDataFullyLoaded;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in GetUpdatedContacts: " + ex.Message);
    }
  }
  public async Task<bool> CheckContactExistsInDb(string contactId)
  {
    try
    {
      var result = GetSQLiteAsyncConnection();
      var count = await result.ExecuteScalarAsync<int>(
        "SELECT COUNT(*) FROM KKSqlTableForContact WHERE ContactID = ?",
        contactId);
      return count > 0;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error checking contact existence: {ex.Message}");
      return false;
    }
  }
  public async Task<int> DeleteContactsByIds(string contactIds)
  {
    try
    {
      var conn = GetSQLiteAsyncConnection();

      var result = await conn.ExecuteAsync(
        "DELETE FROM KKSqlTableForContact WHERE ContactID = ?",
        contactIds);
      Debug.WriteLine($"Deleted contact ID: {contactIds}, Rows affected: {result}");
      return result;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error deleting contacts: {ex.Message}");
      return 0;
    }
  }

  public async Task<int> InsertSyncUpdate(bool updateStatus)
  {
    try
    {
      var syncUpdate = new KKSqlTableUpdate
      {
        IsDataFullyLoaded = updateStatus,
      };
      var result = await GetSQLiteAsyncConnection().InsertAsync(syncUpdate);
      return result;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in InsertSyncUpdate: " + ex.Message);
    }
  }
  public async Task<int> TotalCount()
  {
    try
    {
      var result = await GetSQLiteAsyncConnection().Table<KKSqlTableForContact>().CountAsync();
      return result;
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex);
      return 0;
    }

  }

  public async Task<int> TotalCount(string query)
  {
    try
    {
      var searchPattern = $"%{query}%";
      var result = await GetSQLiteAsyncConnection()
                        .ExecuteScalarAsync<int>(
                            "SELECT COUNT(*) FROM KKSqlTableForContact WHERE DisplayName LIKE ? COLLATE NOCASE",
                            searchPattern);
      return result;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error in TotalCount with query: {ex.Message}");
      return 0;
    }
  }
}
