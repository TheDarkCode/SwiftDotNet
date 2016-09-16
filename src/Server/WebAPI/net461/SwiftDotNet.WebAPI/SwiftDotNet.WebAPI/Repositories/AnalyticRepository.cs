using System;
using System.Linq;
using SwiftDotNet.WebAPI.Entities.Analytics;
using SwiftDotNet.WebAPI.Helpers;

namespace SwiftDotNet.WebAPI.Repositories
{
    public class AnalyticRepository : RepositoryBase<Analytic>, IAnalyticRepository
    {
        public AnalyticRepository() : base("analytic", AppSettingsConfig.Db, AppSettingsConfig.MainCollection)
        {

        }
    }
}
