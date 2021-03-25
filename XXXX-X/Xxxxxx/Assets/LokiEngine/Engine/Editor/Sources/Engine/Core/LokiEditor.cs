using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using System.Reflection;

namespace Loki
{
	public class LokiEditorGUIUtility
	{
		private static GUILayoutOption[] msDefaultLayoutOptions = null;

		private static GUILayoutOption WidthPercent(float percent)
		{
			return GUILayout.Width(EditorGUIUtility.currentViewWidth * Mathf.Clamp01(percent));
		}

		public static GUILayoutOption[] defaultValueLayoutOptions
		{
			get
			{
				if (msDefaultLayoutOptions == null)
				{
					msDefaultLayoutOptions = new GUILayoutOption[] { WidthPercent(0.6f), };
				}
				return msDefaultLayoutOptions;
			}
		}

		public static GUILayoutOption[] defaultKeyLayoutOptions
		{
			get
			{
				if (msDefaultLayoutOptions == null)
				{
					msDefaultLayoutOptions = new GUILayoutOption[] { WidthPercent(0.4f), };
				}
				return msDefaultLayoutOptions;
			}
		}

		public static readonly Dictionary<Type, Func<Rect, object, object>> EditorGUIFields =
			new Dictionary<Type, Func<Rect, object, object>>()
			{
			{ typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
			{ typeof(long), (rect, value) => EditorGUI.LongField(rect, (long)value) },
			{ typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
			{ typeof(string), (rect, value) => EditorGUI.TextField(rect, (string)value) },
			{ typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
			{ typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
			{ typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
			{ typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
			{ typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
			{ typeof(Color), (rect, value) => EditorGUI.ColorField(rect, (Color)value) },
		};

		public static readonly Dictionary<Type, Func<object, object>> EditorGUILayoutFields =
			new Dictionary<Type, Func<object, object>>()
			{
			{ typeof(int), (value) => EditorGUILayout.IntField((int)value) },
			{ typeof(long), (value) => EditorGUILayout.LongField((long)value) },
			{ typeof(float), (value) => EditorGUILayout.FloatField((float)value) },
			{ typeof(string), (value) => EditorGUILayout.TextField((string)value) },
			{ typeof(bool), (value) => EditorGUILayout.Toggle((bool)value) },
			{ typeof(Vector2), (value) => EditorGUILayout.Vector2Field(GUIContent.none, (Vector2)value) },
			{ typeof(Vector3), (value) => EditorGUILayout.Vector3Field(GUIContent.none, (Vector3)value) },
			{ typeof(Bounds), (value) => EditorGUILayout.BoundsField((Bounds)value) },
			{ typeof(Rect), (value) => EditorGUILayout.RectField((Rect)value) },
			{ typeof(Color), (value) => EditorGUILayout.ColorField((Color)value) },
		};

		public static readonly Dictionary<Type, Func<object, object, object, object>> EditorGUILayoutSliderFields =
			new Dictionary<Type, Func<object, object, object, object>>()
			{
			{ typeof(int), (value, min, max) => EditorGUILayout.IntSlider((int)value, (int)(float)min, (int)(float)max) },
			{ typeof(long), (value, min, max) => EditorGUILayout.LongField((long)value) },
			{ typeof(float), (value, min, max) => EditorGUILayout.Slider((float)value, (float)min, (float)max) },
			{ typeof(string), (value, min, max) => EditorGUILayout.TextField((string)value) },
			{ typeof(bool), (value, min, max) => EditorGUILayout.Toggle((bool)value) },
			{ typeof(Vector2), (value, min, max) => EditorGUILayout.Vector2Field(GUIContent.none, (Vector2)value) },
			{ typeof(Vector3), (value, min, max) => EditorGUILayout.Vector3Field(GUIContent.none, (Vector3)value) },
			{ typeof(Bounds), (value, min, max) => EditorGUILayout.BoundsField((Bounds)value) },
			{ typeof(Rect), (value, min, max) => EditorGUILayout.RectField((Rect)value) },
			{ typeof(Color), (value, min, max) => EditorGUILayout.ColorField((Color)value) },
		};

		public static void Test()
		{

		}

	}


	[CanEditMultipleObjects]
	public abstract class LokiEditor : Editor
	{
		private readonly List<MethodInfo> mInvokableMethods = new List<MethodInfo>();
		private readonly List<string> mAllCategories = new List<string>();
		private readonly Dictionary<string, bool> mCategoryFoldout = new Dictionary<string, bool>();
		private readonly List<KeyValuePair<UnityObject, Editor>> mInjectEditors = new List<KeyValuePair<UnityObject, Editor>>();
		private bool mInjectEditorsFoldout = false;

		private EditorApplication.CallbackFunction mEditorUpdate = null;
		private MethodInfo mOnInspectorUpdate = null;


		public MethodInfo onInspectorUpdate
		{
			get
			{
				if (mOnInspectorUpdate == null)
				{
					var targetType = target.GetType();
					if (targetType.IsSubclassOf(typeof(UObject)))
					{
						mOnInspectorUpdate = GlobalReflectionCache.FindOrAdd(targetType).FindMethods(
							m => m.Name == "OnInspectorUpdate" &&
							m.GetParameters().Length == 0 &&
							m.GetGenericArguments().Length == 0).First();
					}
				}
				return mOnInspectorUpdate;
			}
		}

		public List<MethodInfo> invokableMethods
		{
			get
			{
				if (target == null || mInvokableMethods.Count > 0)
				{
					return mInvokableMethods;
				}
				Type type = target.GetType();
				GlobalReflectionCache.FindOrAdd(type).FindMethods<InspectorMethodAttribute>(mInvokableMethods);
				return mInvokableMethods;
			}
		}

		public List<string> allCategories
		{
			get
			{
				if (mAllCategories.Count == 0)
				{
					mAllCategories.Union(string.Empty);
					foreach (var m in invokableMethods)
					{
						mAllCategories.Union(m.GetCustomAttribute<InspectorMethodAttribute>().category);
					}
				}
				return mAllCategories;
			}
		}

		public static bool IsSupportedEditorGUIObjectField(Type type)
		{
			if (LokiEditorGUIUtility.EditorGUIFields.TryGetValue(type, out var draw))
			{
				return true;
			}
			if (type.IsEnum)
			{
				return true;
			}
			if (typeof(UnityObject).IsAssignableFrom(type))
			{
				return true;
			}
			return false;
		}

		public static bool IsSupportedEditorGUILayoutObjectField(Type type)
		{
			if (LokiEditorGUIUtility.EditorGUILayoutFields.TryGetValue(type, out var draw))
			{
				return true;
			}
			if (type.IsEnum)
			{
				return true;
			}
			if (typeof(UnityObject).IsAssignableFrom(type))
			{
				return true;
			}
			return false;
		}

		public static TMostDerived EditorGUIObjectField<TMostDerived>(object value)
		{
			return (TMostDerived)EditorGUILayoutObjectField(typeof(TMostDerived), value);
		}

		public static T EditorGUILayoutObjectField<T>(Type type, object value)
		{
			return (T)EditorGUILayoutObjectField(type, value);
		}

		public static TMostDerived EditorGUILayoutObjectField<TMostDerived>(object value)
		{
			return (TMostDerived)EditorGUILayoutObjectField(typeof(TMostDerived), value);
		}

		public static T EditorGUIObjectField<T>(Rect rect, Type type, object value)
		{
			return (T)EditorGUIObjectField(rect, type, value);
		}

		public static object EditorGUIObjectField(Rect rect, Type type, object value)
		{
			if (LokiEditorGUIUtility.EditorGUIFields.TryGetValue(type, out var draw))
			{
				return draw(rect, value);
			}
			if (type.IsEnum)
			{
				return (object)EditorGUI.EnumPopup(rect, (Enum)(object)value);
			}

			if (typeof(UnityObject).IsAssignableFrom(type))
			{
				return (object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);
			}

			DebugUtility.LogWarningTrace(LoggerTags.Engine, "Unsupport type : {0}", type.Name);
			return value;
		}

		public static object EditorGUILayoutObjectField(Type type, object value)
		{
			if (LokiEditorGUIUtility.EditorGUILayoutFields.TryGetValue(type, out var draw))
			{
				return draw(value);
			}
			if (type.IsEnum)
			{
				return (object)EditorGUILayout.EnumPopup((Enum)(object)value);
			}

			if (typeof(UnityObject).IsAssignableFrom(type))
			{
				return (object)EditorGUILayout.ObjectField((UnityObject)(object)value, type, true);
			}

			DebugUtility.LogWarningTrace(LoggerTags.Engine, "Unsupport type : {0}", type.Name);
			return value;
		}

		public static object EditorGUILayoutObjectField(Type type, object value, object min, object max)
		{
			if (LokiEditorGUIUtility.EditorGUILayoutSliderFields.TryGetValue(type, out var draw))
			{
				return draw(value, min, max);
			}
			if (type.IsEnum)
			{
				return (object)EditorGUILayout.EnumPopup((Enum)(object)value);
			}

			if (typeof(UnityObject).IsAssignableFrom(type))
			{
				return (object)EditorGUILayout.ObjectField((UnityObject)(object)value, type, true);
			}

			DebugUtility.LogWarningTrace(LoggerTags.Engine, "Unsupport type : {0}", type.Name);
			return value;
		}

		public static void DrawLine(int height, Color color)
		{
			Rect rect = EditorGUILayout.GetControlRect(false, height);
			rect.height = height;
			EditorGUI.DrawRect(rect, color);
		}

		public static void DrawLine(Rect rect, Color color)
		{
			EditorGUI.DrawRect(rect, color);
		}

		public static GUILayoutOption WidthPercent(float percent)
		{
			return GUILayout.Width(EditorGUIUtility.currentViewWidth * Mathf.Clamp01(percent));
		}

		public static void DrawLine(Rect rect)
		{
			EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
		}

		public static void DrawLine(int upSpace, int downSpace, Rect rect, Color color)
		{
			if (upSpace > 0)
				GUILayout.Space(upSpace);

			EditorGUI.DrawRect(rect, color);

			if (downSpace > 0)
				GUILayout.Space(downSpace);
		}

		public static void DrawLine(int height = 1)
		{
			DrawLine(height, new Color(0.5f, 0.5f, 0.5f, 1));
		}

		public static void DrawLine(int upSpace, int downSpace)
		{
			DrawLine(upSpace, 1, downSpace, new Color(0.5f, 0.5f, 0.5f, 1));
		}

		public static void DrawLine(int upSpace, int height, int downSpace, Color color)
		{
			if (upSpace > 0)
				GUILayout.Space(upSpace);

			DrawLine(height, color);

			if (downSpace > 0)
				GUILayout.Space(downSpace);
		}

		public static void RepaintWindow(Type windowType)
		{
			if (windowType == null)
				return;
			var w = EditorWindow.GetWindow(windowType);
			if (w != null)
			{
				w.Repaint();
			}
		}

		public static void OnSceneGUIDrawTitle(Component component)
		{
			EditorHelper.OnSceneGUIDrawTitle(component);
		}

		public static void RepaintWindow(Type assemblyType, string windowTypeString)
		{
			var type = Assembly.GetAssembly(assemblyType).GetType("UnityEditor.SceneHierarchyWindow");
			RepaintWindow(type);
		}

		public Type GetTargetType()
		{
			if (target != null)
			{
				return target.GetType();
			}
			return null;
		}

		protected virtual void OnEnable()
		{
			EditorApplication.update += OnEditorUpdate;
		}

		protected void ClearDirtyInjectedEditors()
		{
			mInjectEditors.ForEach(kv =>
			{
				if (kv.Key == null)
				{
					if (kv.Value != null)
					{
						UnityObject.DestroyImmediate(kv.Value);
					}
				}
			});
			mInjectEditors.RemoveAll(kv => kv.Key == null);
		}

		protected void DrawInjectedInspector(UnityObject injectTarget, string name, bool draw)
		{
			if (injectTarget == null)
				return;

			var resultIdx = mInjectEditors.FindIndex(kv => kv.Key == injectTarget);
			Editor editor = null;
			if (resultIdx >= 0)
			{
				editor = mInjectEditors[resultIdx].Value;
			}
			if (editor == null)
			{
				editor = Editor.CreateEditor(injectTarget);
			}
			if (resultIdx >= 0)
			{
				mInjectEditors[resultIdx] = new KeyValuePair<UnityObject, Editor>(injectTarget, editor);
			}
			else
			{
				mInjectEditors.Add(new KeyValuePair<UnityObject, Editor>(injectTarget, editor));
			}
			if (editor != null && draw)
			{
				Component component = injectTarget as Component;
				if (component == null)
				{
					return;
				}

				if (component.transform == Selection.activeTransform)
				{
					return;
				}

				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(name, WidthPercent(0.1f));
				EditorGUILayout.ObjectField(injectTarget, injectTarget.GetType(), false, WidthPercent(0.8f));
				EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginChangeCheck();
				editor.OnInspectorGUI();
				if (EditorGUI.EndChangeCheck())
				{
					EditorUtility.SetDirty(injectTarget);
				}
			}
		}

		protected void DeclareInjectEditors(List<UnityObject> allInjectObjects)
		{
			for (int i = 0; i < mInjectEditors.Count; ++i)
			{
				var kv = mInjectEditors[i];
				if (kv.Key == null || !allInjectObjects.Contains(kv.Key))
				{
					if (kv.Value != null)
					{
						UnityObject.DestroyImmediate(kv.Value);
					}

					kv = new KeyValuePair<UnityObject, Editor>(null, null);
					mInjectEditors[i] = kv;
				}
			}

			mInjectEditors.RemoveAll(kv => kv.Key == null || kv.Value == null);

			foreach (var o in allInjectObjects)
			{
				DrawInjectedInspector(o, "", false);
			}
		}

		protected void DrawInjectEditors(List<UnityObject> allInjectObjects)
		{
			if (allInjectObjects.Count > 0)
			{
				DrawLine();
				EditorGUILayout.BeginVertical();
				mInjectEditorsFoldout = EditorGUILayout.Foldout(mInjectEditorsFoldout, "Editors", EditorStyles.foldout);
				if (mInjectEditorsFoldout)
				{
					int idx = 0;
					foreach (var drawObejct in allInjectObjects)
					{
						DrawLine();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.Space();
						EditorGUILayout.BeginVertical();
						DrawInjectedInspector(drawObejct, idx.ToString(), true);
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space();
						++idx;
					}
				}
				DrawLine();
				EditorGUILayout.EndVertical();
			}
		}

		protected virtual void OnDisable()
		{
			EditorApplication.update -= OnEditorUpdate;
			SetEditorUpdate(null);

			foreach (var kv in mInjectEditors)
			{
				if (kv.Value != null)
				{
					UnityObject.DestroyImmediate(kv.Value);
				}
			}
			mInjectEditors.Clear();
		}

		protected virtual void OnEditorUpdate()
		{
			ForceInvokeInspectorUpdate();
		}

		protected void ForceInvokeInspectorUpdate()
		{
			try
			{
				if (target == null)
					return;

				if (onInspectorUpdate != null)
				{
					onInspectorUpdate.Invoke(target, null);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
		}

		protected void SetEditorUpdate(EditorApplication.CallbackFunction editorUpdate)
		{
			if (mEditorUpdate != null)
			{
				EditorApplication.update -= mEditorUpdate;
				mEditorUpdate = null;
			}
			if (editorUpdate != null)
			{
				EditorApplication.update += editorUpdate;
				mEditorUpdate = editorUpdate;
			}
		}

		protected virtual bool CanDrawInspectorGUI()
		{
			return !EditorApplication.isCompiling;
		}

		protected void OnDrawInspectorMethods()
		{
			if (allCategories.Count > 0)
			{
				DrawLine(2);
				foreach (var category in allCategories)
				{
					DrawInspectorMethods(category);
				}
			}
		}

		protected static void AutoSerializing(object t)
		{
			Type targetType = t.GetType();
			UObject target = t as UObject;
			if (target == null)
				return;

			var fieldsInfo = GlobalReflectionCache.FindOrAdd(targetType).FindSerializeFields();
			if (fieldsInfo == null)
			{
				return;
			}

			foreach (var field in fieldsInfo)
			{
				bool fieldIsGameObject = (field.FieldType == typeof(GameObject));
				bool fieldIsComponent = (field.FieldType.IsSubclassOf(typeof(Component)));

				string fieldName = field.Name;
				var attr = field.GetCustomAttribute<AutoSerializeFieldAttribute>(true);
				if (attr != null)
				{
					if (!string.IsNullOrEmpty(attr.aliasName))
					{
						fieldName = attr.aliasName;
					}
					else
					{
						string autoRemovePrefix = attr.autoRemovePrefix;
						if (!string.IsNullOrEmpty(autoRemovePrefix))
						{
							if (fieldName.Length > autoRemovePrefix.Length && fieldName.StartsWith(autoRemovePrefix))
							{
								fieldName = fieldName.Substring(autoRemovePrefix.Length);
							}
						}
					}
				}

				if (fieldIsComponent || fieldIsGameObject)
				{
					Transform tr = target.transform.FindUnique(fieldName, StringComparison.OrdinalIgnoreCase);
					if (tr == null)
					{
						if (fieldName.StartsWith("m"))
						{
							tr = target.transform.FindUnique(fieldName.Substring(1), StringComparison.OrdinalIgnoreCase);
						}
					}
					if (tr != null)
					{
						if (fieldIsGameObject)
						{
							field.SetValue(target, tr.gameObject);
						}
						else if (fieldIsComponent)
						{
							var fieldComponent = tr.GetComponent(field.FieldType);
							if (fieldComponent != null)
							{
								field.SetValue(target, fieldComponent);
							}
						}
						else // todo
						{

						}
					}
				}
			}
		}

		protected virtual void OnDrawInspectorPreview()
		{
			if (target == null)
				return;

			Type targetType = target.GetType();
			var memberInfos = GlobalReflectionCache.FindOrAdd(targetType).FindMembers<PreviewMemberBaseAttribute>();
			if (memberInfos == null || memberInfos.Count <= 0)
			{
				return;
			}

			DrawLine();
			foreach (var m in memberInfos)
			{
				PreviewMemberBaseAttribute attr = m.GetCustomAttribute<PreviewMemberBaseAttribute>(true);
				TooltipAttribute ta = m.GetCustomAttribute<TooltipAttribute>(true);
				Type drawType = null;
				object value = null;
				Action<object, object> setValue = null;
				bool readonlyMember = false;

				if (m is FieldInfo)
				{
					var field = (FieldInfo)m;
					value = field.GetValue(target);
					drawType = field.FieldType;
					setValue = field.SetValue;
					readonlyMember = field.IsInitOnly;
				}
				else if (m is PropertyInfo)
				{
					var property = (PropertyInfo)m;
					value = property.GetValue(target);
					drawType = property.PropertyType;
					setValue = property.SetValue;
					readonlyMember = !property.CanWrite;
				}
				else
				{
					continue;
				}

				string tooltip = ta != null ? ta.tooltip : string.Empty;
				if (string.IsNullOrEmpty(tooltip))
				{
					var lta = m.GetCustomAttribute<LokiTooltipAttribute>(true);
					tooltip = lta != null ? lta.tooltip : string.Empty;
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent(m.Name, tooltip), WidthPercent(0.4f));

				EditorGUI.BeginChangeCheck();
				var lastGUIEnable = GUI.enabled;
				GUI.enabled = !readonlyMember;

				if (attr.useRange)
				{
					float rangeMin = 0.0f;
					float rangeMax = 100000.0f;
					if (attr is PreviewMemberAttribute)
					{
						PreviewMemberAttribute real = (PreviewMemberAttribute)attr;
						rangeMin = real.rangeMin;
						rangeMax = real.rangeMax;
					}
					else if (attr is PreviewMemberDynamicPropertyAttribute)
					{
						PreviewMemberDynamicPropertyAttribute real = (PreviewMemberDynamicPropertyAttribute)attr;
						if (drawType == typeof(float) || drawType == typeof(int))
						{
							if (!string.IsNullOrEmpty(real.rangeMin))
								rangeMin = (float)targetType.GetProperty(real.rangeMin).GetValue(target);
							if (!string.IsNullOrEmpty(real.rangeMax))
								rangeMax = (float)targetType.GetProperty(real.rangeMax).GetValue(target);
						}
					}
					else if (attr is PreviewMemberDynamicFieldAttribute)
					{
						PreviewMemberDynamicFieldAttribute real = (PreviewMemberDynamicFieldAttribute)attr;
						if (drawType == typeof(float) || drawType == typeof(int))
						{
							if (!string.IsNullOrEmpty(real.rangeMin))
								rangeMin = (float)targetType.GetField(real.rangeMin).GetValue(target);
							if (!string.IsNullOrEmpty(real.rangeMax))
								rangeMax = (float)targetType.GetField(real.rangeMax).GetValue(target);
						}
					}
					value = EditorGUILayoutObjectField(drawType, value, rangeMin, rangeMax);
				}
				else
				{
					value = EditorGUILayoutObjectField(drawType, value);
				}
				GUI.enabled = lastGUIEnable;
				if (EditorGUI.EndChangeCheck())
				{
					setValue(target, value);
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		protected virtual void OnDrawInspectorGUI()
		{
			base.OnInspectorGUI();
		}

		public sealed override void OnInspectorGUI()
		{
			if (CanDrawInspectorGUI())
			{
				OnDrawInspectorGUI();
				OnDrawInspectorPreview();
				OnDrawInspectorMethods();
				OnDrawInjectInspectorGUI();
			}
		}

		protected virtual void OnDrawInjectInspectorGUI()
		{
			if (mInjectEditors.Count > 0)
			{
				List<UnityObject> toDraw = new List<UnityObject>();
				foreach (var kv in mInjectEditors)
				{
					var drawObejct = kv.Key;
					toDraw.Add(drawObejct);
				}
				DrawInjectEditors(toDraw);
			}
		}

		protected bool IsCategoryFoldout(string category)
		{
			if (mCategoryFoldout.TryGetValue(category, out var foldout))
			{
				return foldout;
			}
			return IsDefaultGroup(category);
		}

		protected void SetCategoryFoldout(string category, bool foldout)
		{
			mCategoryFoldout[category] = foldout;
		}

		protected virtual void OnDrawCustomMethods(string category)
		{
			var component = (target as UObject);
			if (component != null)
			{
				if (IsDefaultGroup(category))
				{
					bool multipleTargets = targets.Length > 1;
					string serializingButtonName = multipleTargets ? "Auto Serializing(M)" : "Auto Serializing";
					if (GUILayout.Button(serializingButtonName, GUILayout.Width(200)))
					{
						AutoSerializing(component);
					}
				}
			}
		}

		private void DrawInvokableMethods(string category)
		{
			foreach (var m in invokableMethods)
			{
				var attr = m.GetCustomAttribute<InspectorMethodAttribute>();
				if (attr.category == category)
				{
					TooltipAttribute ta = m.GetCustomAttribute<TooltipAttribute>(true);
					string tooltip = ta != null ? ta.tooltip : string.Empty;
					if (string.IsNullOrEmpty(tooltip))
					{
						var lta = m.GetCustomAttribute<LokiTooltipAttribute>(true);
						tooltip = lta != null ? lta.tooltip : string.Empty;
					}

					string buttonName = string.IsNullOrEmpty(attr.aliasName) ? m.Name : attr.aliasName;
					if (GUILayout.Button(new GUIContent(buttonName, tooltip), GUILayout.Width(attr.width)))
					{
						if (attr.allowMultipleTargets)
						{
							foreach (var t in targets)
							{
								if (m.IsStatic)
								{
									m.Invoke(null, null);
								}
								else
								{
									m.Invoke(t, null);
								}
							}
						}
						else
						{
							if (m.IsStatic)
							{
								m.Invoke(null, null);
							}
							else
							{
								m.Invoke(target, null);
							}
						}
					}
				}
			}
		}

		protected bool IsDefaultGroup(string category)
		{
			return string.IsNullOrEmpty(category);
		}

		protected bool DrawInspectorMethods(string category)
		{
			if (target == null)
				return false;

			bool isCategoryFoldout = IsCategoryFoldout(category);
			EditorGUI.BeginChangeCheck();

			string groupName = category;
			if (string.IsNullOrEmpty(groupName))
			{
				groupName = "Default Group";
			}

			Rect position = EditorGUILayout.GetControlRect(false, 30);
			isCategoryFoldout = EditorGUI.Foldout(position, isCategoryFoldout, groupName, EditorStyles.foldout);

			if (EditorGUI.EndChangeCheck())
				SetCategoryFoldout(category, isCategoryFoldout);

			if (!isCategoryFoldout)
				return true;

			DrawInvokableMethods(category);
			OnDrawCustomMethods(category);

			return true;
		}

		protected void DrawArrowCaps()
		{
			if (!(target is UObject))
			{
				return;
			}

			UObject t = (UObject)target;

			var attr = GlobalReflectionCache.FindOrAdd(target.GetType()).GetCustomAttribute<DrawArrowCapAttribute>();
			if (attr != null)
			{
				LokiSceneUtility.DrawArrowCap(t.transform);
			}
		}

		protected virtual void OnDrawSceneGUI()
		{
		}
	}

	public abstract class LokiEditor<TMostDerived> : LokiEditor where TMostDerived : LokiEditor<TMostDerived>
	{
		private static TMostDerived msCurrentEditor;

		public static TMostDerived currentEditor
		{
			get
			{
				return msCurrentEditor;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			msCurrentEditor = this as TMostDerived;

		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if (msCurrentEditor == this)
			{
				msCurrentEditor = null;
			}
		}
	}
}
