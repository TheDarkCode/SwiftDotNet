using System;
using System.Linq;
using SwiftDotNet.DocumentDB.Entities.Analytics;
using SwiftDotNet.DocumentDB.Repositories;

namespace SwiftDotNet.DocumentDB.Repositories
{
    public class AnalyticRepository : RepositoryBase<Analytic>, IAnalyticRepository
    {
        public AnalyticRepository() : base("analytic", "{YOUR_DB_NAME}", "{YOUR_COLLECTION_NAME}", "{ENDPOINT}", "{AUTH_KEY}")
        {

        }
    }
}
