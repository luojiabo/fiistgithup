namespace Ubtrobot
{
    public enum CallPlatformMethod
    {
        Unknown,

        bridge,
        startResult,
        getAppInfo,
        getConfigs,
        showLoading,
        hideLoading,
        startCode,
        setTitle,

		connectAsync,
		postMessageAsync,
		disconnectAsync,

		setFullScreen,
		startLaunching,
		stopLaunching,
	}

	public enum CallUnityMethod
    {
        Unknown,
        onBackPressed,
        startup,
        shutdown,

		onRecv,
		requestSupportCmdInfo,
	}
}
