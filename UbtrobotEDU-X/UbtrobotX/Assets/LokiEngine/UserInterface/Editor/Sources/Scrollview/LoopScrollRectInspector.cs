using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace Loki.UI
{

	[CustomEditor(typeof(LoopScrollRect), true)]
	public class LoopScrollRectInspector : Editor
	{
		private int mIndex = 0;
		private float mScrollSpeed = 1000;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();

			LoopScrollRect scroll = (LoopScrollRect)target;
			GUI.enabled = Application.isPlaying;

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Clear"))
			{
				scroll.ClearCells();
			}
			if (GUILayout.Button("Refresh"))
			{
				scroll.RefreshCells();
			}
			if (GUILayout.Button("Refill"))
			{
				scroll.RefillCells();
			}
			if (GUILayout.Button("RefillFromEnd"))
			{
				scroll.RefillCellsFromEnd();
			}
			EditorGUILayout.EndHorizontal();

			EditorGUIUtility.labelWidth = 45;
			float w = (EditorGUIUtility.currentViewWidth - 100) / 2;
			EditorGUILayout.BeginHorizontal();
			mIndex = EditorGUILayout.IntField("Index", mIndex, GUILayout.Width(w));
			mScrollSpeed = EditorGUILayout.FloatField("Speed", mScrollSpeed, GUILayout.Width(w));
			if (GUILayout.Button("Scroll", GUILayout.Width(45)))
			{
				scroll.SrollToCell(mIndex, mScrollSpeed);
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
