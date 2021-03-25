// #define UBT_SHORT_PATH

namespace Ubtrobot
{
	public class SerializationConst
	{
		public const string programFlag = "@";
#if !UBT_SHORT_PATH
		public const string children = "children";
		public const string transform = "transform";
		public const string components = "components";
		public const string address = "address";
		public const string path = "path";
		public const string layer = "layer";
		public const string name = "name";
		public const string tag = "tag";
		public const string alloc = "alloc";
		public const string type = "type";
		public const string userDatas = "userDatas";
		public const string assets = "assets";
#else
		public const string children = "crs";
		public const string transform = "tr";
		public const string components = "cs";
		public const string address = "add";
		public const string path = "p";
		public const string layer = "l";
		public const string name = "n";
		public const string tag = "t";
		public const string alloc = "a";
		public const string type = "type";
		public const string userDatas = "user";
		public const string assets = "assets";
#endif
	}
}
