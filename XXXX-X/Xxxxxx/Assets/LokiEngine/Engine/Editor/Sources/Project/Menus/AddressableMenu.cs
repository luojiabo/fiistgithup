#if UNITY_ADDRESSABLE_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using Object = UnityEngine.Object;

namespace Loki
{
	public class AddressableMenu
	{
		[MenuItem("Assets/Loki/Addressable/Simple address", true)]
		static bool SimpleAddressValidate()
		{
			if (Selection.objects.Length > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		[MenuItem("Assets/Loki/Addressable/Simple address", false)]
		static void SimpleAddress()
		{
			var selected = Selection.objects;
			if (selected == null|| selected.Length == 0) return;

			var aaSettings = AddressableAssetSettingsDefaultObject.Settings;

			var entries = SetEntries(aaSettings, selected);

			for (int i = 0; i < entries.Count; i++)
			{
				entries[i].address = entries[i].MainAsset.name;
			}
		}


		static List<AddressableAssetEntry> SetEntries(AddressableAssetSettings aaSettings, Object[] targets)
		{
			if ( aaSettings.DefaultGroup.ReadOnly)
			{
				Debug.LogError("Current default group is ReadOnly.  Cannot add addressable assets to it");
				return null;
			}

			Undo.RecordObject(aaSettings, "AddressableAssetSettings");
			string path;
			var guid = string.Empty;

			var entriesAdded = new List<AddressableAssetEntry>();
			var result = new List<AddressableAssetEntry>();

			var modifiedGroups = new HashSet<AddressableAssetGroup>();

			AddressableAssetGroup modified = new AddressableAssetGroup();

			Type mainAssetType;

			foreach (var o in targets)
			{
				if (AddressableAssetUtility.GetPathAndGUIDFromTarget(o, out path, ref guid, out mainAssetType))
				{
					var entry = aaSettings.FindAssetEntry(guid);
					if (entry == null)
					{
						if (AddressableAssetUtility.IsInResources(path))
							AddressableAssetUtility.SafeMoveResourcesToGroup(aaSettings, aaSettings.DefaultGroup, new List<string> { path });
						else
						{
							var e = aaSettings.CreateOrMoveEntry(guid, aaSettings.DefaultGroup, false, false);
							entriesAdded.Add(e);
							modifiedGroups.Add(e.parentGroup);
						}
					}
					else
					{
						result.Add(entry);
					}
				}
			}

			foreach (var g in modifiedGroups)
				g.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);

			aaSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);

			result.AddRange(entriesAdded);

			return result;
		}
	}
}
#endif
