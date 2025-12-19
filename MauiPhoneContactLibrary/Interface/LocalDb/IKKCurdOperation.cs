using System;
using SQLite;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Model;

namespace MauiPhoneContactLibrary.Interface.LocalDB;

public interface IKKCurdOperation
{
   public Task<int> InsertContactData(List<KKSqlTableForContact> contactModels);
   public Task<int> TotalCount();
   public Task<int> TotalCount(string Query);
   public Task<List<KKSqlTableForContact>> ReadContactData(int pageIndex);
   public Task<List<KKSqlTableForContact>> SearchAndReadContactData(string Query, int pageIndex);
   public Task<int> UpsertContactDataBulk(List<KKSqlTableForContact> contactModels);
   public Task<int> UpsertContactData(KKSqlTableForContact contactModel);
   public Task<bool> GetFullSyncUpdate();
   public Task<int> InsertSyncUpdate(bool updateStatus);
   public Task<int> DeleteContactsByIds(string contactIds);
   public Task<bool> CheckContactExistsInDb(string contactId);

}
