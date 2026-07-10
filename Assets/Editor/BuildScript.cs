using UnityEditor;
using System;

public static class BuildScript
{
    public static void PerformBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new();
        buildPlayerOptions.scenes = new[] {"Assets/Scenes/NewTestScene.unity"};
        buildPlayerOptions.locationPathName = "Build/PC/Encounter_NightDance.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if(report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new Exception($"빌드에 실패했습니다.\n{report.summary.result}");
        }
    }
}
