namespace Loki
{
	[System.Flags]
	public enum EAssetPackageType
	{
		/// <summary>
		/// Load by path analyzer
		/// </summary>
		Auto = 0,
		/// <summary>
		/// Recommend, use "Assets/Directory/file" as the path.
		/// </summary>
		FileSystem = 0x1,
		/// <summary>
		/// The Unity - Resources.Load() etc., USE LESS
		/// </summary>
		Resources = 0x2,
		/// <summary>
		/// The Unity - StreamingAssets, Just store the Zip, It's slowly, DO NOT USE Directly.
		/// </summary>
		StreamingAssets = 0x4,
	}

	/// <summary>
	/// The asset pool type
	/// </summary>
	public enum EAssetPoolType
	{
		Default,

	}
}
