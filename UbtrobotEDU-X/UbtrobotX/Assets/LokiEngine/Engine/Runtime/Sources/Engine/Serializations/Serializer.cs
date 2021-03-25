using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Loki
{
	[Serializable]
	public abstract class Serializer : ISerializable, ISerializer
	{
		protected int mSerializedVersion = 1;

		public int serializedVersion
		{
			get
			{
				return mSerializedVersion;
			}
			protected set
			{
				mSerializedVersion = value;
			}
		}

		protected Serializer()
		{

		}

		protected Serializer(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new System.ArgumentNullException("info");

			mSerializedVersion = info.GetInt32("SerializedVersion");
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new System.ArgumentNullException("info");

			info.AddValue("SerializedVersion", mSerializedVersion);

		}
	}
}
