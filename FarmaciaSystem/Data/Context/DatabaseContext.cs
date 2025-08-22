using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Context
{
    public class DatabaseContext : IDisposable
    {
        private SQLiteConnection _connection;
        private SQLiteTransaction _transaction;

        public DatabaseContext()
        {
            DatabaseConfig.EnsureDatabaseDirectory();
            _connection = new SQLiteConnection(DatabaseConfig.ConnectionString);
        }

        public async Task OpenAsync()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }
        }

        public async Task<SQLiteTransaction> BeginTransactionAsync()
        {
            await OpenAsync();
            _transaction = _connection.BeginTransaction();
            return _transaction;
        }

        public SQLiteConnection GetConnection()
        {
            return _connection;
        }

        public SQLiteTransaction GetTransaction()
        {
            return _transaction;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
