using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace xOfflineSync
{
	public class Users
	{

		[JsonProperty(PropertyName = "id")]
		public string Id
		{
            get; set;
        }

		[JsonProperty(PropertyName = "text")]
		public string Name
		{
            get; set;
        }

		[JsonProperty(PropertyName = "isDeleted")]
		public bool Done
		{
            get; set;
		}

        [JsonProperty(PropertyName = "firstname")]
        public string FirstName
        {
            get; set;
        }

        [JsonProperty(PropertyName = "lastname")]
        public string LastName
        {
            get; set;
        }

        

        [Version]
        public string Version { get; set; }
	}
}

