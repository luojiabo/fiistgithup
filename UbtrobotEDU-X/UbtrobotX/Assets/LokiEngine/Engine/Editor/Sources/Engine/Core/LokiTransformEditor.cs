using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Loki
{
	[CanEditMultipleObjects, CustomEditor(typeof(Transform))]
	public sealed class LokiTransformEditor : LokiEditor<LokiTransformEditor>
	{
		/// <summary>
		/// controls the width of the input fields
		/// </summary>
		private const float FIELD_WIDTH = 250;
		private const float POSITION_MAX = 100000.0f;

		//private static GUIContent positionGUIContent = new GUIContent(LocalString("Position"), LocalString("The local position of this Game Object relative to the parent."));
		private static readonly GUIContent msRotationGUIContent = new GUIContent(LocalString("Rotation"), LocalString("The local rotation of this Game Object relative to the parent."));
		//private static GUIContent scaleGUIContent = new GUIContent(LocalString("Scale"), LocalString("The local scaling of this Game Object relative to the parent."));

		private static readonly string msPositionWarningText = LocalString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");

		public static bool uniformScaling = false; //Are we using uniform scaling mode?

		private static bool msShowUtilities = false; //Should we show the utilities section?

		private SerializedProperty mPositionProperty; //The position of this transform
		private SerializedProperty mRotationProperty; //The rotation of this transform
		private SerializedProperty mScaleProperty; //The scale of this transform

		#region INITIALISATION

		#region UTILITIES
		private static float msSnapOffset = 0f;
		private static Vector3 msMinRotation;
		private static Vector3 msMaxRotation = new Vector3(360, 360, 360);
		#endregion

		//References to some images for our GUI
		private static Texture2D iconRevert { get { return LokiEditorStyles.iconRevert; } }
		private static Texture2D iconLocked { get { return LokiEditorStyles.iconLocked; } }
		private static Texture2D iconUnlocked { get { return LokiEditorStyles.iconUnlocked; } }

		private static bool thinInspectorMode
		{

			get
			{
				return EditorGUIUtility.currentViewWidth <= 400;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			this.mPositionProperty = this.serializedObject.FindProperty("m_LocalPosition");
			this.mRotationProperty = this.serializedObject.FindProperty("m_LocalRotation");
			this.mScaleProperty = this.serializedObject.FindProperty("m_LocalScale");
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnEditorUpdate()
		{
			base.OnEditorUpdate();
			Repaint();
		}
		#endregion

		/// <summary>
		/// Draws the inspector
		/// </summary>
		protected override void OnDrawInspectorGUI()
		{
			//base.OnDrawInspectorGUI();

			base.serializedObject.Update();
			//Draw the inputs
			DrawPositionElement();
			DrawRotationElement();
			DrawScaleElement();
			DrawChildrenInfo();

			//Draw the Utilities
			msShowUtilities = LokiEditorStyles.DrawHeader("Transform Utilities");
			if (msShowUtilities)
				DrawUtilities();
			//Validate the transform of this object
			if (!ValidatePosition(((Transform)this.target).position))
			{
				EditorGUILayout.HelpBox(msPositionWarningText, MessageType.Warning);
			}
			//Apply the settings to the object
			this.serializedObject.ApplyModifiedProperties();
			EditorGUIUtility.labelWidth = 0;
		}

		/// <summary>
		/// Draws the input for the position
		/// </summary>
		private void DrawPositionElement()
		{
			if (thinInspectorMode)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Position");
				DrawPositionReset();
				GUILayout.EndHorizontal();
			}

			string label = thinInspectorMode ? "" : "Position";

			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - LokiTransformEditor.FIELD_WIDTH - 64; // align field to right of inspector
			this.mPositionProperty.vector3Value = LokiEditorStyles.Vector3InputField(label, this.mPositionProperty.vector3Value);
			if (!thinInspectorMode)
				DrawPositionReset();
			GUILayout.EndHorizontal();
			EditorGUIUtility.labelWidth = 0;
		}

		private void DrawPositionReset()
		{
			GUILayout.Space(18);
			if (GUILayout.Button(new GUIContent("", iconRevert, "Reset this objects position"), LokiEditorStyles.uEditorSkin.GetStyle("ResetButton"), GUILayout.Width(18), GUILayout.Height(18)))
			{
				Vector3 pasteValue = Vector3.zero;
				if (Event.current.control)
				{
					Clipboard.Read(out pasteValue);
				}

				if (Event.current.alt)
				{
					var copyValue = this.mPositionProperty.vector3Value;
					Clipboard.Write(copyValue);
				}

				this.mPositionProperty.vector3Value = pasteValue;
			}
		}

		/// <summary>
		/// Draws the input for the rotation
		/// </summary>
		private void DrawRotationElement()
		{
			if (thinInspectorMode)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Rotation");
				DrawRotationReset();
				GUILayout.EndHorizontal();
			}

			//Rotation layout
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - LokiTransformEditor.FIELD_WIDTH - 64; // align field to right of inspector
			this.RotationPropertyField(this.mRotationProperty, thinInspectorMode ? GUIContent.none : msRotationGUIContent);
			if (!thinInspectorMode)
				DrawRotationReset();
			GUILayout.EndHorizontal();
			EditorGUIUtility.labelWidth = 0;
		}
		private void DrawRotationReset()
		{
			GUILayout.Space(18);
			if (GUILayout.Button(new GUIContent("", iconRevert, "Reset this objects rotation"), LokiEditorStyles.uEditorSkin.GetStyle("ResetButton"), GUILayout.Width(18), GUILayout.Height(18)))
			{
				Quaternion pasteValue = Quaternion.identity;
				if (Event.current.control)
				{
					Clipboard.Read(out pasteValue);
				}

				if (Event.current.alt)
				{
					var copyValue = this.mRotationProperty.quaternionValue;
					Clipboard.Write(copyValue);
				}

				this.mRotationProperty.quaternionValue = pasteValue;
			}
		}

		/// <summary>
		/// Draws the input for the scale
		/// </summary>
		private void DrawScaleElement()
		{
			if (thinInspectorMode)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Scale");
				DrawScaleReset();
				GUILayout.EndHorizontal();
			}
			string label = thinInspectorMode ? "" : "Scale";

			//Scale Layout
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - LokiTransformEditor.FIELD_WIDTH - 64; // align field to right of inspector
			this.mScaleProperty.vector3Value = LokiEditorStyles.Vector3InputField(label, this.mScaleProperty.vector3Value, false, uniformScaling, uniformScaling);
			if (!thinInspectorMode)
				DrawScaleReset();
			GUILayout.EndHorizontal();
			EditorGUIUtility.labelWidth = 0;
		}
		private void DrawScaleReset()
		{
			if (GUILayout.Button(new GUIContent("", (uniformScaling ? iconLocked : iconUnlocked), (uniformScaling ? "Unlock Scale" : "Lock Scale")), LokiEditorStyles.uEditorSkin.GetStyle("ResetButton"), GUILayout.Width(18), GUILayout.Height(18)))
			{
				uniformScaling = !uniformScaling;
			}
			if (GUILayout.Button(new GUIContent("", iconRevert, "Reset this objects scale"), LokiEditorStyles.uEditorSkin.GetStyle("ResetButton"), GUILayout.Width(18), GUILayout.Height(18)))
			{
				Vector3 pasteValue = Vector3.one;
				if (Event.current.control)
				{
					Clipboard.Read(out pasteValue);
				}

				if (Event.current.alt)
				{
					var copyValue = this.mScaleProperty.vector3Value;
					Clipboard.Write(copyValue);
				}

				this.mScaleProperty.vector3Value = pasteValue;
			}
		}

		#region UTILITIES
		private void DrawUtilities()
		{
			GUILayout.Space(5);
			//Snap to ground
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Snap To Ground", LokiEditorStyles.uEditorSkin.button, GUILayout.Width(thinInspectorMode ? 100 : 160)))
			{
				foreach (var tar in this.targets)
				{
					Transform t = (Transform)tar;
					Undo.RecordObject(t, "Snap to Ground");
					t.TransformSnapToGround(msSnapOffset);
				}
			}
			EditorGUIUtility.labelWidth = 50f;
			msSnapOffset = EditorGUILayout.FloatField("Offset", msSnapOffset);
			GUILayout.EndHorizontal();


			//Random rotation
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Random Rotation", LokiEditorStyles.uEditorSkin.button, GUILayout.Width(thinInspectorMode ? 100 : 160), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
			{
				foreach (var tar in this.targets)
				{
					Transform t = (Transform)tar;
					Undo.RecordObject(t, "Random Rotation");
					t.RandomiseRotation(msMinRotation, msMaxRotation);
				}
			}

			GUILayout.BeginVertical();
			msMinRotation = EditorGUILayout.Vector3Field(thinInspectorMode ? "" : "Min", msMinRotation);
			msMaxRotation = EditorGUILayout.Vector3Field(thinInspectorMode ? "" : "Max", msMaxRotation);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			EditorGUIUtility.labelWidth = 0;

		}
		#endregion

		/// <summary>
		/// Returns the localised version of a string
		/// </summary>
		private static string LocalString(string text)
		{
			return text;
			//return LocalizationDatabase.GetLocalizedString(text);
		}


		private bool ValidatePosition(Vector3 position)
		{
			if (Mathf.Abs(position.x) > LokiTransformEditor.POSITION_MAX) return false;
			if (Mathf.Abs(position.y) > LokiTransformEditor.POSITION_MAX) return false;
			if (Mathf.Abs(position.z) > LokiTransformEditor.POSITION_MAX) return false;
			return true;
		}

		private void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content)
		{
			Transform transform = (Transform)this.targets[0];
			Quaternion localRotation = transform.localRotation;
			foreach (UnityEngine.Object t in (UnityEngine.Object[])this.targets)
			{
				if (!SameRotation(localRotation, ((Transform)t).localRotation))
				{
					EditorGUI.showMixedValue = true;
					break;
				}
			}

			EditorGUI.BeginChangeCheck();

			Vector3 eulerAngles = LokiEditorStyles.Vector3InputField(content.text, localRotation.eulerAngles);
			//Vector3 eulerAngles = EditorGUILayout.Vector3Field(content, localRotation.eulerAngles);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObjects(this.targets, "Rotation Changed");
				foreach (UnityEngine.Object obj in this.targets)
				{
					Transform t = (Transform)obj;
					t.localEulerAngles = eulerAngles;
				}
				rotationProperty.serializedObject.SetIsDifferentCacheDirty();
			}

			EditorGUI.showMixedValue = false;
		}

		private bool SameRotation(Quaternion rot1, Quaternion rot2)
		{
			if (rot1.x != rot2.x) return false;
			if (rot1.y != rot2.y) return false;
			if (rot1.z != rot2.z) return false;
			if (rot1.w != rot2.w) return false;
			return true;
		}

		private void DrawChildrenInfo()
		{
			Transform transform = (Transform)this.targets[0];
			GUILayout.BeginVertical();
			DrawLine(10, 2);
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("--Child Count");
			GUILayout.Space(18);
			EditorGUILayout.LabelField(transform.childCount.ToString(), GUILayout.Width(50), GUILayout.Height(18));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("--Max Depth");
			GUILayout.Space(18);
			EditorGUILayout.LabelField(transform.GetDepth().ToString(), GUILayout.Width(50), GUILayout.Height(18));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(transform.GetHierarchyPath());
			GUILayout.EndHorizontal();
			DrawLine(2, 10);
			GUILayout.EndVertical();
		}

	}
}
