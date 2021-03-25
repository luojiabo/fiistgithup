using System;
using System.Collections.Generic;
using LitJson;
using Loki;

namespace Ubtrobot
{
	public abstract class RobotSerializer<TRobot> where TRobot : IRobot
	{
		protected const int kVersion = 1;

		protected TRobot mRobot;

		public RobotSerializer()
		{
		}

		public RobotSerializer(TRobot robot)
		{
			mRobot = robot;
		}

		public LitJson.JsonData ToJson()
		{
			try
			{
				LitJson.JsonData root = new LitJson.JsonData();
				root.SetJsonType(LitJson.JsonType.Object);
				root["ver"] = kVersion;
				var model = new JsonData();
				model.SetJsonType(JsonType.Object);
				SerializeToJson(model);
				root["model"] = model;
				return root;
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			return null;
		}

		public RobotDataModel FromJson(LitJson.JsonData root)
		{
			mRobot = default;
			try
			{
				var ver = (int)root["ver"];
				if (ver != kVersion)
				{
					DebugUtility.LogError(LoggerTags.Project, "Unsupported versions.");
					return default;
				}

				var model = root["model"];
				if (!model.IsObject)
				{
					DebugUtility.LogError(LoggerTags.Project, "Unsupported versions - the model is error.");
					return default;
				}

				return DeserializeFromJson(model);
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			return default;
		}

		protected abstract void SerializeToJson(LitJson.JsonData jsonData);

		protected abstract RobotDataModel DeserializeFromJson(LitJson.JsonData jsonData);
	}
}
