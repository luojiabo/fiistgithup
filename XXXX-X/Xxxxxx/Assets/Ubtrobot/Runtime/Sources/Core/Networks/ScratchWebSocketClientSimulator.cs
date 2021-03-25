using System;
using Loki;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ubtrobot
{
	[Serializable]
	public struct ProtocolValue
	{
		public TextAsset Key;
		public float Value;
	}

	public class ScratchWebSocketClientSimulator : WebSocketClientSimulator
	{
		public ProtocolValue[] sendContents;
		public ProtocolValue[] recvContents;

		[ConsoleField(aliasName = "net.debug")]
		public bool debug = false;
		private bool mDebugBreakpoint = false;

		public void Reset()
		{
			name = "WebSocketClientSimulator";
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "net.stopDebug")]
		public void StopDebug()
		{
			StopAllCoroutines();
			if (RobotManager.GetOrAlloc())
			{
				RobotManager.GetOrAlloc().RewindRobots();
			}
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "net.simSendAll")]
		public void SimulateSendAllMessages()
		{
			StopDebug();
			if (sendContents == null || sendContents.Length == 0)
			{
				DebugUtility.LogError(LoggerTags.Online, "Can't send empty message.");
				return;
			}
			StartCoroutine(SimulateSendAllMessagesCo());
		}

		private IEnumerator SimulateSendAllMessagesCo()
		{
			foreach (var kv in sendContents)
			{
				string text = kv.Key.text;
				if (string.IsNullOrEmpty(text))
				{
					DebugUtility.LogError(LoggerTags.Online, "Can't send empty message.");
					continue;
				}
				SendMessage(Encoding.UTF8.GetBytes(text));
				if (kv.Value > 0.0f)
				{
					yield return new WaitForSeconds(kv.Value);
				}
				else
				{
					yield return null;
				}
			}
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "net.simRecvAll")]
		public void SimulateRecvAllMessages()
		{
			StopDebug();

			if (recvContents == null || recvContents.Length == 0)
			{
				DebugUtility.LogError(LoggerTags.Online, "Can't recv empty message.");
				return;
			}
			StartCoroutine(SimulateRecvAllMessagesCo());
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "net.bp")]
		public void SetBreakPoint()
		{
			mDebugBreakpoint = true;
		}

		[InspectorMethod]
		[ConsoleMethod(aliasName = "net.ns")]
		public void SetNextStep()
		{
			mDebugBreakpoint = false;
		}

#if UNITY_EDITOR
		[InspectorMethod]
		public void ScanProtocols()
		{
			string robotName = transform.name.Split('_')[0];
			string path = Application.dataPath+ "/Projects/ScratchProtocols/" + robotName;

			if (!Directory.Exists(path))
			{
				return;
			}

			DirectoryInfo dir = new DirectoryInfo(path);
			var files = dir.GetFiles("*.txt");
			if (files.Length > 0)
			{
				recvContents = new ProtocolValue[files.Length];
				
				for (int i = 0; i < files.Length; i++)
				{
					string assetPath = files[i].FullName.ToUNIXStyle().Replace(FileSystem.Get().projectPathWithAlt, string.Empty);
					recvContents[i].Key = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
				}
			}
		}
#endif
		[InspectorMethod(category = "Test")]
		public void StartTest()
		{
			RobotManager.GetOrAlloc().Test(true);
		}

		[InspectorMethod(category = "Test")]
		public void StopTest()
		{
			RobotManager.GetOrAlloc().Test(false);
		}

		public IEnumerator SimulateRecvAllMessagesCo()
		{
			foreach (var kv in recvContents)
			{
				string text = kv.Key.text;
				if (string.IsNullOrEmpty(text))
				{
					DebugUtility.LogError(LoggerTags.Online, "Can't recv empty message.");
					continue;
				}
				OnMessage(Encoding.UTF8.GetBytes(text));
				if (kv.Value > 0.0f)
				{
					yield return new WaitForSeconds(kv.Value);
				}
				else
				{
					yield return null;
				}
				if (debug)
				{
					SetBreakPoint();
				}
				while (mDebugBreakpoint)
				{
					yield return null;
				}
			}
		}

		public IEnumerator SimulateRecvAllMessagesCo(Func<int, bool> condition, Action<int> beforeMessage)
		{
			int index = -1;
			foreach (var kv in recvContents)
			{
				++index;
				string text = kv.Key.text;
				if (string.IsNullOrEmpty(text))
				{
					DebugUtility.LogError(LoggerTags.Online, "Can't recv empty message.");
					continue;
				}
				if (beforeMessage != null)
				{
					beforeMessage(index);
				}
				OnMessage(Encoding.UTF8.GetBytes(text));
				if (kv.Value > 0.0f)
				{
					yield return new WaitForSeconds(kv.Value);
				}
				else
				{
					yield return null;
				}
				if (condition != null)
				{
					while (!condition(index))
					{
						yield return null;
					}
				}
				if (debug)
				{
					SetBreakPoint();
				}
				while (mDebugBreakpoint)
				{
					yield return null;
				}
			}
		}
	}
}
