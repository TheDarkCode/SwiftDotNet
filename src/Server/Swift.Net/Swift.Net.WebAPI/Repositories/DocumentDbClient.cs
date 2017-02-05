using System;
using System.Configuration;
using System.Security;
using System.Linq;
using System.Threading.Tasks;
using Swift.Net.WebAPI.Helpers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;


namespace Swift.Net.WebAPI.Repositories
{
    /// <summary>
    /// This is the DocumentDB client class that the RepositoryBase class
    /// will inherit to consume the properties.
    /// </summary>
    public class DocumentDbClient : Disposable
    {
        #region ctors

        private Database _database;

        private readonly string _endpoint;

        private readonly Uri _endpointUri;

        private readonly string _authkey;

        private readonly SecureString _secureAuthKey;

        private readonly string _dbName;

        private readonly string _collectionName;

        private readonly ConnectionMode _connectionMode;

        private readonly Protocol _protocol;

        public DocumentDbClient(string dbName, string collectionName, string endpoint = null, string authkey = null, ConnectionMode connectionMode = ConnectionMode.Direct, Protocol protocol = Protocol.Tcp)
        {
            this._endpoint = endpoint;
            this._authkey = authkey;
            this._dbName = dbName;
            this._collectionName = collectionName;
            this._connectionMode = connectionMode;
            this._protocol = protocol;
        }

        public DocumentDbClient(DocumentClient client, string dbName, string collectionName, ConnectionMode connectionMode = ConnectionMode.Direct, Protocol protocol = Protocol.Tcp)
        {
            this._client = client;
            this._endpointUri = client.ReadEndpoint;
            this._secureAuthKey = client.AuthKey;
            this._dbName = dbName;
            this._collectionName = collectionName;
            this._connectionMode = connectionMode;
            this._protocol = protocol;
        }

        public DocumentDbClient(DocumentClient client, DocumentCollection collection, ConnectionMode connectionMode = ConnectionMode.Direct, Protocol protocol = Protocol.Tcp)
        {
            this._client = client;
            this._collection = collection;
            this._endpointUri = client.ReadEndpoint;
            this._secureAuthKey = client.AuthKey;
            this._connectionMode = connectionMode;
            this._protocol = protocol;
        }

        #endregion

        #region Public Properties

        private DocumentClient _client;
        public DocumentClient Client
        {
            get
            {
                if (_client == null && _endpoint != null && _authkey != null)
                {
                    Uri endpointUri = new Uri(_endpoint);
                    _client = new DocumentClient(endpointUri, _authkey, new ConnectionPolicy
                    {
                        ConnectionMode = _connectionMode,
                        ConnectionProtocol = _protocol
                    });
                }
                else if (_client == null && _endpointUri != null && _secureAuthKey != null)
                {
                    _client = new DocumentClient(_endpointUri, _secureAuthKey, new ConnectionPolicy
                    {
                        ConnectionMode = _connectionMode,
                        ConnectionProtocol = _protocol
                    });
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
