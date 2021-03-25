using System.Collections.Generic;
using Loki;

namespace Ubtrobot
{
	public enum ECommand
	{
		ServoCommand = 1,
		MotorCommand,
		EyeCommand,
		UKitCommand,
		SoundCommand,
		RGBLedCommand,
		BatteryCommand,
		PatrolSensorCommand,
		GyroCommand,
		KeyFeedbackCommand,
		MiscCommand,
	}

	public enum ECommandType
	{
		Disable,
		Broadcast,
		Unicast,
	}

	public interface ICommand
	{
		object context { get; set; }
		string host { get; set; }
		string uuid { get; set; }
		ECommand commandID { get; }
		bool debug { get; set; }
		int id { get; set; }
		int device { get; set; }
		int cmdMode { get; set; }
		/// <summary>
		/// Is this a task command
		/// It means that it will block until it is completed
		/// </summary>
		bool taskCommand { get; set; }
		void Release();
	}

	public interface ICommandResponseAsync
	{
		// yield to wait it done
		bool done { get; }
		bool ignoreProtocol { get; set; }
		object context { get; }
		string host { get; }

		IProtocol protocol { get; set; }
		void GetProtocols(List<IProtocol> protocols);
		void AddResponse(ICommandResponseAsync job);
	}

	public interface ICommandHandler
	{
		ECommandType commandType { get; }
		ICommandResponseAsync Execute(ICommand command);
		bool Verify(ICommand command);
		void Stop();
	}

	public class CommandResponseAsync : ICommandResponseAsync
	{
		private readonly List<ICommandResponseAsync> mWaiting = new List<ICommandResponseAsync>();
		private readonly List<ICommandResponseAsync> mResponses = new List<ICommandResponseAsync>();

		public bool done
		{
			get
			{
				if (mWaiting.Count > 0)
				{
					mWaiting.RemoveAll(job => job.done);
				}
				return mWaiting.Count == 0 && (protocol != null || ignoreProtocol);
			}
		}

		public bool ignoreProtocol { get; set; }

		public List<ICommandResponseAsync> responses { get { return mResponses; } }

		public object context { get; set; }

		public string host { get; set; }

		/// <summary>
		/// 对于已经存在的协议结果，不需要从其他回应数据中提取
		/// </summary>
		public IProtocol protocol { get; set; }

		public CommandResponseAsync()
		{
			ignoreProtocol = true;
		}

		public CommandResponseAsync(IProtocol protocol)
		{
			this.protocol = protocol;
		}

		public void GetProtocols(List<IProtocol> protocols)
		{
			// 如果没有完成，则不处理协议
			if (!done)
				return;

			if (protocol != null)
			{
				protocol.host = host;
				protocols.Add(protocol);
			}
			foreach (var res in mResponses)
			{
				res.GetProtocols(protocols);
			}
		}

		public void AddResponse(ICommandResponseAsync job)
		{
			// 回应列表 >= 等待列表，外部不断去检索是否已经done，如果已经done，等待列表为空
			mResponses.Union(job);

			if (!job.done)
			{
				// 如果这个任务没有完成则继续检索
				mWaiting.Union(job);
			}
		}
	}

	public class TriggerCommandResponseAsync : ICommandResponseAsync
	{
		public bool done { get; set; } = false;
		public bool ignoreProtocol { get; set; } = false;
		public bool isAsyncJob { get; } = true;
		public object context { get; set; }
		public string host { get; set; }
		public IProtocol protocol { get; set; }

		public TriggerCommandResponseAsync(IProtocol p)
		{
			protocol = p;
		}

		public void AddResponse(ICommandResponseAsync job)
		{
		}

		public void GetProtocols(List<IProtocol> protocols)
		{
			// 如果没有完成，则不处理协议
			if (!done)
				return;

			if (protocol != null)
			{
				protocol.host = host;
				protocols.Add(protocol);
			}
		}
	}
}
