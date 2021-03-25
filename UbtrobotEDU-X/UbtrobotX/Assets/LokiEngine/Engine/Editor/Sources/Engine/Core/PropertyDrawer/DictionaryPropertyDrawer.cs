using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Loki
{
	public abstract class DictionaryPropertyDrawer<TK, TV> : PropertyDrawer
	{
		private const float kButtonWidth = 18f;
		private const float kToggleWidth = 50f;
		private const float kLabelWidth = 30f;

		private static readonly SerializableDictionary<Type, Func<Rect, object, object>> msFields =
			new SerializableDictionary<Type, Func<Rect, object, object>>()
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
		};

		private readonly SerializableDictionary<TK, bool> mDictionaryFoldout = new SerializableDictionary<TK, bool>();
		private SerializableDictionary<TK, TV> mDictionary;
		private bool mFoldout = false;
		private bool mAutoIncreaseEnum = true;

		private bool IsTKFoldout(TK key)
		{
			if (mDictionaryFoldout.TryGetValue(key, out var result))
			{
				return result;
			}
			return false;
		}

		private void SetTKFoldout(TK key, bool foldout)
		{
			mDictionaryFoldout[key] = foldout;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			CheckInitialize(property, label);
			if (mFoldout)
			{
				float height = (mDictionary.Count + 1) * 19f;
				if (typeof(System.Collections.IList).IsAssignableFrom(typeof(TV)))
				{
					foreach (var kv in mDictionary)
					{
						if (kv.Value != null && IsTKFoldout(kv.Key))
						{
							height += (((System.Collections.IList)kv.Value).Count + 1) * 17.0f;
						}
						else
						{
							height += 17.0f;
						}
					}
				}
				return height;
			}
			return 19f;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			CheckInitialize(property, label);

			position.height = 17f;

			var foldoutRect = position;
			foldoutRect.width -= 2 * kButtonWidth;
			EditorGUI.BeginChangeCheck();
			mFoldout = EditorGUI.Foldout(foldoutRect, mFoldout, label, true);
			if (EditorGUI.EndChangeCheck())
				EditorPrefs.SetBool(label.text, mFoldout);

			var buttonRect = position;
			buttonRect.x = position.width - kButtonWidth + position.x;
			buttonRect.width = kButtonWidth + 2;

			if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
			{
				if (AddNewItem(mAutoIncreaseEnum))
				{
					mFoldout = true;
				}
			}

			buttonRect.x -= kButtonWidth;

			if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonRight))
			{
				ClearDictionary();
			}

			//var toggleRect = buttonRect;
			//toggleRect.x -= kToggleWidth;
			//toggleRect.width = kToggleWidth;
			//mAutoIncreaseEnum = GUI.Toggle(toggleRect, mAutoIncreaseEnum, "Auto");

			Rect dictCountRect = buttonRect;
			dictCountRect.width = kButtonWidth;
			dictCountRect.x = buttonRect.xMax + 2 - kButtonWidth * 2 - kLabelWidth;

			EditorGUI.LabelField(dictCountRect, mDictionary.Count.ToString());

			if (!mFoldout)
				return;

			Rect lineRect = position;
			lineRect.height = 1;

			foreach (var item in mDictionary)
			{
				var key = item.Key;
				var value = item.Value;

				lineRect.y = position.y;
				LokiEditor.DrawLine(lineRect);

				position.y += 17f;

				foldoutRect = position;
				foldoutRect.width = kButtonWidth;
				foldoutRect.x += kButtonWidth;

				var keyRect = position;
				keyRect.width /= 2;
				keyRect.width -= 4;
				keyRect.x += kButtonWidth * 2;

				EditorGUI.BeginChangeCheck();
				bool isTKFoldout = IsTKFoldout(key);
				isTKFoldout = EditorGUI.Foldout(foldoutRect, isTKFoldout, "", true);
				if (EditorGUI.EndChangeCheck())
				{
					SetTKFoldout(key, isTKFoldout);
				}

				EditorGUI.BeginChangeCheck();
				var newKey = DoField(ref position, ref keyRect, typeof(TK), key);
				if (EditorGUI.EndChangeCheck())
				{
					if (!mDictionary.ContainsKey(newKey))
					{
						mDictionary.Remove(key);
						mDictionary.Add(newKey, value);
					}
					break;
				}

				var valueRect = position;
				valueRect.x = position.width / 2 + 15;
				valueRect.width = keyRect.width - kButtonWidth;

				var removeRect = valueRect;
				removeRect.x = valueRect.xMax + 2 - kButtonWidth;
				removeRect.width = kButtonWidth;
				if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
				{
					RemoveItem(key);
					break;
				}
				EditorGUI.BeginChangeCheck();

				if (typeof(System.Collections.IList).IsAssignableFrom(typeof(TV)))
				{
					value = DoListField(ref position, ref valueRect, typeof(TV), key, value);
				}
				else
				{
					value = DoField(ref position, ref valueRect, typeof(TV), value);
				}

				if (EditorGUI.EndChangeCheck())
				{
					mDictionary[key] = value;
					break;
				}

				lineRect.y = position.y;
				LokiEditor.DrawLine(lineRect);
			}
		}

		private void RemoveItem(TK key)
		{
			mDictionary.Remove(key);
		}

		private void CheckInitialize(SerializedProperty property, GUIContent label)
		{
			if (mDictionary == null)
			{
				var target = property.serializedObject.targetObject;
				mDictionary = fieldInfo.GetValue(target) as SerializableDictionary<TK, TV>;
				if (mDictionary == null)
				{
					mDictionary = new SerializableDictionary<TK, TV>();
					fieldInfo.SetValue(target, mDictionary);
				}
				mFoldout = EditorPrefs.GetBool(label.text);
			}
		}

		private T DoListField<T>(ref Rect position, ref Rect rect, Type type, TK key, T value)
		{
			if (value == null)
			{
				value = Misc.Default<T>(type);
			}

			if (typeof(System.Collections.IList).IsAssignableFrom(type))
			{
				var list = (System.Collections.IList)value;

				Rect listCountRect = rect;
				listCountRect.width = kButtonWidth;
				listCountRect.x = rect.xMax + 6 - kButtonWidth * 2 - kLabelWidth;

				EditorGUI.LabelField(listCountRect, list.Count.ToString());

				Type elementType = null;
				var attri = type.GetCustomAttribute<CollectionAttribute>(true);
				if (attri != null)
				{
					elementType = attri.valueType;
				}
				else if (type.IsGenericType)
				{
					elementType = type.GetGenericArguments()[0];
				}

				if (elementType!=null)
				{
					Rect buttonRect = rect;
					buttonRect.width = kButtonWidth;
					buttonRect.x = rect.xMax + 2;
					if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
					{
						list.Add(Misc.Default(elementType));
						SetTKFoldout(key, true);
					}
				}
				position.y += 17;

				if (IsTKFoldout(key))
				{
					rect.width -= kButtonWidth * 2.0f;
					int count = list.Count;
					for (int i = 0; i < count; ++i)
					{
						rect.y = position.y;

						var removeRect = rect;
						removeRect.y = rect.y;
						removeRect.width = kButtonWidth;
						removeRect.x = rect.xMax + 2 + kButtonWidth;
						if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
						{
							list.RemoveAt(i);
							break;
						}
						//rect.width -= kButtonWidth;
						list[i] = DoField(ref position, ref rect, list[i].GetType(), list[i]);
						position.y += 17;
					}
				}
			}

			return value;
		}

		private T DoField<T>(ref Rect position, ref Rect rect, Type type, T value)
		{
			if (value == null)
			{
				value = Misc.Default<T>(type);
			}

			if (LokiEditor.IsSupportedEditorGUIObjectField(type))
			{
				return LokiEditor.EditorGUIObjectField<T>(rect, type, value);
			}

			DebugUtility.LogWarningTrace(LoggerTags.Engine, "Type is not supported: " + type);
			return value;
		}

		private void ClearDictionary()
		{
			mDictionary.Clear();
		}

		private bool AddNewItem(bool autoIncreaseEnum)
		{
			Type typeTK = typeof(TK);
			TK key = Misc.Default<TK>();
			if (autoIncreaseEnum && typeTK.IsEnum)
			{
				if (mDictionary.ContainsKey(key))
				{
					var values = EnumUtilityNoObsoleted<TK>.GetValues();
					foreach (var v in values)
					{
						if (!mDictionary.ContainsKey(v))
						{
							key = v;
							break;
						}
					}
				}
			}

			//TK key = Misc.Default<TK>();
			if (mDictionary.ContainsKey(key))
				return false;
			mDictionary.Add(key, Misc.Default<TV>());
			return true;
		}
	}

}
