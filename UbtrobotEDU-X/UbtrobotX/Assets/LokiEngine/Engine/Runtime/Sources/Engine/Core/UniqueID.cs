using System;
using System.Collections.Generic;

namespace Loki
{
	public struct UniqueID : IEqualityComparer<UniqueID>, IEquatable<UniqueID>, IComparable<UniqueID>, IComparable
	{
		public static readonly UniqueID Empty;

		private readonly Guid mGuid;

		public static implicit operator UniqueID(Guid guid)
		{
			return new UniqueID(guid);
		}

		public static bool operator ==(UniqueID lhs, UniqueID rhs)
		{
			return lhs.mGuid == rhs.mGuid;
		}

		public static bool operator !=(UniqueID lhs, UniqueID rhs)
		{
			return lhs.mGuid != rhs.mGuid;
		}

		public static UniqueID New()
		{
			return Guid.NewGuid();
		}

		static UniqueID()
		{
			Empty = Guid.Empty;
		}

		private UniqueID(Guid guid)
		{
			mGuid = guid;
		}

		public override string ToString()
		{
			return mGuid.ToString();
		}

		public override bool Equals(object o)
		{
			if (o is UniqueID)
				return Equals((UniqueID)o);
			return false;
		}

		public override int GetHashCode()
		{
			return mGuid.GetHashCode();
		}

		public bool Equals(UniqueID x, UniqueID y)
		{
			return x.mGuid == y.mGuid;
		}

		public bool Equals(UniqueID other)
		{
			return mGuid == other.mGuid;
		}

		public int GetHashCode(UniqueID obj)
		{
			return obj.mGuid.GetHashCode();
		}

		public int CompareTo(UniqueID other)
		{
			return mGuid.CompareTo(other.mGuid);
		}

		public int CompareTo(object obj)
		{
			if (obj is UniqueID)
				return CompareTo((UniqueID)obj);
			return -1;
		}
	}
}
