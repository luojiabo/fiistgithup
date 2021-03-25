using System;
using CommandUndoRedo;
using UnityEngine;

namespace RuntimeGizmos
{
	public class TransformCommand : ICommand
	{
		private TransformValues mNewValues;
		private TransformValues mOldValues;

		private Transform mTransform;
		private TransformGizmo mTransformGizmo;

		public TransformCommand(TransformGizmo transformGizmo, Transform transform)
		{
			this.mTransformGizmo = transformGizmo;
			this.mTransform = transform;

			mOldValues = new TransformValues() { position = transform.position, rotation = transform.rotation, scale = transform.localScale };
		}

		public void StoreNewTransformValues()
		{
			mNewValues = new TransformValues() { position = mTransform.position, rotation = mTransform.rotation, scale = mTransform.localScale };
		}

		public void Execute()
		{
			mTransform.position = mNewValues.position;
			mTransform.rotation = mNewValues.rotation;
			mTransform.localScale = mNewValues.scale;

			mTransformGizmo.SetPivotPoint();
		}

		public void UnExecute()
		{
			mTransform.position = mOldValues.position;
			mTransform.rotation = mOldValues.rotation;
			mTransform.localScale = mOldValues.scale;

			mTransformGizmo.SetPivotPoint();
		}

		struct TransformValues
		{
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 scale;
		}
	}
}
