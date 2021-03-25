using System;
using System.Collections.Generic;

namespace Loki
{
	public interface ISerializer
	{
		int serializedVersion { get; }
	}
}
