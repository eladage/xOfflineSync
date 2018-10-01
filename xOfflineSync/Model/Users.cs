using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace xOfflineSync
{
	public class Users
	{
		string id;
		string name;
		bool done;

		[JsonProperty(PropertyName = "id")]
		public string Id
		{
			get { return id; }
			set { id = value;}
		}

		[JsonProperty(PropertyName = "text")]
		public string Name
		{
			get { return name; }
			set { name = value;}
		}

		[JsonProperty(PropertyName = "complete")]
		public bool Done
		{
			get { return done; }
			set { done = value;}
		}

        [JsonProperty(PropertyName = "firstname")]
        public string FirstName
        {
            get;set;
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

