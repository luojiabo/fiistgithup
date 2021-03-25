using System;
using System.Collections.Generic;
using System.Linq;

namespace Loki
{
	public abstract class Workflow : IWorkflow
	{
		private EWorkflowEndReason mLastReason = EWorkflowEndReason.Success;
		private EWorkflowStatus mStatus = EWorkflowStatus.Running;

		public object context { get; set; }
		public bool pause { get; set; } = false;
		public float passed { get; private set; } = 0.0f;

		public virtual float duration { get { return -1.0f; } }

		public EWorkflowStatus OnUpdate(float deltaTime, out EWorkflowEndReason reason)
		{
			reason = EWorkflowEndReason.Success;

			EWorkflowStatus status = mStatus;
			if (status == EWorkflowStatus.Running)
			{
				passed += deltaTime;
				status = DoUpdate(deltaTime, out reason);

				if (status != EWorkflowStatus.Done)
				{
					if (duration > 0.0f && passed >= duration)
					{
						status = EWorkflowStatus.Done;
						reason = EWorkflowEndReason.Success;
					}
				}
			}
			return status;
		}

		public EWorkflowStatus GetStatus()
		{
			return mStatus;
		}

		public EWorkflowEndReason GetLastReason()
		{
			return mLastReason;
		}

		public virtual EWorkflowStatus Begin(ref EWorkflowEndReason lastFlowReason)
		{
			return EWorkflowStatus.Running;
		}

		public void End(EWorkflowEndReason reason)
		{
			mStatus = EWorkflowStatus.Done;
			mLastReason = reason;
			OnStop(reason);
		}

		public void Skip()
		{

		}

		protected virtual void OnStop(EWorkflowEndReason reason)
		{

		}

		protected virtual EWorkflowStatus DoUpdate(float deltaTime, out EWorkflowEndReason reason)
		{
			reason = EWorkflowEndReason.Success;

			return EWorkflowStatus.Done;
		}

	}
}
