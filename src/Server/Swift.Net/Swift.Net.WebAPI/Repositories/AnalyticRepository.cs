using System;
using System.Linq;
using Swift.Net.WebAPI.Entities.Analytics;
using Swift.Net.WebAPI.Repositories;
using Microsoft.Azure.Documents.Client;

namespace Swift.Net.WebAPI.Repositories
{
    public class AnalyticRepository : RepositoryBase<Analytic>, IAnalyticRepository
    {
        public AnalyticRepository(string docType = "analytic", string database = "{YOUR_DB_NAME}", string collection = "{YOUR_COLLECTION_NAME}", string endpoint = "{ENDPOINT}", string authKey = "{AUTH_KEY}") : base(docType, database, collection, endpoint, authKey)
        {

        }

        public AnalyticRepository(DocumentClient client, string docType = "analytic", string dbName = "{YOUR_DB_NAME}", string collectionName = "{YOUR_COLLECTION_NAME}") : base(docType, client, dbName, collectionName)
        {

        }
    }
}
