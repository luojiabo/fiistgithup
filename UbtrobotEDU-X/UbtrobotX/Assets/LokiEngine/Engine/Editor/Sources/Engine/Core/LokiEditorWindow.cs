using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loki
{
	public abstract class LokiEditorWindow : EditorWindow
	{
		public abstract string assetName { get; }

		protected abstract string GetAssetPath(string fileName);

		protected void LoadWindowAssets(string windowName)
		{
			// Each editor window contains a root VisualElement object
			VisualElement root = rootVisualElement;

			// A stylesheet can be added to a VisualElement.
			// The style will be applied to the VisualElement and all of its children.
			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(GetAssetPath(windowName + ".uss"));

			// Import UXMLS
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(GetAssetPath(windowName + ".uxml"));
			VisualElement uxml = visualTree.CloneTree();
			root.Add(uxml);
			uxml.styleSheets.Add(styleSheet);
		}
	}

	public abstract class LokiEditorWindow<TMostDerived> : LokiEditorWindow where TMostDerived : LokiEditorWindow<TMostDerived>
	{
		protected virtual void OnEnable()
		{
			LoadWindowLayout();
		}

		protected virtual void LoadWindowLayout()
		{
			LoadWindowAssets(assetName);
		}


	}
}

