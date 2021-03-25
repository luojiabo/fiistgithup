using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loki
{
	public static class EngineComparers
	{
		public static readonly ModuleAttributeComparer defaultModuleAttributeComparer = new ModuleAttributeComparer();
	}

	public class ModuleAttributeComparer : IComparer<Type>
	{
		public int Compare(Type x, Type y)
		{
			ModuleAttribute xAttr = x.GetCustomAttribute<ModuleAttribute>(true);
			ModuleAttribute yAttr = y.GetCustomAttribute<ModuleAttribute>(true);
			if (xAttr == null)
				return -1;

			if (yAttr == null)
				return 1;

			if (xAttr.engine && !yAttr.engine)
				return -1;

			if (!xAttr.engine && yAttr.engine)
				return 1;

			return xAttr.order.CompareTo(yAttr.order);
		}
	}

}
