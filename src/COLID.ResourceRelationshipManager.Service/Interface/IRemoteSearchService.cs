using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace COLID.ResourceRelationshipManager.Services.Interface
{
    public interface IRemoteSearchService
    {

        /// <summary>
        /// Get Documents By PID URIs
        /// </summary>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        Task<IDictionary<string, IEnumerable<JObject>>> GetDocumentsByIds(IEnumerable<string> identifiers);
    }
}
