using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SwiftDotNet.DocumentDB.Entities.Analytics;
using SwiftDotNet.DocumentDB.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDotNet.Core.Controllers.api
{
    [Route("api/[controller]")]
    public class AnalyticsController : Controller
    {

        #region ctors

        private readonly IAnalyticRepository _repo;
        private static HttpClient _client = new HttpClient();

        public AnalyticsController(IAnalyticRepository repo)
        {
            this._repo = repo;
        }

        #endregion

        #region Standard CRUD

        #endregion

        #region Private Methods
        
        #endregion
    }
}
