using System;
using System.Text;
using Loki;

namespace Ubtrobot
{
    public class BridgeCenter : Singleton<BridgeCenter>
    {
		private CommonBridgeCaller caller = new CommonBridgeCaller();
        public static CommonBridgeCaller Caller => GetOrAlloc().caller;

		protected override void OnInitialize()
		{
			BridgeReceiver.GetOrAlloc().SetCaller(caller);
		}

	}
}
