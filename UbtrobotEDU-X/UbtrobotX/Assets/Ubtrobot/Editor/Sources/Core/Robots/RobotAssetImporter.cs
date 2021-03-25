using UnityEditor;

namespace Loki
{
	class RobotAssetImporter : AssetPostprocessor
	{
		private void OnPreprocessModel()
		{
			var importer = assetImporter as ModelImporter;
			OnFBXImport(importer);
		}

		private static void OnFBXImport(ModelImporter importer)
		{
			if (importer.animationType == ModelImporterAnimationType.Legacy) return;
			importer.importBlendShapes = false;
			importer.importVisibility = false;
			importer.importCameras = false;
			importer.importLights = false;
			importer.animationType = ModelImporterAnimationType.Legacy;
			importer.SaveAndReimport();
		}
	}
}
