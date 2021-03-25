using System;
using System.Collections.Generic;
using LitJson;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	internal static class ComponentSerializer
	{
		private static readonly Dictionary<Type, Func<Component, JsonData>> msComponentToJson = new Dictionary<Type, Func<Component, JsonData>>();

		private static readonly HashSet<Type> msBlacklist = new HashSet<Type>()
		{
			typeof(Transform),
			typeof(RectTransform),
		};

		private static readonly List<Type> msSkipAllocAtRuntimeComponents = new List<Type>()
		{
		};

		static ComponentSerializer()
		{
			RegisterParts();
			RegisterRobots();
			RegisterConnectivities();
			RegisterJointComponents();
			RegisterPhysics();
			RegisterUnityComponent();
		}

		private static void RegisterRobots()
		{
			Register(typeof(Robot), (component) =>
			{
				var userData = new JsonData();

				if (component is RobotVehicle vehicle)
				{
					if (vehicle.leftRotationParts != null && vehicle.leftRotationParts.Length > 0)
					{
						userData["left"] = LitJsonHelper.CreateArrayJsonData(vehicle.leftRotationParts.GetHierarchyPath(typeof(Robot)));
					}
					if (vehicle.rightRotationParts != null && vehicle.rightRotationParts.Length > 0)
					{
						userData["right"] = LitJsonHelper.CreateArrayJsonData(vehicle.rightRotationParts.GetHierarchyPath(typeof(Robot)));
					}
					userData["moveFactor"] = LitJsonHelper.CreateJsonData(vehicle.moveFactor);
					userData["rotationSpeed"] = LitJsonHelper.CreateJsonData(vehicle.rotationSpeed);
					userData["angularVelocity"] = LitJsonHelper.CreateJsonData(vehicle.angularVelocity);
				}

				return userData;
			});
		}

		private static void RegisterParts()
		{
			Register(typeof(Part), (component) =>
			{
				var part = (Part)component;
				var userData = new JsonData();
				userData[SerializationConst.address] = part.addressName;
				return userData;
			});

			Register(typeof(PartsGroup), (component) =>
			{
				var group = (PartsGroup)component;
				var userData = new JsonData();
				userData.SetJsonType(JsonType.Object);

				userData["up"] = (int)group.upAxis;
				userData["inv"] = group.inverseAxis.To01();
				userData["commandType"] = (int)group.commandType;

				if (group.controlTargets != null && group.controlTargets.Length > 0)
					userData["targets"] = LitJsonHelper.CreateArrayJsonData(group.controlTargets.GetHierarchyPath(typeof(Robot)));

				return userData;
			});

			Register(typeof(PartComponent), (component) =>
			{
				var userData = new JsonData();

				var pc = (PartComponent)component;
				if (pc.pivotLink != null && pc.pivotLink != pc.transform)
				{
					userData["pivotLink"] = pc.pivotLink.GetHierarchyPath(typeof(Part));
				}

				if (component is AdvancedPartComponent apc)
				{
					userData["id"] = apc.id;
				}

				if (component is ButtonSensorComponent bsc)
				{
					userData["eventTimeout"] = LitJsonHelper.CreateJsonData(bsc.eventTimeout);
					userData["timeout"] = LitJsonHelper.CreateJsonData(bsc.timeout);
				}

				if (component is LineTraceSensorComponent ltsc)
				{
					userData["expectedRange"] = LitJsonHelper.CreateJsonData(ltsc.expectedRange);
				}

				if (component is RotationPartComponent rpc)
				{
					if (rpc.axisDP != null)
					{
						userData["axisDP"] = rpc.axisDP.transform.GetHierarchyPath(typeof(Part));
					}

					userData["minEularAngleY"] = LitJsonHelper.CreateJsonData(rpc.minEularAngleY);
					userData["maxEularAngleY"] = LitJsonHelper.CreateJsonData(rpc.maxEularAngleY);
					userData["wrapMode"] = (int)rpc.wrapMode;
					userData["convertMode"] = (int)rpc.convertMode;
					userData["localEulerAnglesY"] = LitJsonHelper.CreateJsonData(rpc.localEulerAnglesY);
				}

				return userData;
			});

		}

		private static void RegisterConnectivities()
		{
			Register(typeof(Connectivity), (component) =>
			{
				var userData = new JsonData();
				var c = (Connectivity)component;
				if (c != null)
				{
					if (c.pivotLink != null && c.pivotLink != c.transform)
					{
						userData["pivotLink"] = c.pivotLink.GetHierarchyPath(typeof(Robot));
					}

					userData["up"] = (int)c.upAxis;
					userData["inv"] = c.inverseAxis.To01();
				}

				if (component is AxleConnectivity axle)
				{
					userData["selfRotation"] = axle.isSelfRotation.To01();
					if (axle.joint != null)
						userData["joint"] = axle.joint.transform.GetHierarchyPath(typeof(Robot));

				}
				return userData;
			});
		}

		private static void RegisterJointComponents()
		{
			Register(typeof(JointComponent), (component) =>
			{
				var userData = new JsonData();
				if (component is JointComponent jc)
				{
					if (jc.controlTarget != null)
					{
						userData["controlTarget"] = jc.controlTarget.transform.GetHierarchyPath(typeof(Robot));
					}
					if (jc.controller != null)
					{
						userData["controller"] = jc.controller.transform.GetHierarchyPath(typeof(Robot));
					}
				}

				return userData;
			});
		}

		private static void RegisterPhysics()
		{
			Register(typeof(BoxCollider), (component) =>
			{
				var userData = new JsonData();
				if (component is BoxCollider collider)
				{
					userData["size"] = LitJsonHelper.CreateJsonData(collider.size);
					userData["center"] = LitJsonHelper.CreateJsonData(collider.center);
					userData["isTrigger"] = collider.isTrigger.To01();
				}
				return userData;
			});

			Register(typeof(SphereCollider), (component) =>
			{
				var userData = new JsonData();
				if (component is SphereCollider collider)
				{
					userData["radius"] = LitJsonHelper.CreateJsonData(collider.radius);
					userData["center"] = LitJsonHelper.CreateJsonData(collider.center);
					userData["isTrigger"] = collider.isTrigger.To01();
				}
				return userData;
			});

			Register(typeof(MeshCollider), (component) =>
			{
				throw new Exception("Unsupported MeshCollider");
			});

			Register(typeof(CapsuleCollider), (component) =>
			{
				var userData = new JsonData();
				if (component is CapsuleCollider collider)
				{
					userData["radius"] = LitJsonHelper.CreateJsonData(collider.radius);
					userData["center"] = LitJsonHelper.CreateJsonData(collider.center);
					userData["height"] = LitJsonHelper.CreateJsonData(collider.height);
					userData["direction"] = collider.direction;
					userData["isTrigger"] = collider.isTrigger.To01();
				}
				return userData;
			});

			Register(typeof(Rigidbody), (component) =>
			{
				var userData = new JsonData();
				if (component is Rigidbody rigidbody)
				{
					userData["mass"] = LitJsonHelper.CreateJsonData(rigidbody.mass);
					userData["drag"] = LitJsonHelper.CreateJsonData(rigidbody.drag);
					userData["angularDrag"] = LitJsonHelper.CreateJsonData(rigidbody.angularDrag);
					userData["useGravity"] = rigidbody.useGravity.To01();
					userData["isKinematic"] = rigidbody.isKinematic.To01();
					userData["collisionDetectionMode"] = (int)rigidbody.collisionDetectionMode;
					userData["interpolation"] = (int)rigidbody.interpolation;
					userData["constraints"] = (int)rigidbody.constraints;
				}
				return userData;
			});

		}

		private static void RegisterUnityComponent()
		{
			Register(typeof(Light), (component) =>
			{
				var userData = new JsonData();
				if (component is Light light)
				{
					//userData["size"] = LitJsonHelper.CreateJsonData(collider.size);
					//userData["center"] = LitJsonHelper.CreateJsonData(collider.center);
					//userData["isTrigger"] = collider.isTrigger.To01();

				}
				return userData;
			});
		}

		public static JsonData ToJson(Transform tr, JsonData node)
		{
			try
			{
				node[SerializationConst.name] = tr.name;
				node[SerializationConst.path] = tr.GetHierarchyPath(typeof(Robot));
				node[SerializationConst.layer] = tr.gameObject.layer;
				string tag = tr.gameObject.tag;
				node[SerializationConst.tag] = tag;// == TagUtility.EditorOnly ? TagUtility.Untagged : tag;
				var transform = new JsonData();
				transform.Add(LitJsonHelper.CreateJsonData(tr.transform.localPosition));
				transform.Add(LitJsonHelper.CreateJsonData(tr.transform.localRotation));
				transform.Add(LitJsonHelper.CreateJsonData(tr.transform.localScale));
				node[SerializationConst.transform] = transform;
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Project, ex.Message);
			}
			return node;
		}

		private static JsonData CreateComponentJson(Component component, JsonData userDatas)
		{
			if (userDatas == null)
			{
				return null;
			}

			var result = new JsonData();
			var targetType = component.GetType();
			result[SerializationConst.type] = targetType.Name;

			bool rtAlloc = true;
			foreach (var skipAlloc in msSkipAllocAtRuntimeComponents)
			{
				if (targetType == skipAlloc || targetType.IsCompatibleWith(skipAlloc))
				{
					rtAlloc = false;
					break;
				}
			}
			result[SerializationConst.alloc] = rtAlloc.To01();
			if (userDatas.GetJsonType() != JsonType.None)
				result[SerializationConst.userDatas] = userDatas;
			return result;
		}

		private static void Register(Type type, Func<Component, JsonData> serialization)
		{
			msComponentToJson[type] = serialization;
		}

		public static JsonData ComponentToJson(Component component)
		{
			var type = component.GetType();
			if (msBlacklist.Contains(type))
				return null;

			JsonData userData = null;
			if (msComponentToJson.TryGetValue(type, out var toJsonFunc))
			{
				userData = toJsonFunc(component);
			}
			else
			{
				foreach (var item in msComponentToJson)
				{
					if (type.IsCompatibleWith(item.Key, false))
					{
						userData = item.Value(component);
						break;
					}
				}
			}
			return CreateComponentJson(component, userData);
		}

	}

	internal static class ComponentDeserializer
	{
		private static readonly Dictionary<Type, Func<RobotComponentDataModel, Component, bool>> msJsonToComponent = new Dictionary<Type, Func<RobotComponentDataModel, Component, bool>>();

		static ComponentDeserializer()
		{
			RegisterParts();
			RegisterRobots();
			RegisterConnectivities();
			RegisterJointComponents();
			RegisterPhysics();
		}

		private static void RegisterParts()
		{
			Register(typeof(Part), (componentData, component) =>
			{
				return true;
			});

			Register(typeof(PartsGroup), (componentData, component) =>
			{
				var userData = componentData.userData;
				if (userData != null)
				{
					if (component is PartsGroup group)
					{
						group.upAxis = (AxisType)(int)userData["up"];
						group.inverseAxis = (int)userData["inv"] == 1;
						group.commandType = (ECommandType)(int)userData["commandType"];

						if (userData.ContainsKey("targets"))
						{
							var robot = group.robot;
							var targetPaths = LitJsonHelper.ParseJsonArray(userData["targets"], data => (string)data, ArrayHelper<string>.Empty);
							group.controlTargets = new Group[targetPaths.Length];
							for (int i = 0; i < targetPaths.Length; i++)
							{
								string path = targetPaths[i];
								if (string.IsNullOrEmpty(path))
									continue;

								group.controlTargets[i] = robot.transform.FindHierarchyPath<Group>(path);
							}
						}
					}
				}
				return true;
			});

			Register(typeof(PartComponent), (componentData, component) =>
			{
				var userData = componentData.userData;
				if (userData != null)
				{
					var pc = (PartComponent)component;
					var part = pc.GetPart();

					if (userData.ContainsKey("pivotLink"))
					{
						if (part != null)
						{
							pc.pivotLink = part.FindHierarchyPath((string)userData["pivotLink"]);
						}
					}

					if (component is AdvancedPartComponent apc)
					{
						apc.id = (int)userData["id"];
					}

					if (component is ButtonSensorComponent bsc)
					{
						bsc.eventTimeout = LitJsonHelper.ParseJsonToFloat(userData["eventTimeout"]);
						bsc.timeout = LitJsonHelper.ParseJsonToFloat(userData["timeout"]);
					}

					if (component is LineTraceSensorComponent ltsc)
					{
						ltsc.expectedRange = LitJsonHelper.ParseJsonToFloat(userData["expectedRange"]);
					}

					if (component is RotationPartComponent rpc)
					{
						if (part != null)
						{
							if (userData.ContainsKey("axisDP"))
							{
								rpc.axisDP = part.FindHierarchyPath<AxleConnectivity>((string)userData["axisDP"]);
							}
						}

						rpc.wrapMode = (ERotationWrapMode)(int)userData["wrapMode"];
						rpc.convertMode = (EEulerAngleConvertMode)(int)userData["convertMode"];
						rpc.minEularAngleY = LitJsonHelper.ParseJsonToFloat(userData["minEularAngleY"]);
						rpc.maxEularAngleY = LitJsonHelper.ParseJsonToFloat(userData["maxEularAngleY"]);
						rpc.localEulerAnglesY = LitJsonHelper.ParseJsonToFloat(userData["localEulerAnglesY"]);
					}
				}

				return true;
			});

		}

		private static void RegisterRobots()
		{
			Register(typeof(Robot), (componentData, component) =>
			{
				var userData = componentData.userData;
				if (userData != null)
				{
					if (component is RobotVehicle vehicle)
					{
						if (userData.ContainsKey("left"))
						{
							var left = userData["left"];
							int leftCount = left.Count;
							var leftRotations = LitJsonHelper.ParseJsonArray(left, data => (string)data, ArrayHelper<string>.Empty);
							vehicle.leftRotationParts = new RotationPartComponent[leftCount];
							for (int i = 0; i < leftCount; i++)
							{
								vehicle.leftRotationParts[i] = vehicle.FindHierarchyPath<RotationPartComponent>(leftRotations[i]);
							}
						}

						if (userData.ContainsKey("right"))
						{
							var right = userData["right"];
							int rightCount = right.Count;
							var rightRotations = LitJsonHelper.ParseJsonArray(right, data => (string)data, ArrayHelper<string>.Empty);
							vehicle.rightRotationParts = new RotationPartComponent[rightCount];
							for (int i = 0; i < rightCount; i++)
							{
								vehicle.rightRotationParts[i] = vehicle.FindHierarchyPath<RotationPartComponent>(rightRotations[i]);
							}
						}
						vehicle.moveFactor = LitJsonHelper.ParseJsonToFloat(userData["moveFactor"]);
						vehicle.rotationSpeed = LitJsonHelper.ParseJsonToFloat(userData["rotationSpeed"]);
						vehicle.angularVelocity = LitJsonHelper.ParseJsonToFloat(userData["angularVelocity"]);
					}
				}
				return true;
			});
		}

		private static void RegisterConnectivities()
		{
			Register(typeof(Connectivity), (componentData, component) =>
			{
				var userData = componentData.userData;
				if (userData != null)
				{
					var c = (Connectivity)component;
					var robot = c.controller != null && c.controller.GetPart() ? c.controller.GetPart().robot : null;
					if (robot != null)
					{
						if (userData.ContainsKey("pivotLink"))
						{
							c.pivotLink = robot.transform.FindHierarchyPath((string)userData["pivotLink"]);
						}

						if (userData.ContainsKey("up"))
							c.upAxis = (AxisType)(int)userData["up"];

						if (userData.ContainsKey("inv"))
							c.inverseAxis = (int)userData["inv"] == 1;

						if (component is AxleConnectivity axle)
						{
							axle.isSelfRotation = (int)userData["selfRotation"] == 1;
							if (userData.ContainsKey("joint"))
								axle.joint = robot.transform.FindHierarchyPath<AxleJointComponent>((string)userData["joint"]);
						}
					}
				}
				return true;
			});
		}

		private static void RegisterJointComponents()
		{
			Register(typeof(JointComponent), (componentData, component) =>
			{
				var userData = componentData.userData;
				if (userData != null)
				{
					if (component is JointComponent jc)
					{
						var robot = jc.transform.GetComponentInParent<Robot>(false, true);
						if (userData.ContainsKey("controlTarget"))
						{
							jc.controlTarget = robot.FindHierarchyPath<Group>((string)userData["controlTarget"]);
						}
						if (userData.ContainsKey("controller"))
						{
							jc.controller = robot.FindHierarchyPath<Connectivity>((string)userData["controller"]);
						}
					}
				}
				return true;
			});
		}

		private static void RegisterPhysics()
		{
			Register(typeof(BoxCollider), (componentData, component) =>
			{
				var userData = componentData.userData;
				if (userData != null)
				{
					if (component is BoxCollider collider)
					{
						LitJsonHelper.ParseVector(userData["size"], out Vector3 size);
						collider.size = size;
						LitJsonHelper.ParseVector(userData["center"], out Vector3 center);
						collider.center = center;
						collider.isTrigger = ((int)userData["isTrigger"]) == 1;
					}
				}
				return true;
			});

			Register(typeof(SphereCollider), (componentData, component) =>
			{
				var userData = componentData.userData;
				if (userData != null)
				{
					if (component is SphereCollider collider)
					{
						collider.radius = LitJsonHelper.ParseJsonToFloat(userData["radius"]);
						LitJsonHelper.ParseVector(userData["center"], out Vector3 center);
						collider.center = center;
						collider.isTrigger = ((int)userData["isTrigger"]) == 1;
					}
				}
				return true;
			});

			Register(typeof(MeshCollider), (componentData, component) =>
			{
				return true;
			});

			Register(typeof(CapsuleCollider), (componentData, component) =>
			{
				var userData = componentData.userData;
				if (userData != null)
				{
					if (component is CapsuleCollider collider)
					{
						collider.radius = LitJsonHelper.ParseJsonToFloat(userData["radius"]);
						collider.height = LitJsonHelper.ParseJsonToFloat(userData["height"]);
						LitJsonHelper.ParseVector(userData["center"], out Vector3 center);
						collider.center = center;
						collider.isTrigger = ((int)userData["isTrigger"]) == 1;
						collider.direction = (int)userData["direction"];
					}
				}
				return true;
			});

			Register(typeof(Rigidbody), (componentData, component) =>
			{
				var userData = componentData.userData;
				if (userData != null)
				{
					if (component is Rigidbody rigidbody)
					{
						rigidbody.mass = LitJsonHelper.ParseJsonToFloat(userData["mass"]);
						rigidbody.drag = LitJsonHelper.ParseJsonToFloat(userData["drag"]);
						rigidbody.angularDrag = LitJsonHelper.ParseJsonToFloat(userData["angularDrag"]);
						rigidbody.useGravity = (int)userData["useGravity"] == 1;
						rigidbody.isKinematic = (int)userData["isKinematic"] == 1;
						rigidbody.collisionDetectionMode = (CollisionDetectionMode)(int)userData["collisionDetectionMode"];
						rigidbody.interpolation = (RigidbodyInterpolation)(int)userData["interpolation"];
						rigidbody.constraints = (RigidbodyConstraints)(int)userData["constraints"];
					}
				}
				return true;
			});

		}

		private static void Register(Type type, Func<RobotComponentDataModel, Component, bool> deserialization)
		{
			msJsonToComponent[type] = deserialization;
		}

		public static Component DeserializeFromJson(RobotComponentDataModel componentDataModel)
		{
			var component = componentDataModel.component;
			if (component == null)
				return null;

			var targetType = component.GetType();
			if (!msJsonToComponent.TryGetValue(targetType, out var convert))
			{
				foreach (var kv in msJsonToComponent)
				{
					if (targetType.IsCompatibleWith(kv.Key))
					{
						convert = kv.Value;
						break;
					}
				}
			}
			if (convert != null)
			{
				try
				{
					convert(componentDataModel, component);
				}
				catch (Exception ex)
				{
					DebugUtility.LogException(ex);
				}
			}
			return component;
		}

		public static Component AttachComponent(Transform transform, RobotComponentDataModel componentDataModel)
		{
			var component = transform.GetUserComponent<Component>(componentDataModel.type);
			componentDataModel.component = component;

			if (!componentDataModel.alloc)
			{
				return component;
			}
			else if (component == null)
			{
				component = transform.AddUserComponent<Component>(componentDataModel.type);
				componentDataModel.component = component;
				if (component == null)
				{
					DebugUtility.LogErrorTrace(LoggerTags.Project, "Missing Type {0}", componentDataModel.type);
				}
				return component;
			}
			return component;
		}

		public static void ComponentDataFromJson(JsonData jsonData, out string type, out bool alloc, out JsonData userDatas)
		{
			type = (string)jsonData[SerializationConst.type];
			alloc = (int)jsonData[SerializationConst.alloc] == 1;
			if (jsonData.ContainsKey(SerializationConst.userDatas))
			{
				userDatas = jsonData[SerializationConst.userDatas];
			}
			else
			{
				userDatas = null;
			}
		}

		public static void FromJson(JsonData jsonNode, out string name, out string path, out int layer, out string tag, out Vector3 pos, out Quaternion rot, out Vector3 scale)
		{
			name = (string)jsonNode[SerializationConst.name];
			path = (string)jsonNode[SerializationConst.path];
			tag = string.Empty;
			layer = LayerUtility.DefaultLayer;

			if (jsonNode.ContainsKey(SerializationConst.layer))
				layer = (int)jsonNode[SerializationConst.layer];
			if (jsonNode.ContainsKey(SerializationConst.tag))
				tag = (string)jsonNode[SerializationConst.tag];
			if (string.IsNullOrEmpty(tag))
				tag = TagUtility.Untagged;

			var transform = jsonNode[SerializationConst.transform];
			LitJsonHelper.ParseVector(transform[0], out pos);
			LitJsonHelper.ParseQuaternion(transform[1], out rot);
			LitJsonHelper.ParseVector(transform[2], out scale);
		}

		public static void ComponentsFromJson(JsonData jsonNode, Action<JsonData> forEveryComponent)
		{
			if (jsonNode.ContainsKey(SerializationConst.components))
			{
				var componentsJson = jsonNode[SerializationConst.components];
				int componentsJsonCount = componentsJson.Count;
				for (int i = 0; i < componentsJsonCount; i++)
				{
					forEveryComponent(componentsJson[i]);
				}
			}
		}

		public static void ChildrenFromJson(JsonData jsonNode, Action<JsonData> forEveryChild)
		{
			if (jsonNode.ContainsKey(SerializationConst.children))
			{
				var childrenJson = jsonNode[SerializationConst.children];
				int childrenJsonCount = childrenJson.Count;
				for (int i = 0; i < childrenJsonCount; i++)
				{
					forEveryChild(childrenJson[i]);
				}
			}
		}

	}

}
