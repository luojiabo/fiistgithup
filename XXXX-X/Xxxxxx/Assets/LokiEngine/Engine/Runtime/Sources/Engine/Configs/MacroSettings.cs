using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Loki
{

	[CreateAssetMenu(menuName = "Loki/Configs/Editor/Macro Settings", fileName = "MacroSettings")]
	public class MacroSettings : UEditorAssetObject
	{
#if UNITY_EDITOR
		private static readonly char[] msDefaultSymbolSplit = new[] { ';' };

		[SerializeField]
		public SerializableDictionaryBLS symbols;

		[SerializeField]
		public char[] symbolSplit = msDefaultSymbolSplit;

		public char[] GetSymbolSplit()
		{
			if (symbolSplit == null || symbolSplit.Length ==0)
			{
				return msDefaultSymbolSplit;
			}
			return symbolSplit;
		}

		private void OnEnable()
		{
			Fetch();
		}

		[InspectorMethod]
		private void Apply()
		{
			//DebugUtility.LogTrace(LoggerTags.Engine, "Apply");
			foreach (var kv in symbols)
			{
				if (kv.Key == BuildTargetGroup.Unknown)
					continue;

				var noRepeatingList = kv.Value.RemoveRepeating();
				if (noRepeatingList.Count > 0)
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(kv.Key, string.Join(";", noRepeatingList));
				}
				else
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(kv.Key, "");
				}
			}
		}

		[InspectorMethod]
		private void Fetch()
		{
			if (symbols == null)
				return;

			foreach (var group in symbols.Keys)
			{
				if (group == BuildTargetGroup.Unknown)
					continue;

				var groupValues = symbols[group];
				if (groupValues == null)
				{
					groupValues = new SerializableNestListString();
					symbols[group] = groupValues;
				}
				if (groupValues.Count == 0)
				{
					string strSyms = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
					string[] allSymbols = strSyms.Split(GetSymbolSplit());

					foreach (var sym in allSymbols)
					{
						if (string.IsNullOrEmpty(sym))
							continue;
						groupValues.Union(sym);
					}
				}
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
			OnEnable();
			//PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.)
		}
#endif
	}
}
