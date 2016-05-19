using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using SwiftDotNet.WebAPI.Helpers;

namespace SwiftDotNet.WebAPI.PartitionRepositories
{
    /// <summary>
    /// This is the DocumentDB Partitioning client class that the RepositoryBase class
    /// will inherit to consume the properties.
    /// </summary>
    public class DocumentDbPartitioningClient : Disposable
    {
        #region ctors

        private Database _database;

        private readonly string _dbName;


        public DocumentDbPartitioningClient(string dbName)
        {
            this._dbName = dbName;
        }

        #endregion

        #region Public Properties

        private DocumentClient _client;
        public DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    // This could be handled differently.
                    string endpoint = AppSettingsConfig.EndPoint;
                    string authkey = AppSettingsConfig.AuthKey;

                    Uri endpointUri = new Uri(endpoint);
                    _client = new DocumentClient(endpointUri, authkey);
                }
                return _client;
            }
        }

        #endregion

        #region Private Methods

        private Database SetupDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                .Where(d => d.Id == _dbName)
                .AsEnumerable()
                .FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = _dbName }).Result;
            }
            return db;
        }

        protected Database Database
        {
            get
            {
                if (_database == null)
                {
                    _database = SetupDatabase();
                }

                return _database;
            }
        }

        #endregion

        #region Overrides

        protected override void DisposeCore()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }

        #endregion
    }
}
