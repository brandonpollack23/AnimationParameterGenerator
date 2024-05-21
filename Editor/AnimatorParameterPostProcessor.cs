using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorParameterPostprocessor : AssetPostprocessor
{
  // This method is called whenever assets are imported, deleted, or moved
  static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
  {
    // Check if any imported or moved assets are AnimatorControllers
    foreach (string assetPath in importedAssets)
    {
      if (assetPath.EndsWith(".controller"))
      {
        AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
        AnimatorParameterGenerator.Generate();
      }
    }

    // Check if any deleted assets are AnimatorControllers
    foreach (string assetPath in deletedAssets)
    {
      if (assetPath.EndsWith(".controller"))
      {
        string file = AssetPathToGeneratedParametersClass(assetPath);
        System.IO.File.Delete(file);
      }
    }

    movedAssets.Zip(movedFromAssetPaths, (movedAsset, movedFromAssetPath) =>
    {
      if (movedAsset.EndsWith(".controller"))
      {
        // Move the generated parameter file to the new location
        string fromFile = AssetPathToGeneratedParametersClass(movedFromAssetPath);
        string toFile = AssetPathToGeneratedParametersClass(movedAsset);
        System.IO.File.Move(fromFile, toFile);
      }
      return 0;
    });
  }

  private static string AssetPathToGeneratedParametersClass(string assetPath)
  {
    string filePath = System.IO.Directory.GetParent(assetPath).FullName;
    string className = Path.GetFileNameWithoutExtension(assetPath) + "Parameters";
    return Path.Combine(filePath, className + ".cs");
  }
}
