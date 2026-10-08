using BankingApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankingApp.Services
{
    public class BankingDatabaseService
    {
        private string dbFileName = "banking.db";
        private string localDbPath = string.Empty;
        private SQLiteAsyncConnection dbConnection;

        public BankingDatabaseService()
        {
            localDbPath = Path.Combine(FileSystem.Current.AppDataDirectory, dbFileName);
        }

        private async Task<SQLiteAsyncConnection> GetDbConnectionAsync()
        {
            if (dbConnection != null)
            {
                return dbConnection;
            }
                 

            if (!File.Exists(localDbPath))
                await ExtractDbfromMauiAssetAsync();

            dbConnection = new SQLiteAsyncConnection(localDbPath, storeDateTimeAsTicks: false);

            return dbConnection;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            SQLiteAsyncConnection connection = await GetDbConnectionAsync();

            return await connection.Table<Customer>().ToListAsync();
        }

        private async Task ExtractDbfromMauiAssetAsync()
        {
            using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(dbFileName);
            
            // Copy the file to the AppDataDirectory
            using FileStream outputStream = File.Create(localDbPath);
            await inputStream.CopyToAsync(outputStream);

        }
    }
}
