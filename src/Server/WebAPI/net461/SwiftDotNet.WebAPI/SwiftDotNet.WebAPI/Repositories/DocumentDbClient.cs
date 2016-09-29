using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using SwiftDotNet.WebAPI.Helpers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;


namespace SwiftDotNet.WebAPI.Repositories
{
    /// <summary>
    /// This is the DocumentDB client class that the RepositoryBase class
    /// will inherit to consume the properties.
    /// </summary>
    public class DocumentDbClient //: Disposable
    {
        #region ctors

        private Database _database;

        private readonly string _endpoint;

        private readonly string _authkey;

        private readonly string _dbName;

        private readonly string _collectionName;

        public DocumentDbClient(string dbName, string collectionName, string endpoint = null, string authkey = null)
        {
            this._endpoint = endpoint ?? AppSettingsConfig.EndPoint;
            this._authkey = authkey ?? AppSettingsConfig.AuthKey;
            this._dbName = dbName;
            this._collectionName = collectionName;
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
                    //// This could be handled differently.
                    //string endpoint = AppSettingsConfig.EndPoint;
                    //string authkey = AppSettingsConfig.AuthKey;

                    Uri endpointUri = new Uri(_endpoint);
                    _client = new DocumentClient(endpointUri, _authkey);
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

        private async Task SetupCollection(string databaseLink)
        {
            _collection = Client.CreateDocumentCollectionQuery(databaseLink)
                .Where(c => c.Id == _collectionName)
                .AsEnumerable()
                .FirstOrDefault();

            if (_collection == null)
            {
                var collectionSpec = new DocumentCollection { Id = _collectionName };
                _collection = await Client.CreateDocumentCollectionAsync(databaseLink, collectionSpec);
            }
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

        private DocumentCollection _collection;
        protected DocumentCollection Collection
        {
            get
            {
                if (_collection == null)
                {
                    SetupCollection(Database.SelfLink).Wait();
                }
                return _collection;
            }
        }

        #endregion

        #region Overrides

        //protected override void DisposeCore()
        //{
        //    if (_client != null)
        //    {
        //        _client.Dispose();
        //    }
        //}

        #endregion
    }
}
