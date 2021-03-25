using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public abstract class CameraController : UComponent, ICameraController
	{
		[SerializeField]
		private Material mLineMaterial;

		[SerializeField, HideInInspector]
		protected int mPostProcessingLayerMask = 1;

		private Camera mCamera;

		string ICameraController.name { get { return base.name; } set { base.name = value; } }

		public virtual Transform viewTarget { get; }


		public virtual bool active
		{
			get
			{
				return gameObject.activeSelf;
			}
			set
			{
				if (gameObject.activeSelf != value)
				{
					gameObject.SetActive(value);
				}
			}
		}

		[PreviewMember]
		public int postProcessingLayerMask { get => mPostProcessingLayerMask; set => mPostProcessingLayerMask = value; }

		[PreviewMember]
		[ConsoleProperty(aliasName = "r.enableWireframe")]
		public bool enableWireframe { get; set; } = false;

		public LineRenderer lineRenderer { get; private set; } = new LineRenderer();

		public Camera controlled
		{
			get
			{
				if (mCamera == null)
				{
					mCamera = GetComponent<Camera>();
				}
				return mCamera;
			}
		}

		protected virtual void OnEnable()
		{
			//DebugUtility.LogTrace(LoggerTags.Engine, "CameraController : {0} OnEnable", name);
		}

		protected virtual void OnDisable()
		{
			//DebugUtility.LogTrace(LoggerTags.Engine, "CameraController : {0} OnDisable", name);
		}

		protected override void Awake()
		{
			base.Awake();
			//DebugUtility.Log(LoggerTags.Project, "CameraController Frame : {0}", Time.frameCount);

			CameraSystem system = ModuleManager.Get().GetSystemChecked<CameraSystem>();
			if (system != null)
			{
				system.AddController(this, enabled);
			}
		}

		protected override void OnDestroy()
		{
			//DebugUtility.Log(LoggerTags.Project, "CameraController OnDestroy : {0}", name);
			base.OnDestroy();
			CameraSystem system = ModuleManager.Get().GetSystemChecked<CameraSystem>();
			if (system != null)
			{
				system.RemoveController(this);
			}
		}

		private void OnPreRender()
		{
			if (enableWireframe)
			{
				GL.wireframe = true;
			}
		}

		private void OnPostRender()
		{
			if (enableWireframe)
			{
				GL.wireframe = false;
			}

			if (mLineMaterial == null)
			{
				mLineMaterial = new Material(Shader.Find("Loki/Lines"));
			}
			lineRenderer.lineMaterial = mLineMaterial;
			lineRenderer.Drawcall();
		}

		public Ray ScreenPointToRay(Vector3 screenPosition)
		{
			var ray = controlled.ScreenPointToRay(screenPosition);
			return ray;
		}
	}
}
