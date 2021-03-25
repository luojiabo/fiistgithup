#if UNITY_WEBRTC && (!UNITY_WEBGL || UNITY_EDITOR)
using System;
using UnityEngine;
using Unity.WebRTC;
using System.Collections;
using System.Threading.Tasks;

namespace Loki
{
	class WebRTCBehaviour
	{
		public static bool hasInitialized { get; private set; }

		public static void Initialize()
		{
			if (hasInitialized) return;
			hasInitialized = true;
			WebRTC.Initialize(WebRTC.SupportHardwareEncoder ? EncoderType.Hardware : EncoderType.Software);
		}

		public static void UnInitialize()
		{
			if (!hasInitialized) return;
			hasInitialized = false;
			WebRTC.Finalize();
		}
	}

	public class WebRTCClient : MonoBehaviour, IWebSocketClient
	{
		private bool mDisposed = false;
		private string mHost;

		private RTCPeerConnection mPeer = null;
		private RTCDataChannel mSendDataChannel = null;
		private RTCDataChannel mRecvDataChannel = null;
		private Coroutine mCurrentTask = null;

		public string host => mHost;

		public ENetState state => throw new NotImplementedException();

		public Action onConnected { get; set; }
		public Action<ENetCode> onDisconnected { get; set; }
		public Action<byte[]> onRecv { get; set; }
		public Action<string> onError { get; set; }

		public Coroutine currentTask
		{
			get { return mCurrentTask; }
			set
			{
				if (mCurrentTask != null)
				{
					StopCoroutine(mCurrentTask);
					mCurrentTask = null;
				}
			}
		}

		public WebRTCClient()
		{
			WebRTCBehaviour.Initialize();

		}

		public void Connect(string url)
		{
			if (!string.IsNullOrEmpty(mHost))
				throw new Exception("The host has connected. Please disconnect at first.");

			mHost = url;

			var conf = GetSelectedSdpSemantics();
			mPeer = new RTCPeerConnection(ref conf);
			mPeer.OnIceCandidate = OnIceCandidate;
			mPeer.OnIceConnectionChange = OnIceConnectionChange;
			mPeer.OnDataChannel = OnDataChannel;

			DebugUtility.Log(LoggerTags.Online, "Create peer connection : {0}", host);

			var init = new RTCDataChannelInit(true);
			mSendDataChannel = mPeer.CreateDataChannel("data", ref init);
			mSendDataChannel.OnOpen = OnDataChannelOpen;
			mSendDataChannel.OnClose = OnDataChannelClose;

			currentTask = StartCoroutine(StartCreationWorkflow(mPeer));
		}

		private void OnSetSessionDescriptionError(ref RTCError error, string func)
		{
			DebugUtility.LogTrace(LoggerTags.Online, "Failure to call {0}. Detail : {1}, HTTP state code : {2}", func, error.errorDetail, error.httpRequestStatusCode);
		}

		private IEnumerator StartCreationWorkflow(RTCPeerConnection peer)
		{
			var offerOptions = new RTCOfferOptions
			{
				iceRestart = false,
				offerToReceiveAudio = true,
				offerToReceiveVideo = false
			};

			var op1 = peer.CreateOffer(ref offerOptions);
			yield return op1;
			if (op1.isError)
			{
				OnSetSessionDescriptionError(ref op1.error, "CreateOffer");
				yield break;
			}

			var op1desc = op1.desc;
			DebugUtility.Log(LoggerTags.Online, "Success to call CreateOffer. DESC.type : {0}, DESC.sdp : {0}", op1desc.type, op1desc.sdp);
			var op2 = peer.SetLocalDescription(ref op1desc);
			yield return op2;
			if (op2.isError)
			{
				OnSetSessionDescriptionError(ref op2.error, "SetLocalDescription");
				yield break;
			}

			DebugUtility.Log(LoggerTags.Online, "Success to call SetLocalDescription.");
			var op3 = peer.SetRemoteDescription(ref op1desc);
			yield return op3;
			if (op3.isError)
			{
				OnSetSessionDescriptionError(ref op3.error, "SetRemoteDescription");
				yield break;
			}

			DebugUtility.Log(LoggerTags.Online, "Success to call SetRemoteDescription.");
			RTCAnswerOptions answerOptions = new RTCAnswerOptions { iceRestart = false, };
			var op4 = peer.CreateAnswer(ref answerOptions);
			if (op4.isError)
			{
				OnSetSessionDescriptionError(ref op4.error, "CreateAnswer");
				yield break;
			}

			var op4desc = op4.desc;
			DebugUtility.Log(LoggerTags.Online, "Success to call CreateAnswer. DESC.type : {0}, DESC.sdp : {0}", op4desc.type, op4desc.sdp);
			var op5 = peer.SetLocalDescription(ref op4desc);
			if (op5.isError)
			{
				OnSetSessionDescriptionError(ref op4.error, "SetLocalDescription");
				yield break;
			}

			DebugUtility.Log(LoggerTags.Online, "Success to call SetLocalDescription.");
			var op6 = peer.SetRemoteDescription(ref op4desc);
			if (op6.isError)
			{
				OnSetSessionDescriptionError(ref op4.error, "SetRemoteDescription");
				yield break;
			}
			DebugUtility.Log(LoggerTags.Online, "Success to call SetRemoteDescription.");

		}

		private void OnDataChannel(RTCDataChannel channel)
		{
			mRecvDataChannel = channel;
			mRecvDataChannel.OnMessage = OnMessage;
		}

		private void OnDataChannelOpen()
		{
			DebugUtility.LogTrace(LoggerTags.Online, "{0} OnDataChannelOpen : {1}", host, state);
		}

		private void OnDataChannelClose()
		{
			DebugUtility.LogTrace(LoggerTags.Online, "{0} OnDataChannelClose : {1}", host, state);
		}

		public void Disconnect()
		{
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (mDisposed)
			{
				return;
			}
			mDisposed = true;
			if (disposing)
			{
				onRecv = null;
				onError = null;
				onConnected = null;
				onDisconnected = null;

				Disconnect();
			}
		}

		protected void OnConnected()
		{
			DebugUtility.LogTrace(LoggerTags.Online, "{0} Connected : {1}", host, state);

			if (onConnected != null)
			{
				onConnected();
			}
		}

		protected void OnDisconnected(int error)
		{
			DebugUtility.LogTrace(LoggerTags.Online, "{0} Disconnected : {1}", host, error);

			if (onDisconnected != null)
			{
				onDisconnected(0);
			}
		}

		public void SendMessage(byte[] datas)
		{
			mSendDataChannel.Send(datas);
		}

		protected void OnMessage(byte[] msg)
		{
			if (onRecv != null)
			{
				onRecv(msg);
			}
		}

		protected void OnError(string errMsg)
		{
			DebugUtility.LogErrorTrace(LoggerTags.Online, "{0} Error : {1}, State : {2}", host, errMsg, state);

			if (onError != null)
			{
				onError(errMsg);
			}
		}

		private RTCConfiguration GetSelectedSdpSemantics()
		{
			RTCConfiguration config = default;
			config.iceServers = new RTCIceServer[]
			{
				new RTCIceServer { urls = new string[] { host } }
			};
			return config;
		}

		private void OnIceConnectionChange(RTCIceConnectionState state)
		{
			switch (state)
			{
				case RTCIceConnectionState.New:
					DebugUtility.Log(LoggerTags.Online, "{0} IceConnectionState: New", host);
					break;
				case RTCIceConnectionState.Checking:
					DebugUtility.Log(LoggerTags.Online, "{0} IceConnectionState: Checking", host);
					break;
				case RTCIceConnectionState.Closed:
					DebugUtility.Log(LoggerTags.Online, "{0} IceConnectionState: Closed", host);
					break;
				case RTCIceConnectionState.Completed:
					DebugUtility.Log(LoggerTags.Online, "{0} IceConnectionState: Completed", host);
					break;
				case RTCIceConnectionState.Connected:
					DebugUtility.Log(LoggerTags.Online, "{0} IceConnectionState: Connected", host);
					break;
				case RTCIceConnectionState.Disconnected:
					DebugUtility.Log(LoggerTags.Online, "{0} IceConnectionState: Disconnected", host);
					break;
				case RTCIceConnectionState.Failed:
					DebugUtility.Log(LoggerTags.Online, "{0} IceConnectionState: Failed", host);
					break;
				case RTCIceConnectionState.Max:
					DebugUtility.Log(LoggerTags.Online, "{0} IceConnectionState: Max", host);
					break;
				default:
					break;
			}
		}

		private void OnIceCandidate(RTCIceCandidate​ e)
		{
			if (!string.IsNullOrEmpty(e.candidate))
			{
				mPeer.AddIceCandidate(ref e);
				DebugUtility.Log(LoggerTags.Online, "{0} ICE candidate:\n {1}", host, e.candidate);
			}
			else
			{
				DebugUtility.Log(LoggerTags.Online, "{0} ICE empty candidate:\n", host);
			}
		}
	}
}
#endif
