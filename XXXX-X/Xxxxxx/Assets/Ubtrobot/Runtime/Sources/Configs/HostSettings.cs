using UnityEngine;
using Loki;
using System.Collections.Generic;

namespace Ubtrobot
{
	[System.Serializable]
	public struct HostConfig
	{
		public string connType;
		public string url;
	}

	[CreateAssetMenu(menuName = "Loki/Configs/Project/HostSettings", fileName = "HostSettings", order = -999)]
	public class HostSettings : UAssetObject
	{
		public static readonly string DefaultConnType = "deploy";

		[ConsoleField(aliasName = "host.conn")]
		public string connType = "deploy";

		[ConsoleField(aliasName = "host.debug")]
		public bool debug = false;

		public HostConfig[] configs;

		[InspectorMethod]
		[ConsoleMethod(aliasName = "host.default")]
		public void Default()
		{
			connType = DefaultConnType;
			debug = false;
		}

		public void OnEnable()
		{
			Default();
		}

		public void OnDisable()
		{
			Default();
		}

		public string GetHost(string def = "ws://127.0.0.1")
		{
			if (configs != null)
			{
				foreach (var kv in configs)
				{
					if (kv.connType == connType)
					{
						return kv.url;
					}
				}
			}
			return def;
		}

		public bool GetHost(string conn, out string host, string def = "ws://127.0.0.1")
		{
			if (configs != null)
			{
				foreach (var kv in configs)
				{
					if (kv.connType == conn)
					{
						host = kv.url;
						return true;
					}
				}
			}
			host = def;
			return false;
		}

		public bool FindHostEndsWith(string end, System.StringComparison comparison, out string connType, string def = "ws://127.0.0.1")
		{
			if (configs != null)
			{
				foreach (var kv in configs)
				{
					if (kv.url.EndsWith(end, comparison))
					{
						connType = kv.connType;
						return true;
					}
				}
			}

			connType = def;
			return false;
		}

		public void Reset()
		{
			configs = new HostConfig[]
			{
				new HostConfig(){ url = "ws://127.0.0.1", connType = "deploy" },
				new HostConfig(){ url = "webrpc://", connType = "server" },
			};
			//scratchWindmillSIMHost = "sim://Windmill";
			//scratchRoboticArmSIMHost = "sim://RoboticArm";
		}
	}
}
