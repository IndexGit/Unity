using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    static readonly string[] Scenes = 
        {
            "Assets/Scenes/Begin.unity",
            "Assets/Scenes/Game.unity",
        };
    static string[] Scenes_
    {
        get
        {
            var r = Directory.GetFiles(@".\Assets\Scenes", "*.unity",SearchOption.TopDirectoryOnly);
            return r.Select(f => $"Assets/Scenes/{Path.GetFileName(f)}").ToArray();
        }
    }

    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        BuildWindowsServer();
        //        BuildLinuxServer();
        BuildWindowsClient();
    }

    [MenuItem("Build/Build Server (Windows)")]
    public static void BuildWindowsServer()
    {
        Debug.Log("Building Server (Windows)...");
        Debug.Log($"Scenes:{string.Join(", ", Scenes)}");
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = "Build_Server/NetworkDragAndDrop_Server.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.EnableHeadlessMode //BuildOptions.CompressWithLz4HC |
        };

        Console.WriteLine("Building Server (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Server (Windows).");
    }

    [MenuItem("Build/Build Client (Windows)")]
    public static void BuildWindowsClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = "Build_Client/NetworkDragAndDrop_Client.exe",
            target = BuildTarget.StandaloneWindows64,
            //options = BuildOptions.CompressWithLz4HC
        };

        Console.WriteLine("Building Client (Windows)...");
        //EditorUtility.DisplayProgressBar();
        var r = BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Client (Windows).");
    }

    [MenuItem("Build/Build WebGL Client")]
    public static void BuildWebGlClient()
    {
        Debug.Log("Building WebGL Client...");
        Debug.Log($"Scenes:{string.Join(", ", Scenes)}");
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = "Build_Web/",
            target = BuildTarget.WebGL,
            //options = BuildOptions.EnableHeadlessMode //BuildOptions.CompressWithLz4HC |
        };

        Console.WriteLine("Building WebGL Client...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built WebGL Client.");
    }
}
