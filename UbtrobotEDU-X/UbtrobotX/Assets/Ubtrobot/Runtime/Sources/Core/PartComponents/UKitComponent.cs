using System;
using System.Collections.Generic;
using System.Linq;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	[DisallowMultipleComponent]
	public abstract class UKitComponent : AdvancedPartComponent
	{
		private static readonly HashSet<ECommand> msSupportedCommands = new HashSet<ECommand>
		{
			ECommand.UKitCommand,
		};

		public  override DeviceType deviceID => DeviceType.UKit;

		public override ICommandResponseAsync Execute(ICommand command)
		{
			IProtocol result = null;
			switch (command.commandID)
			{
				case ECommand.UKitCommand:
					{
						result = ExecuteUKitCommands(command);
						break;
					}
			}

			if (result == null)
			{
				DebugUtility.LogError(LoggerTags.Project, "Failure to execute command : {0}", command);
			}
			else
			{
				DebugUtility.Log(LoggerTags.Project, "Success to execute command : {0}", command);
			}
			var cra = new CommandResponseAsync(result);
			cra.host = command.host;
			cra.context = command.context;
			return cra;
		}

		public override bool Verify(ICommand command)
		{
			if (command.id != id && command.id != 0)
				return false;

			if (!msSupportedCommands.Contains(command.commandID))
				return false;

			return OnVerify(command);
		}

		protected virtual bool OnVerify(ICommand command)
		{
			return false;
		}

		protected virtual IProtocol ExecuteUKitCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			return result;
		}

		public bool GetEnvironment(out Environment env)
		{
			env = null;
			var envSys = EnvironmentSystem.Get();
			if (envSys != null && envSys.GetEnvironment(transform.position, transform.forward, out env))
			{
				return true;
			}
			return false;
		}
	}
}
