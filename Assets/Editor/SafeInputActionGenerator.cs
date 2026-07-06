#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Editor;

public class SafeInputActionGenerator : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.EndsWith(".inputactions"))
            {
                if (assetPath.Contains("MainAction.inputactions"))
                {
                    string wrapperFilePath = "Assets/Input/MainAction.cs";
                    
                    var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(assetPath);
                    if (asset == null) return;

                    var options = new InputActionCodeGenerator.Options
                    {
                        sourceAssetPath = assetPath,
                        namespaceName = "",
                        className = "MainAction",
                    };

                    if (InputActionCodeGenerator.GenerateWrapperCode(wrapperFilePath, asset, options))
                    {
                        // 임포트 프로세스가 완전히 끝난 다음 프레임(delayCall)에 안전하게 C# 파일을 임포트하여 컴파일을 트리거합니다.
                        EditorApplication.delayCall += () =>
                        {
                            AssetDatabase.ImportAsset(wrapperFilePath);
                        };
                    }
                }
            }
        }
    }
}
#endif
