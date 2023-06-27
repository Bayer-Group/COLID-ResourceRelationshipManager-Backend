using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace COLID.ResourceRelationshipManager.Common.DataModels.Entity
{
    [NotMapped]
    public class Status
    {
        public Status(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (key.StartsWith("http"))
                {
                    Key = key;
                    Value = key?.Split('/')?.Last();
                }
                else
                {
                    Key = GetUri(key);
                    Value = key;
                }
            }
        }
        public string Key { get; set; }
        public string Value { get; set; }

        private static string GetUri(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (Enums.Status.Created.Contains(key, StringComparison.OrdinalIgnoreCase))
                    return Enums.Status.Created;
                else if (Enums.Status.Deleted.Contains(key, StringComparison.OrdinalIgnoreCase))
                    return Enums.Status.Deleted;
            }
            return key;
        }
    }
}
