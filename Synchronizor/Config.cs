using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
	public class Config 
	{
		private static readonly Dictionary<string, object> InnerStorage;

		static Config()
		{
			InnerStorage = new Dictionary<string, object>();
			foreach (var key in ConfigurationManager.AppSettings.AllKeys)
			{
				InnerStorage.Add(key, ConfigurationManager.AppSettings[key]);
			}
			
		}
		public Object this[string key]
		{
			get
			{
				if (InnerStorage.ContainsKey(key)) return InnerStorage[key];
				throw new ApplicationException(string.Format("Config value with key {0} was not found.", key));
			}
			set
			{
				if (!InnerStorage.ContainsKey(key))
				{
					InnerStorage.Add(key, value);
				}
				InnerStorage[key] = value;
			}
		}

		private static Config _config;

		public static Config Global
		{
			get { return _config ?? (_config = new Config()); }
		}

	}
}
