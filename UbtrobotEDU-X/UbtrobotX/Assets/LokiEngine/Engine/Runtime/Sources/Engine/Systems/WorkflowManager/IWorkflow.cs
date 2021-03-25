using System;
using System.Collections.Generic;

namespace Loki
{
	public delegate void WorkflowCallback(object context);

	public enum EWorkflowEndReason
	{
		Failure,
		ForceCompleted,
		Success,
	}

	public enum EWorkflowStatus
	{
		Running,
		Done,
	}

	public interface IWorkflow
	{
		object context { get; set; }
		EWorkflowStatus GetStatus();
		EWorkflowEndReason GetLastReason();
		EWorkflowStatus Begin(ref EWorkflowEndReason lastFlowReason);
		EWorkflowStatus OnUpdate(float deltaTime, out EWorkflowEndReason reason);
		void End(EWorkflowEndReason reason);
	}
}
