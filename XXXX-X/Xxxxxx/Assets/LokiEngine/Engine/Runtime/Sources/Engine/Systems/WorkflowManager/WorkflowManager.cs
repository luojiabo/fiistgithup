using System;
using System.Collections.Generic;
using System.Linq;

namespace Loki
{
	public class WorkflowManager : UObject, IWorkflow, IUpdatable
	{
		private readonly List<IWorkflow> mWorkflows = new List<IWorkflow>();
		private EWorkflowStatus mLastStatus = EWorkflowStatus.Done;
		private EWorkflowEndReason mLastReason = EWorkflowEndReason.Success;
		private int mCurrentIndex = -1;

		private WorkflowCallback mOnFlowsStarted = null;
		private WorkflowCallback mOnFlowsEnded = null;

		public object context { get; set; }

		public int count { get { return mWorkflows.Count; } }

		public int currentIndex
		{
			get
			{
				return mCurrentIndex;
			}
			private set
			{
				mCurrentIndex = value;
			}
		}

		protected virtual void OnFlowsStarted()
		{
			if (mOnFlowsStarted != null)
			{
				mOnFlowsStarted(context);
			}
		}

		protected virtual void OnFlowsEnded()
		{
			if (mOnFlowsEnded != null)
			{
				mOnFlowsEnded(context);
			}
		}

		public void Next()
		{
			if (mLastStatus == EWorkflowStatus.Running)
			{
				EndCurrentFlow(EWorkflowEndReason.Success, EWorkflowEndReason.ForceCompleted);
				DoNext();
			}
		}

		public void AddWorkflow(IWorkflow workflow)
		{
			mWorkflows.Add(workflow);
		}

		public void OnUpdate(float deltaTime)
		{
			// just do update when the Workflow manager is running
			if (GetStatus() == EWorkflowStatus.Running)
			{
				OnUpdate(deltaTime, out var reason);
			}
		}

		public EWorkflowStatus GetStatus()
		{
			return mLastStatus;
		}

		public EWorkflowEndReason GetLastReason()
		{
			return mLastReason;
		}

		public virtual bool ToContinueWorkflow(EWorkflowEndReason lastEndReason)
		{
			return lastEndReason != EWorkflowEndReason.Failure;
		}

		private EWorkflowEndReason EndCurrentFlow(EWorkflowEndReason assumeReason, EWorkflowEndReason contextReason)
		{
			EWorkflowEndReason result = assumeReason;
			if (currentIndex >= 0 && currentIndex < count)
			{
				var current = mWorkflows[currentIndex];
				EWorkflowStatus status = current.GetStatus();
				if (status == EWorkflowStatus.Running)
				{
					current.End(contextReason);
				}
				result = current.GetLastReason();
			}
			mLastStatus = EWorkflowStatus.Done;
			mLastReason = result;
			return result;
		}

		public virtual EWorkflowStatus Begin(ref EWorkflowEndReason lastReason)
		{
			DebugUtility.AssertFormat(count > 0, "The flow queue in manager is empty.");

			mLastStatus = EWorkflowStatus.Running;
			mLastReason = EWorkflowEndReason.Success;

			Next();

			lastReason = mLastReason;
			// return the status of flow manager
			return mLastStatus;
		}

		public void End(EWorkflowEndReason reason)
		{

		}

		private void DoNext()
		{
			// status is the the status of single flow
			while (mLastStatus != EWorkflowStatus.Running)
			{
				currentIndex++;
				if (currentIndex >= count)
				{
					mLastStatus = EWorkflowStatus.Done;
					mLastReason = EWorkflowEndReason.Success;
					// reset the index
					currentIndex = -1;
					break;
				}
				else
				{
					var current = mWorkflows[currentIndex];
					if (!ToContinueWorkflow(mLastReason))
					{
						break;
					}

					mLastStatus = current.Begin(ref mLastReason);
					if (mLastStatus != EWorkflowStatus.Running)
					{
						current.End(mLastReason);
					}
				}
			}
		}

		private EWorkflowStatus UpdateCurrent(float deltaTime, out EWorkflowEndReason lastReason)
		{
			DebugUtility.AssertFormat(mLastStatus == EWorkflowStatus.Running, "The status is not [Running].");
			DebugUtility.AssertFormat(mLastReason != EWorkflowEndReason.Failure, "The last reason is [Failure].");

			EWorkflowStatus status = EWorkflowStatus.Running;
			lastReason = EWorkflowEndReason.Success;

			if (currentIndex >= 0 && currentIndex < count)
			{
				var current = mWorkflows[currentIndex];
				status = current.GetStatus();
				if (status == EWorkflowStatus.Running)
				{
					status = current.OnUpdate(deltaTime, out lastReason);
					if (status != EWorkflowStatus.Running)
					{
						current.End(lastReason);
						lastReason = current.GetLastReason();
					}
				}
			}

			mLastReason = lastReason;
			return status;
		}

		public EWorkflowStatus OnUpdate(float deltaTime, out EWorkflowEndReason lastReason)
		{
			DebugUtility.AssertFormat(mLastStatus == EWorkflowStatus.Running, "The status is not running.");

			UpdateCurrent(deltaTime, out lastReason);
			DoNext();
			return mLastStatus;
		}
	}
}
