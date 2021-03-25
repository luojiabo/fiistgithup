using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Runtime.Serialization;

namespace Loki
{
	[Serializable, DebuggerDisplay("Count = {Count}")]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector]
		private List<TKey> mKeys = new List<TKey>();
		[SerializeField, HideInInspector]
		private List<TValue> mValues = new List<TValue>();

		public SerializableDictionary()
		{
		}

		public SerializableDictionary(int capacity)
			: base(capacity)
		{
		}

		public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
			: base(dictionary)
		{
		}

		public SerializableDictionary(IEqualityComparer<TKey> comparer)
			: base(comparer)
		{
		}

		public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: base(dictionary, comparer)
		{
		}

		public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
			: base(capacity, comparer)
		{
		}

		protected SerializableDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public void OnBeforeSerialize()
		{
			mKeys.Clear();
			mValues.Clear();

			mKeys.AddRange(this.Keys);
			mValues.AddRange(this.Values);
		}

		public void OnAfterDeserialize()
		{
			DebugUtility.AssertFormat(mKeys.Count == mValues.Count, "Data corruption");
			this.Clear();
			int length = mKeys.Count;
			for (int i = 0; i < length; ++i)
			{
				Add(mKeys[i], mValues[i]);
			}

			mKeys.Clear();
			mValues.Clear();
		}
	}
}
