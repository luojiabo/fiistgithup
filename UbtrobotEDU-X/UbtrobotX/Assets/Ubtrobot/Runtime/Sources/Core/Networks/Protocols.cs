using System.Text;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	public enum ProtocolFormat
	{
		JSON,
		Protobuf2,
		Protobuf3,
	}

	public enum ProtocolCode
	{
		Success,
		Failure,
	}

	public enum ProtocolOutput
	{
		ExploreToScratch,
		ScratchToExplore,
	}

	public interface IProtocol
	{
		string host { get; set; }
		void ToBytes(ProtocolOutput output, out bool encryption, out byte[] bytes);
		void ToString(ProtocolOutput output, out string result);
	}
}

