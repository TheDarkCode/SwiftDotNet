using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace SwiftDotNet.DocumentDB.Repositories
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

        public DocumentDbClient(string dbName, string collectionName, string endpoint, string authKey)
        {
            this._endpoint = endpoint;
            this._authkey = authKey;
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
