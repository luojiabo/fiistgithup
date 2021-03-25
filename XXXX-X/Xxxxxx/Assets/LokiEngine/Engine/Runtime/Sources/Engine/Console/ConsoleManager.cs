using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace Loki
{
	public class ConsoleManager : USingletonObject<ConsoleManager>, ISystem
	{
		struct ConsoleCommand
		{
			public ConsoleAttribute attribute;
			public MemberInfo memberInfo;

			public PropertyInfo property
			{
				get
				{
					return (memberInfo != null && memberInfo.MemberType == MemberTypes.Property) ? (PropertyInfo)memberInfo : null;
				}
			}
			public FieldInfo field
			{
				get
				{
					return (memberInfo != null && memberInfo.MemberType == MemberTypes.Field) ? (FieldInfo)memberInfo : null;
				}
			}
			public MethodInfo method
			{
				get
				{
					return (memberInfo != null && memberInfo.MemberType == MemberTypes.Method) ? (MethodInfo)memberInfo : null;
				}
			}
			public MethodInfo validate;
			public Type owner;
			public string cmd;

			private bool SetFieldValue(IEnumerable<IConsoleObject> objs, object value)
			{
				try
				{
					var field = this.field;
					if (!field.IsStatic && objs == null)
					{
						return false;
					}

					value = (field.FieldType.IsValueType && value == null) ? Misc.Default(field.FieldType) : value;

					object[] args = new[] { value };

					// only call once
					if (field.IsStatic)
					{
						bool result = true;
						if (validate != null)
						{
							result = (bool)validate.Invoke(null, args);
						}

						if (result)
						{
							field.SetValue(null, value);
						}
					}
					else
					{
						foreach (var obj in objs)
						{
							bool result = true;
							if (validate != null)
							{
								result = (bool)validate.Invoke(obj, args);
							}
							if (result)
							{
								field.SetValue(obj, value);
							}
						}
					}
				}
				catch (Exception ex)
				{
					DebugUtility.LogException(ex);
					return false;
				}
				return true;
			}

			private bool GetFieldValue(IEnumerable<IConsoleObject> objs)
			{
				try
				{
					var field = this.field;
					if (!field.IsStatic && objs == null)
					{
						return false;
					}

					// only call once
					if (field.IsStatic)
					{
						object resultObject = field.GetValue(null);
						if (resultObject != null)
						{
							DebugUtility.Log(LoggerTags.Console, "Field Value （{1}) : \n{0}", resultObject, owner.Name);
						}
					}
					else
					{
						foreach (var obj in objs)
						{
							object resultObject = field.GetValue(obj);
							if (resultObject != null)
							{
								DebugUtility.Log(LoggerTags.Console, "Field Value ({1}) : \n{0}", resultObject, obj.statID);
							}
						}
					}
				}
				catch (Exception ex)
				{
					DebugUtility.LogException(ex);
					return false;
				}
				return true;
			}

			private bool SetPropertyValue(IEnumerable<IConsoleObject> objs, object value)
			{
				try
				{
					var property = this.property;
					if (!property.IsStatic() && objs == null)
					{
						return false;
					}

					value = (property.PropertyType.IsValueType && value == null) ? Misc.Default(property.PropertyType) : value;

					object[] args = new[] { value };

					// only call once
					if (property.IsStatic())
					{
						bool result = true;
						if (validate != null)
						{
							result = (bool)validate.Invoke(null, args);
						}

						if (result)
						{
							property.SetValue(null, value);
						}
					}
					else
					{
						foreach (var obj in objs)
						{
							bool result = true;
							if (validate != null)
							{
								result = (bool)validate.Invoke(obj, args);
							}
							if (result)
							{
								property.SetValue(obj, value);
							}
						}
					}
				}
				catch (Exception ex)
				{
					DebugUtility.LogException(ex);
					return false;
				}
				return true;
			}

			private bool GetPropertyValue(IEnumerable<IConsoleObject> objs)
			{
				try
				{
					var property = this.property;
					if (!property.IsStatic() && objs == null)
					{
						return false;
					}

					// only call once
					if (property.IsStatic())
					{
						object resultObject = property.GetValue(null);
						if (resultObject != null)
						{
							DebugUtility.Log(LoggerTags.Console, "Property Value （{1}) : \n{0}", resultObject, owner.Name);
						}
					}
					else
					{
						foreach (var obj in objs)
						{
							object resultObject = property.GetValue(obj);
							if (resultObject != null)
							{
								DebugUtility.Log(LoggerTags.Console, "Property Value ({1}) : \n{0}", resultObject, obj.statID);
							}
						}
					}
				}
				catch (Exception ex)
				{
					DebugUtility.LogException(ex);
					return false;
				}
				return true;
			}

			private bool ExecuteMethod(IEnumerable<IConsoleObject> objs, object[] args)
			{
				try
				{
					var method = this.method;
					if (method == null)
					{
						return false;
					}

					if (!method.IsStatic && objs == null)
					{
						return false;
					}

					// only call once
					if (method.IsStatic)
					{
						bool result = true;
						if (validate != null)
						{
							result = (bool)validate.Invoke(null, args);
						}

						if (result)
						{
							var resultObject = method.Invoke(null, args);
							if (resultObject != null)
							{
								DebugUtility.Log(LoggerTags.Console, "Method result （{1}) : \n{0}", resultObject, owner.Name);
							}
						}
					}
					else
					{
						foreach (var obj in objs)
						{
							bool result = true;
							if (validate != null)
							{
								result = (bool)validate.Invoke(obj, args);
							}
							if (result)
							{
								var resultObject = method.Invoke(obj, args);
								if (resultObject != null)
								{
									DebugUtility.Log(LoggerTags.Console, "Method result ({1}) : \n{0}", resultObject, obj.statID);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					DebugUtility.LogException(ex);
					return false;
				}
				return true;
			}

			public bool Process(IEnumerable<IConsoleObject> objs, string cmdWithParams, string[] cmdWithArgs)
			{
				var field = this.field;
				var method = this.method;
				var property = this.property;

				if (field == null && method == null && property == null)
					return false;

				if (owner == null)
					return false;

				if (field != null)
				{
					if (cmdWithArgs.Length > 1)
					{
						if (cmdWithArgs.Length > 2)
						{
							DebugUtility.LogError(LoggerTags.Console, "Please check your params: {0}", cmdWithParams);
							return false;
						}
						object fieldValue = TypeUtility.ToObject(cmdWithArgs[1], field.FieldType);
						return SetFieldValue(objs, fieldValue);
					}
					else
					{
						// DebugUtility.LogError(LoggerTags.Console, "Please check your params: {0}", cmdWithParams);
						return GetFieldValue(objs);
					}
				}

				if (property != null)
				{
					if (cmdWithArgs.Length > 1)
					{
						if (cmdWithArgs.Length > 2)
						{
							DebugUtility.LogError(LoggerTags.Console, "Please check your params: {0}", cmdWithParams);
							return false;
						}
						object propertyType = TypeUtility.ToObject(cmdWithArgs[1], property.PropertyType);
						return SetPropertyValue(objs, propertyType);
					}
					else
					{
						// DebugUtility.LogError(LoggerTags.Console, "Please check your params: {0}", cmdWithParams);
						return GetPropertyValue(objs);
					}
				}

				if (method != null)
				{
					var parameters = method.GetParameters();
					if (parameters.Length != cmdWithArgs.Length - 1)
					{
						DebugUtility.LogError(LoggerTags.Console, "Please check your params: {0}", cmdWithParams);
						return false;
					}
					object[] args = new object[parameters.Length];
					for (var i = 0; i < args.Length; ++i)
					{
						args[i] = TypeUtility.ToObject(cmdWithArgs[i + 1], parameters[i].ParameterType);
					}
					return ExecuteMethod(objs, args);
				}

				return false;
			}
		}

		struct ConsoleLog
		{
			public string msg;
			public string stacktrace;
			public LogType logType;

			internal void DrawTextArea(Rect consoleViewRect, GUIStyle style)
			{
				var color = GUI.color;
				GUI.color = DebugUtility.GetLogColor(logType);
				GUI.TextArea(consoleViewRect, string.Join("\n", msg, stacktrace), style);
				GUI.color = color;
			}
		}

		private static readonly char[] msParamSeparators = new[] { ' ' };
		private static readonly Vector2 msExecCMDSize = new Vector2(50.0f, 30.0f);
		private static readonly float msCMDHeight = 30.0f;
		private static readonly float msMsgHeight = 80.0f;
		private static readonly float msMsgPadding = 1.0f;
		private static readonly int msMaxHistoryMessageCount = 30;
		private static readonly int msMaxHistoryCommandCount = 20;
		private static readonly string msConsoleCommandInputName = "ConsoleCommand.OnCommandInput";

		private static readonly Type msType = typeof(ConsoleManager);

		private int mManagedThreadId = -1;

		private readonly HashSet<Type> mSearchedType = new HashSet<Type>();
		private readonly Dictionary<Type, List<IConsoleObject>> mConsoleInfos = new Dictionary<Type, List<IConsoleObject>>();
		private readonly Dictionary<string, List<ConsoleCommand>> mCommandInfos = new Dictionary<string, List<ConsoleCommand>>();
		private bool mIsVisible = false;

		private readonly List<ConsoleLog> mHistoryMessages = new List<ConsoleLog>();

		private readonly List<ConsoleLog> mHistoryThreadMessages = new List<ConsoleLog>();
		private readonly object mHistoryThreadMessageMute = new object();

		private readonly List<string> mPreviousCmds = new List<string>();
		private Vector2 mConsoleHistoryPosition = Vector2.zero;
		private int mPreviousCmdInversedIndex = -1;
		private string mCurrentCmd = "";
		private string mCurrentFocusName = string.Empty;

		private GUIStyle mLabelStyle = null;
		private GUIStyle mTextFieldStyle = null;
		private GUIStyle mTextAreaStyle = null;
		private GUIStyle mButtonStyle = null;

		private GUIStyle buttonStyle
		{
			get
			{
				if (mButtonStyle == null)
				{
					mButtonStyle = new GUIStyle(GUI.skin.button);
				}
				return mButtonStyle;
			}
		}

		private GUIStyle labelStyle
		{
			get
			{
				if (mLabelStyle == null)
				{
					mLabelStyle = new GUIStyle(GUI.skin.label);
				}
				return mLabelStyle;
			}
		}

		private GUIStyle textFieldStyle
		{
			get
			{
				if (mTextFieldStyle == null)
				{
					mTextFieldStyle = new GUIStyle(GUI.skin.textField);
				}
				return mTextFieldStyle;
			}
		}

		private GUIStyle textAreaStyle
		{
			get
			{
				if (mTextAreaStyle == null)
				{
					mTextAreaStyle = new GUIStyle(GUI.skin.textArea);
				}
				return mTextAreaStyle;
			}
		}

		[PreviewMember]
		[ConsoleProperty(aliasName = "console.defaultFontSize")]
		public float defaultFontSize { get; private set; } = 18.0f;

		public Vector2 screenScale
		{
			get
			{
				if (Screen.width > Screen.height)
					return new Vector2(Screen.width / 1280.0f, Screen.height / 720.0f);
				return new Vector2(Screen.width / 720.0f, Screen.height / 1280.0f);
			}
		}

		[ConsoleProperty(aliasName = "console.visible")]
		public bool visible
		{
			get { return mIsVisible && isActiveAndEnabled; }
			set
			{
				if (mIsVisible != value)
				{
					mIsVisible = value;
					if (mIsVisible)
					{
						OnPreviousCommand();
						mCurrentFocusName = msConsoleCommandInputName;
					}
				}
			}
		}

		[ConsoleProperty(aliasName = "console.autoVisibleIfError")]
		public bool autoVisibleIfError { get; set; } = false;

		public IModuleInterface module { get; set; }

		public string systemName { get { return msType.Name; } }

		public IEnumerator Initialize()
		{
			mManagedThreadId = Misc.GetCurrentManagedThreadID();
			Application.logMessageReceivedThreaded += OnLogMessageReceivedThread;
			yield break;
		}

		public IEnumerator PostInitialize()
		{


			return null;
		}

		public void Uninitialize()
		{
			mManagedThreadId = -1;
			Application.logMessageReceivedThreaded -= OnLogMessageReceivedThread;
		}

		private void OnLogMessageReceivedThread(string condition, string stackTrace, LogType type)
		{
			if (mManagedThreadId != Misc.GetCurrentManagedThreadID())
			{
				AddConsoleMessageThread(condition, stackTrace, type);
			}
			else
			{
				if (autoVisibleIfError && !visible)
				{
					visible = (type == LogType.Error) || (type == LogType.Assert) || (type == LogType.Exception);
				}
				AddConsoleMessage(condition, stackTrace, type);
			}
		}

		public void OnFixedUpdate(float fixedDeltaTime)
		{
		}

		public void OnLateUpdate()
		{
		}

		private void ProcessCurrentEvent()
		{
			Event e = Event.current;
			if (e == null)
				return;

			switch (e.keyCode)
			{
				case KeyCode.Return:
					{
						if (ProcessCommand())
						{
						}
						break;
					}
				case KeyCode.UpArrow:
					{
						if (OnPreviousCommand())
						{
						}
						break;
					}
				case KeyCode.DownArrow:
					{
						if (OnNextCommand())
						{
						}
						break;
					}
			}
		}

		public void OnUpdate(float deltaTime)
		{
			if (Input.GetKeyDown(KeyCode.BackQuote) || Input.touchCount > 4)
			{
				Toggle();
			}

			MergeMessageToMainThread();
		}

		public void Shutdown()
		{
		}

		public void Startup()
		{
		}

		public void UnregisterConsole(IConsoleObject o)
		{
			if (o == null)
				return;

			Type type = o.GetType();
			if (mConsoleInfos.TryGetValue(type, out var consoleInfo))
			{
				consoleInfo.Remove(o);
			}
		}

		private void DoRegister(IConsoleObject o)
		{
			Type type = o.GetType();

			if (!mConsoleInfos.TryGetValue(type, out var consoleInfo))
			{
				consoleInfo = new List<IConsoleObject>();
				mConsoleInfos.Add(type, consoleInfo);
			}

			consoleInfo.Union(o);

			if (mSearchedType.Add(type))
			{
				var reflectTypeInfo = GlobalReflectionCache.FindOrAdd(type);
				var methods = reflectTypeInfo.FindMethods<ConsoleMethodAttribute>();

				foreach (var m in methods)
				{
					var attr = m.GetCustomAttribute<ConsoleMethodAttribute>();
					MethodInfo validate = null;
					if (!string.IsNullOrEmpty(attr.validate))
					{
						validate = type.GetMethod(attr.validate);
					}

					if (validate != null)
					{
						if (validate.IsStatic != m.IsStatic)
						{
							DebugUtility.LogError(LoggerTags.Console, "The Method [{0}] and the validate [{1}] must be the same modifier (ex. static)", m.Name, validate.Name);
							continue;
						}

						if (validate.DeclaringType != typeof(bool))
						{
							DebugUtility.LogError(LoggerTags.Console, "The Validate [{0}] must return bool (true or false)", validate.Name);
							continue;
						}
					}

					string cmd = attr.aliasName;
					if (string.IsNullOrEmpty(cmd))
					{
						cmd = string.Concat(type.FullName, ".", m.Name);
					}

					if (!mCommandInfos.TryGetValue(cmd, out var cmdGroup))
					{
						cmdGroup = new List<ConsoleCommand>();
						mCommandInfos.Add(cmd, cmdGroup);
					}
					else
					{
						if (cmdGroup.FindIndex(command => command.owner == type) >= 0)
						{
							DebugUtility.LogError(LoggerTags.Engine, "The command is existed, it is not allowed : {0}", string.Concat(type.FullName, ".", m.Name));
							continue;
						}
					}

					ConsoleCommand entry;
					entry.owner = type;
					entry.attribute = attr;
					entry.validate = validate;
					entry.memberInfo = m;
					entry.cmd = cmd;
					cmdGroup.Add(entry);
				}

				var fieldsOrProperties = reflectTypeInfo.FindMembers<ConsoleMemberAttribute>();
				foreach (var m in fieldsOrProperties)
				{
					var attr = m.GetCustomAttribute<ConsoleMemberAttribute>();
					MethodInfo validate = null;
					if (!string.IsNullOrEmpty(attr.validate))
					{
						validate = type.GetMethod(attr.validate);
					}

					if (validate != null)
					{
						bool memberIsStatic = false;
						if (m.MemberType == MemberTypes.Field)
						{
							memberIsStatic = ((FieldInfo)m).IsStatic;
						}
						else if (m.MemberType == MemberTypes.Property)
						{
							memberIsStatic = ((PropertyInfo)m).IsStatic();
						}
						if (validate.IsStatic != memberIsStatic)
						{
							DebugUtility.LogError(LoggerTags.Console, "The Field [{0}] and the validate [{1}] must be the same modifier (ex. static)", m.Name, validate.Name);
							continue;
						}

						if (validate.DeclaringType != typeof(bool))
						{
							DebugUtility.LogError(LoggerTags.Console, "The Validate [{0}] must return bool (true or false)", validate.Name);
							continue;
						}
					}

					string cmd = attr.aliasName;
					if (string.IsNullOrEmpty(cmd))
					{
						cmd = string.Concat(type.FullName, ".", m.Name);
					}

					if (!mCommandInfos.TryGetValue(cmd, out var cmdGroup))
					{
						cmdGroup = new List<ConsoleCommand>();
						mCommandInfos.Add(cmd, cmdGroup);
					}
					else
					{
						if (cmdGroup.FindIndex(command => command.owner == type) >= 0)
						{
							DebugUtility.LogError(LoggerTags.Engine, "The command is existed, it is not allowed : {0}", string.Concat(type.FullName, ".", m.Name));
							continue;
						}
					}

					ConsoleCommand entry;
					entry.owner = type;
					entry.attribute = attr;
					entry.validate = validate;
					entry.memberInfo = m;
					entry.cmd = cmd;

					cmdGroup.Add(entry);
				}
			}
		}

		public void RegisterConsole(IConsoleObject o)
		{
			ProfilingUtility.BeginSample("RegisterConsole_", o.statID);
			DoRegister(o);
			ProfilingUtility.EndSample();
		}

		[InspectorMethod]
		public void Toggle()
		{
			visible = !visible;
		}

		[ConsoleMethod(aliasName = "console.clear")]
		public void ClearScreen()
		{
			mHistoryMessages.Clear();
		}

		public Rect CalcCmdInputRect(float width, float height)
		{
			return new Rect(0, 0, width, height);
		}

		private void AddConsoleMessage(ConsoleLog log)
		{
			mHistoryMessages.Add(log);
			if (mHistoryMessages.Count > msMaxHistoryMessageCount && msMaxHistoryMessageCount >= 0)
			{
				mHistoryMessages.RemoveAt(0);
			}
		}

		private void AddConsoleMessage(string condition, string stackTrace, LogType type)
		{
			AddConsoleMessage(new ConsoleLog() { msg = condition, stacktrace = TrimRedundancyInfo(stackTrace), logType = type });
		}

		private void AddConsoleMessageThread(string condition, string stackTrace, LogType type)
		{
			lock (mHistoryThreadMessageMute)
			{
				mHistoryThreadMessages.Add(new ConsoleLog() { msg = condition, stacktrace = TrimRedundancyInfo(stackTrace), logType = type });
			}
		}

		private static string TrimRedundancyInfo(string stackTrace)
		{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			return DebugUtility.TrimRedundancyInfo(stackTrace);
#else
			return stackTrace;
#endif
		}

		private void MergeMessageToMainThread()
		{
			lock (mHistoryThreadMessageMute)
			{
				foreach (var msg in mHistoryThreadMessages)
				{
					AddConsoleMessage(msg);
				}
				mHistoryThreadMessages.Clear();
			}
		}

		private bool OnPreviousCommand()
		{
			mPreviousCmdInversedIndex++;
			if (mPreviousCmds.Count <= 0 || mPreviousCmds.Count <= mPreviousCmdInversedIndex)
			{
				return false;
			}

			mPreviousCmdInversedIndex %= mPreviousCmds.Count;
			mCurrentCmd = mPreviousCmds[mPreviousCmds.Count - 1 - mPreviousCmdInversedIndex];
			return true;
		}

		private bool OnNextCommand()
		{
			if (mPreviousCmdInversedIndex <= 0)
				return false;

			mPreviousCmdInversedIndex--;
			if (mPreviousCmds.Count <= 0 || mPreviousCmds.Count <= mPreviousCmdInversedIndex)
			{
				return false;
			}
			if (mPreviousCmdInversedIndex < 0)
			{
				mPreviousCmdInversedIndex = 0;
			}
			mPreviousCmdInversedIndex %= mPreviousCmds.Count;
			mCurrentCmd = mPreviousCmds[mPreviousCmds.Count - 1 - mPreviousCmdInversedIndex];
			return true;
		}

		private string OnCommandRecord(string cmdWithParams)
		{
			string result = cmdWithParams;
			mCurrentCmd = "";
			mPreviousCmdInversedIndex = -1;
			mPreviousCmds.Add(result);
			if (mPreviousCmds.Count > msMaxHistoryCommandCount && msMaxHistoryCommandCount >= 0)
			{
				mPreviousCmds.RemoveAt(0);
			}
			return result;
		}

		private int DrawAutoCompletedList(Rect startPosition, int selected, List<GUIContent> contents, Rect rect, ref Vector2 scrollViewPosition)
		{
			scrollViewPosition = GUI.BeginScrollView(startPosition, scrollViewPosition, rect);



			GUI.EndScrollView();
			return selected;
		}

		private void OnGUI()
		{
			if (visible)
			{
				var scale = screenScale;
				var maxScaleValue = Mathf.Max(scale.x, scale.y);
				var minScaleValue = Mathf.Min(scale.x, scale.y);

				var fontSize = (int)(maxScaleValue * defaultFontSize);

				textAreaStyle.fontSize = fontSize;
				textFieldStyle.fontSize = fontSize;
				labelStyle.fontSize = fontSize;
				buttonStyle.fontSize = fontSize;

				var cmdHeight = msCMDHeight * maxScaleValue;
				var msgHeight = msMsgHeight * maxScaleValue;
				var execCMDSize = msExecCMDSize * maxScaleValue;

				GUI.Box(ApplicationUtility.halfScreenRect, TextureUtility.blackground);
				float cmdPosY = ApplicationUtility.halfScreenHeight - cmdHeight;
				Rect consoleViewPosition = RectUtility.Generate(ApplicationUtility.halfScreenWidth, cmdPosY);
				Rect consoleViewRect = RectUtility.Generate(ApplicationUtility.halfScreenWidth, mHistoryMessages.Count * (msgHeight + msMsgPadding));
				mConsoleHistoryPosition = GUI.BeginScrollView(consoleViewPosition, mConsoleHistoryPosition, consoleViewRect, true, true);
				var msgRect = consoleViewRect;
				msgRect.height = msgHeight;
				foreach (var msg in mHistoryMessages)
				{
					msg.DrawTextArea(msgRect, textAreaStyle);
					msgRect = msgRect.OffsetY(msgRect.height + msMsgPadding);
					//consoleViewRect = consoleViewRect.OffsetY(msgRect.height + msMsgPadding);
				}
				GUI.EndScrollView();

				float cmdPosX = 0.0f;
				if (GUI.Button(RectUtility.Generate(execCMDSize).Offset(cmdPosX, cmdPosY), "Exec", buttonStyle))
				{
					ProcessCommand();
				}

				cmdPosX += execCMDSize.x;
				float cmdWidth = ApplicationUtility.halfScreenWidth - cmdPosX;

				var inputRect = RectUtility.Generate(cmdWidth, cmdHeight).Offset(cmdPosX, cmdPosY);

				GUI.SetNextControlName(msConsoleCommandInputName);
				mCurrentCmd = GUI.TextField(inputRect, mCurrentCmd, textFieldStyle);
				mCurrentCmd = mCurrentCmd.TrimStart(msParamSeparators);

				ProcessCurrentEvent();

				if (!string.IsNullOrEmpty(mCurrentFocusName))
				{
					GUI.FocusControl(mCurrentFocusName);
					mCurrentFocusName = string.Empty;
				}
			}
		}

		private bool ProcessConsoleCommand(string cmdWithParams, string[] cmdWithArgs)
		{
			var command = cmdWithArgs[0];
			if (string.IsNullOrEmpty(command))
				return false;

			string name = string.Empty;
			bool exeResult = false;
			// support objectName->MethodCall arg0 arg1
			if (command.Contains("->"))
			{
				var nameAndCmd = command.Split(new[] { "->" }, StringSplitOptions.None);
				name = nameAndCmd[0];
				command = nameAndCmd[1];
			}
			if (mCommandInfos.TryGetValue(command, out var cmdGroup))
			{
				foreach (var cmdInfo in cmdGroup)
				{
					mConsoleInfos.TryGetValue(cmdInfo.owner, out var consoleObjects);
					if (!string.IsNullOrEmpty(name))
					{
						var result = consoleObjects.Find(o => o.name == name);
						consoleObjects.Clear();
						if (result != null)
						{
							consoleObjects.Add(result);
						}
					}
					var b = cmdInfo.Process(consoleObjects, cmdWithParams, cmdWithArgs);
					if (!exeResult)
					{
						exeResult = b;
					}
				}
			}
			return exeResult;
		}

		private bool Exec(string cmdWithParams)
		{
			OnCommandRecord(cmdWithParams);

			string[] cmdWithParamsArray = cmdWithParams.Split(msParamSeparators);
			if (cmdWithParamsArray.Length > 0)
			{
				return ProcessConsoleCommand(cmdWithParams, cmdWithParamsArray);
			}
			return false;
		}

		private bool ProcessCommand()
		{
			if (string.IsNullOrEmpty(mCurrentCmd))
			{
				return false;
			}

			var cmd = mCurrentCmd;
			if (!Exec(cmd))
			{
				DebugUtility.LogError(LoggerTags.Console, "Failure to exec: [{0}]", cmd);
			}
			else
			{
				DebugUtility.Log(LoggerTags.Console, "Success to exec: [{0}]", cmd);
			}
			return true;
		}
	}
}
