using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loki;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ubtrobot
{
	[CreateAssetMenu(menuName = "Loki/Configs/Project/Ubtrobot Num-Material Settings", fileName = "NumMaterialSettings", order = -999)]
	public class NumMaterialSettings : UAssetObject
	{
		[SerializeField]
		private Material[] mNumMaterials = null;

		[SerializeField]
		private Material[] mIDMaterials = null;

		[AssetPathToObject]
		public string numPath = "Assets/Projects/Art/FBX/Textures/Num";

		public override string category { get { return "ProjectConfig"; } }

		[InspectorMethod(aliasName = "ScanMaterials")]
		private void OnEnable()
		{
#if UNITY_EDITOR
			mNumMaterials = EnsureMaterials(mNumMaterials, "{0}/Num{1}.mat", out var changed0);
			mIDMaterials = EnsureMaterials(mIDMaterials, "{0}/ID{1}.mat", out var changed1);

			if (changed0 || changed1)
			{
				EditorUtility.SetDirty(this);
			}
#endif
		}



		public Material[] EnsureMaterials(Material[] mats, string format, out bool changed)
		{
			changed = false;
			if (mats == null || mats.Length == 0 || mats[0] == null)
			{
				mats = new Material[10];
				changed = true;
				for (int num = 0; num < mats.Length; ++num)
				{
					string path = string.Format(format, numPath, num);
#if UNITY_EDITOR
					mats[num] = AssetDatabase.LoadAssetAtPath<Material>(path);
#endif
					if (mats[num] == null)
					{
						DebugUtility.LogError(LoggerTags.Project, "Missing material : {0}", path);
					}
				}
			}
			return mats;
		}

		public void GetIDMaterial(int num, out Material _10, out Material _1)
		{
			_10 = null;
			_1 = null;

			DebugUtility.AssertFormat(num >= 0, "The num is less than zero: {0}", num);
			DebugUtility.AssertFormat(num < mIDMaterials.Length * 10 + mIDMaterials.Length, "The num[{0}] is not less than {1}", num, mIDMaterials.Length * 10 + mIDMaterials.Length);

			if (mIDMaterials.Length == 0)
			{
				DebugUtility.AssertFormat(false, "The ID Materials num is zero");
				return;
			}

			num = Mathf.Clamp(num, 0, mIDMaterials.Length * 10 + mIDMaterials.Length - 1);

			int value1 = num % 10;
			int value10 = (num - value1) / 10;

			if (mIDMaterials.Length > value10)
				_10 = mIDMaterials[value10];
			if (mIDMaterials.Length > value1)
				_1 = mIDMaterials[value1];
		}

		public void GetMaterial(int num, out Material _10, out Material _1)
		{
			_10 = null;
			_1 = null;

			DebugUtility.AssertFormat(num >= 0, "The num is less than zero: {0}", num);
			DebugUtility.AssertFormat(num < mNumMaterials.Length * 10 + mNumMaterials.Length, "The num[{0}] is not less than {1}", num, mNumMaterials.Length * 10 + mNumMaterials.Length);

			if (mNumMaterials.Length == 0)
			{
				DebugUtility.AssertFormat(false, "The ID Materials num is zero");
				return;
			}

			num = Mathf.Clamp(num, 0, mNumMaterials.Length * 10 + mNumMaterials.Length - 1);

			int value1 = num % 10;
			int value10 = (num - value1) / 10;

			if (mIDMaterials.Length > value10)
				_10 = mNumMaterials[value10];
			if (mIDMaterials.Length > value1)
				_1 = mNumMaterials[value1];
		}
	}
}
