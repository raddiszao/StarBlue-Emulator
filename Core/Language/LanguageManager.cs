using Database_Manager.Database.Session_Details.Interfaces;
using log4net;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.Core.Language
{
    public class LanguageManager
    {
        private readonly Dictionary<string, string> _values;

        private static readonly ILog log = LogManager.GetLogger("StarBlue.Core.Language.LanguageManager");

        public LanguageManager()
        {
            _values = new Dictionary<string, string>();
        }

        public void Init()
        {
            if (_values.Count > 0)
            {
                _values.Clear();
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_locale`");
                DataTable Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        _values.Add(Row["key"].ToString(), Row["value"].ToString());
                    }
                }
            }

            log.Info(">> Loaded " + _values.Count + " Language Locales.");
        }

        public string TryGetValue(string value)
        {
            return _values.ContainsKey(value) ? _values[value] : "No language locale found for [" + value + "]";
        }
    }
}
