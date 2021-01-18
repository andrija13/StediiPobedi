using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Redis;

namespace StediiPobedi
{
	public static class Config
	{
		public const bool IgnoreLongTests = true;

		public static string SingleHost
		{
			get { return "localhost"; }
		}
		public static readonly string[] MasterHosts = new[] { "localhost" };
		public static readonly string[] SlaveHosts = new[] { "localhost" };

		public const int RedisPort = 6379;

		public static string SingleHostConnectionString
		{
			get
			{
				return SingleHost + ":" + RedisPort;
			}
		}

		public static BasicRedisClientManager BasicClientManger
		{
			get
			{
				return new BasicRedisClientManager(new[] {
					SingleHostConnectionString
				});
			}
		}
	}
}
