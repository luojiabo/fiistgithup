namespace Ubtrobot
{
	public class ProtocolFactory
	{
		public static IProtocol Generate(ProtocolOutput output, byte[] bytes, int offset, int length)
		{
			if (output == ProtocolOutput.ScratchToExplore)
			{
				return ExploreProtocol.Create(ExploreProtocolDataType.DTInt, bytes, offset, length, false, "JSONProtocol");
			}
			return null;
		}

		public static TProtocol Generate<TProtocol>(ProtocolOutput output, byte[] bytes, int offset, int length) where TProtocol : IProtocol
		{
			var result = Generate(output, bytes, offset, length);
			if (result is TProtocol)
			{
				return (TProtocol)result;
			}
			return default(TProtocol);
		}
	}
}
