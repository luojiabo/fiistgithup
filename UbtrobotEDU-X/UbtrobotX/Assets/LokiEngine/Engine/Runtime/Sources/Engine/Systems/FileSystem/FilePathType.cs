namespace Loki
{
	/// <summary>
	/// <see cref="https://docs.unity3d.com/ScriptReference/Application.html"/>
	/// </summary>
	public enum EFilePathType
	{
		/// <summary>
		/// Unity Editor: <path to project folder>/
		/// </summary>
		EditorProject,

		/// <summary>
		/// Contains the path to the game data folder (Read Only).
		///
		/// The value depends on which platform you are running on:
		///
		/// Unity Editor: <path to project folder>/Assets
		///
		/// Mac player: <path to player app bundle>/Contents
		///
		/// iOS player: <path to player app bundle>/<AppName.app>/Data (this folder is read only, use PersistentDataPath to save data).
		///
		/// Win/Linux player: <path to executablename_Data folder> (note that most Linux installations will be case-sensitive!)
		///
		/// WebGL: The absolute url to the player data file folder (without the actual data file name)
		///
		/// Android: Normally it would point directly to the APK. The exception is if you are running a split binary build in which case it points to the the OBB instead.
		///
		/// Windows Store Apps: The absolute path to the player data folder (this folder is read only, use PersistentDataPath to save data)
		/// 
		/// </summary>
		EditorAssetRoot,

		/// <summary>
		/// Unity Editor: <path to project folder>/Assets/Resources/Common/EngineGenerated/Configs
		/// </summary>
		EngineGeneratedConfigPath,

		/// <summary>
		/// Unity Editor: 
		/// The Default Resources point to <path to project folder>/Assets/Resources
		/// </summary>
		EngineDefaultResources,

		/// <summary>
		/// Unity Editor: 
		/// The Default Resources point to <path to project folder>/Assets/Editor/Resources
		/// </summary>
		EditorDefaultResources,

		/// <summary>
		/// Unity Editor: 
		/// The Resources point to <path to project folder>/Assets/StreamingAssets (Read/Write).
		/// 
		/// Unity combines most Assets into a Project when it builds the Project.
		/// However, it is sometimes useful to place files into the normal filesystem on the target machine to make them accessible via a pathname. 
		/// An example of this is the deployment of a movie file on iOS devices; 
		/// the original movie file must be available from a location in the filesystem to be played by the PlayMovie function.
		/// 
		/// Any files placed in a folder called StreamingAssets (case-sensitive) in a Unity project will be copied verbatim to a particular folder
		/// on the target machine. You can retrieve the folder using the Application.streamingAssetsPath property. 
		/// It's always best to use Application.streamingAssetsPath to get the location of the StreamingAssets folder, 
		/// as it will always point to the correct location on the platform where the application is running.
		///
		/// The location returned by Application.streamingAssetsPath varies per platform:
		/// Most platforms (Unity Editor, Windows, macOS, Linux players, PS4, Xbox One, Switch) 
		/// use Application.dataPath + "/StreamingAssets", iOS uses Application.dataPath + "/Raw", 
		/// Android uses files inside a compressed APK/JAR file, "jar:file://" + Application.dataPath + "!/assets". 
		/// To read streaming Assets on platforms like Android and WebGL , where you cannot access streaming Asset files directly, use UnityWebRequest. 
		/// For an example, see Application.streamingAssetsPath.
		/// 
		/// <seealso cref="https://docs.unity3d.com/Manual/StreamingAssets.html"/>
		/// </summary>
		StreamingAssets,

		/// <summary>
		/// Contains the path to a persistent data directory (Read Only).
		/// 
		/// This value is a directory path where you can store data that you want to be kept between runs. When you publish on iOS and Android, persistentDataPath points to 
		/// a public directory on the device. Files in this location are not erased by app updates. The files can still be erased by users directly.
		/// 
		/// When you build the Unity application, a GUID is generated that is based on the Bundle Identifier. This GUID is part of persistentDataPath. If you keep the same 
		/// Bundle Identifier in future versions, the application keeps accessing the same location on every update.
		/// 
		/// Windows Store Apps: PersistentDataPath points to %userprofile%/AppData/Local/Packages/<productname>/LocalState.
		/// 
		/// iOS: PersistentDataPath points to /var/mobile/Containers/Data/Application/<guid>/Documents.
		/// 
		/// Android: PersistentDataPath points to /storage/emulated/0/Android/data/<packagename>/files on most devices 
		/// (some older phones might point to location on SD card if present), the path is resolved using android.content.Context.getExternalFilesDir.
		/// 
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html"/>
		/// </summary>
		PersistentDataPath,
	}

}
