using LitJson;

namespace Ubtrobot
{
    public static class BridgeUtility
    {
        public static JsonData EmptyArray
        {
            get
            {
                var json = new JsonData();
                json.SetJsonType(JsonType.Array);
                return json;
            }
        }

        public static JsonData EmptyObject
        {
            get
            {
                var json = new JsonData();
                json.SetJsonType(JsonType.Object);
                return json;
            }
        }

		public static bool AsBool(this int value)
		{
			return value == 1;
		}

		public static int AsInt(this bool value)
		{
			return value ? 1 : 0;
		}
	}
}
