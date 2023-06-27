using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace COLID.ResourceRelationshipManager.Common.Enums
{
    public static class Status
    {
        private static readonly string _basePath = Path.GetFullPath("appsettings.json");
        private static readonly string _filePath = _basePath[..^16];
        private static readonly IConfigurationRoot configuration = new ConfigurationBuilder()
                     .SetBasePath(_filePath)
                    .AddJsonFile("appsettings.json")
                    .Build();
        private static readonly string _serviceUrl = configuration.GetValue<string>("ServiceUrl");
        public static readonly string Deleted = _serviceUrl + "kos/19050/Deleted";
        public static readonly string Created = _serviceUrl + "kos/19050/Created";
    }
}
