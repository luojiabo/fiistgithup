using Loki;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

using Object = UnityEngine.Object;

namespace Ubtrobot
{
	/// <summary>
	/// Part contains many part components
	/// </summary>
	[NameToType]
	public sealed class Part : Actor, IPart
	{
		private readonly List<IPartComponent> mPartComponentsCache = new List<IPartComponent>();
		private readonly List<ICommandHandler> mCommandHandlers = new List<ICommandHandler>();
		private readonly List<Renderer> mAllRenderers = new List<Renderer>();
		private MaterialPropertyBlock mPropertyBlock;

		private bool mIsKinematicReady = false;
		private bool mBoundsIsDirty = true;
		private Bounds mBounds;
		private Group mOwnerGroup = null;

		[SerializeField, HideInInspector]
		private float mDelayInit = 0.1f;

		[SerializeField, HideInInspector]
		private Transform mBoundsRange = null;

		protected override bool initializeOnAwake => false;

		public string addressName
		{
			get
			{
				return GetAddressName();
			}
		}

		[PreviewMember]
		public float delayInit
		{
			get
			{
				return mDelayInit;
			}
			set
			{
				mDelayInit = value;
			}
		}

		public bool isKinematicReady
		{
			get
			{
				return mIsKinematicReady;
			}
		}

		[PreviewMember]
		public Transform boundsRange
		{
			get
			{
				if (mBoundsRange == null)
				{
					return transform;
				}
				return mBoundsRange;
			}
			set
			{
				// Don't set the default value
				if (value == transform)
				{
					value = null;
				}
				if (mBoundsRange != value)
				{
					mBoundsIsDirty = true;
					mBoundsRange = value;
				}
			}
		}

		[PreviewMember]
		public ECommandType commandType { get; set; } = ECommandType.Unicast;

		public Bounds bounds
		{
			get
			{
				if (mBoundsIsDirty)
				{
					mBounds = boundsRange.CalcRenerersBounds();
					mBoundsIsDirty = false;
				}
				return mBounds;
			}
		}

		public List<IPartComponent> partsComponents
		{
			get
			{
				return mPartComponentsCache;
			}
		}

		public List<ICommandHandler> commandHandlers
		{
			get
			{
				return mCommandHandlers;
			}
		}

		public IGroup owner
		{
			get
			{
				return GetGroup();
			}
		}

		public IRobot robot
		{
			get
			{
				if (owner != null)
				{
					return owner.robot;
				}
				return null;
			}
		}

		public void Rewind()
		{
			foreach (var com in mPartComponentsCache)
			{
				com.Rewind();
			}
		}

		public Group GetGroup()
		{
			if (mOwnerGroup == null)
			{
				mOwnerGroup = this.GetComponentInParent<Group>(false);
			}
			return mOwnerGroup;
		}

		public string ExportName(int nameID)
		{
			return string.Concat(GetAddressName(), "@", nameID.ToString());
		}

		public string GetAddressName()
		{
			string aName = name;
			if (string.IsNullOrEmpty(aName))
				return string.Empty;

			var idxOfID = aName.LastIndexOf('@');
			if (idxOfID >= 0)
				return aName.Substring(0, idxOfID);
			return aName;
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			ForceUpdate();
		}

		[InspectorMethod]
		public void MakeBoundsDirty()
		{
			mBoundsIsDirty = true;
		}

#if UNITY_EDITOR
		[ContextMenu("SelectOwnerGroup")]
		[InspectorMethod]
		public void SelectOwnerGroup()
		{
			var group = GetGroup();
			if (group != null)
			{
				UnityEditor.Selection.activeObject = group.gameObject;
			}
		}

#endif

		public void ForceUpdate()
		{
			mPartComponentsCache.Clear();
			mCommandHandlers.Clear();

			transform.GetComponentsInChildren<PartComponent, PartsGroup, IPartComponent>(true, mPartComponentsCache);
			foreach (var component in mPartComponentsCache)
			{
				if (component is ICommandHandler)
				{
					mCommandHandlers.Add((ICommandHandler)component);
				}
			}

			mAllRenderers.Clear();
			transform.GetComponentsInChildren(true, mAllRenderers);
		}

		public static void SetGlobalLightIntensity(bool unlit)
		{
			MaterialManager.SetGlobalLightIntensity(unlit);
		}

		/// <summary>
		/// 对于循环设置，一般在循环外部使用SetGlobalLightIntensity，然后定点对象使用SetHighlight，
		/// </summary>
		[InspectorMethod(allowMultipleTargets = true)]
		public void EnableHighlight()
		{
			SetGlobalLightIntensity(true);
			SetHighlight(true);
		}

		/// <summary>
		/// 对于循环设置，一般在循环外部使用SetGlobalLightIntensity，然后定点对象使用SetHighlight，
		/// </summary>
		[InspectorMethod(allowMultipleTargets = true)]
		public void DisableHighlight()
		{
			SetGlobalLightIntensity(false);
			SetHighlight(false);
		}

		public void SetHighlight(bool flag)
		{
			foreach (var renderer in mAllRenderers)
			{
				if (mPropertyBlock == null)
					mPropertyBlock = new MaterialPropertyBlock();
				renderer.GetPropertyBlock(mPropertyBlock);
				if (flag)
				{
					mPropertyBlock.SetFloat(ShaderIDs.HighlightIntensity, MaterialManager.kUnlitLightIntensity);
				}
				else
				{
					mPropertyBlock.SetFloat(ShaderIDs.HighlightIntensity, 0.0f);
				}
				renderer.SetPropertyBlock(mPropertyBlock);
			}
		}

		[InspectorMethod(allowMultipleTargets = true)]
		public void EnableOutline()
		{
			UseOutline(true, 0);
		}

		[InspectorMethod(allowMultipleTargets = true)]
		public void DisableOutline()
		{
			UseOutline(false, 0);
		}

		public void UseOutline(bool outline, int colorIndex)
		{
			foreach (var renderer in mAllRenderers)
			{
				if (outline)
				{
					var outlineComponent = renderer.gameObject.GetOrAllocComponent<Outline>();
					outlineComponent.color = colorIndex;
					outlineComponent.enabled = true;
				}
				else
				{
					var outlineComponent = renderer.gameObject.GetComponent<Outline>();
					if (outlineComponent != null)
					{
						outlineComponent.enabled = false;
					}
				}
			}
		}

		public bool Verify(ICommand command)
		{
			return mCommandHandlers.Count > 0 && (commandType != ECommandType.Disable);
		}

		public ICommandResponseAsync Execute(ICommand command)
		{
			bool enableBroadcast = commandType == ECommandType.Broadcast;
			ICommandResponseAsync job = null;
			for (var i = 0; i < mCommandHandlers.Count; ++i)
			{
				var h = mCommandHandlers[i];
				if (h.Verify(command))
				{
					DebugUtility.Log(LoggerTags.Online, "Try to execute command in component  {0} - {1}", h, command);

					var result = h.Execute(command);
					if (result != null)
					{
						if (job == null)
						{
							job = new CommandResponseAsync();
						}
						job.AddResponse(result);
						// 不是广播模式，立即停止
						if (!enableBroadcast)
						{
							break;
						}
					}
				}
				else
				{
					DebugUtility.Log(LoggerTags.Online, "Fail to verify command in component  {0} - {1}", h, command);
				}
			}
			return job;
		}

		public void Stop()
		{
			foreach (var commandHandler in mCommandHandlers)
			{
				commandHandler.Stop();
			}
		}

		protected override void Awake()
		{
			base.Awake();
		}

		private IEnumerator Start()
		{
			if (mDelayInit > 0.0f)
			{
				yield return new WaitForSeconds(mDelayInit);
			}
			else
			{
				yield return null;
			}

			mIsKinematicReady = true;
			// DebugUtility.Log(LoggerTags.Project, "KinematicReady, Frame ({0})", Time.frameCount);
		}

		public void OnPartOverlap(IPart other)
		{
			var robot = this.robot;
			if (robot == null)
				return;

			if (!this.isKinematicReady)
			{
				robot.physicsSystem.AddCollider(this, other);
			}
			else
			{
				robot.physicsSystem.AddMotionCollider(this, other);
			}
		}

		public void OnPartSeparation(IPart other)
		{
			var robot = this.robot;
			if (robot == null)
				return;

			if (this.isKinematicReady)
			{
				robot.physicsSystem.RemoveMotionCollider(this, other);
			}
		}


#if UNITY_EDITOR
		private void OnEditorUpdate()
		{
			CheckPhysComponent();
		}

		public void OnInspectorUpdate()
		{
			ForceUpdate();

			for (var i = mPartComponentsCache.Count - 1; i >= 0; --i)
			{
				if (mPartComponentsCache[i] != null)
				{
					mPartComponentsCache[i].OnInspectorUpdate();
				}
			}
		}

		[InspectorMethod]
		private void AddPhysComponent()
		{
			gameObject.GetOrAllocComponent<PartPhysicsComponent>();
		}

		[InspectorMethod]
		private void CheckPhysComponent()
		{
			if (GetComponent<Collider>())
			{
				AddPhysComponent();
			}
		}

		[InspectorMethod]
		private void ClearPhysComponent()
		{
			var collider = GetComponent<Collider>();
			var rigidbody = GetComponent<Rigidbody>();

			if (collider != null)
				DestroyImmediate(collider, true);
			if (rigidbody != null)
				DestroyImmediate(rigidbody, true);

			ForceUpdate();
			for (var i = mPartComponentsCache.Count - 1; i >= 0; --i)
			{
				if (mPartComponentsCache[i] != null && mPartComponentsCache[i] is PartPhysicsComponent)
				{
					((PartPhysicsComponent)mPartComponentsCache[i]).OnClearPhysicsFeature();
					DestroyImmediate(((PartPhysicsComponent)mPartComponentsCache[i]), true);
				}
			}
			ForceUpdate();
		}

		[InspectorMethod]
		public void SetAsPivot()
		{
			Group group = GetComponentInParent<Group>();
			List<Transform> children = new List<Transform>();
			for (int i = 0; i < group.transform.childCount; ++i)
			{
				children.Add(group.transform.GetChild(i));
			}
			group.transform.DetachChildren();
			group.transform.CopyFrom(transform, Space.World);
			foreach (var tr in children)
			{
				tr.transform.SetParent(group.transform, true);
			}
		}

#endif
	}
}
