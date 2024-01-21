using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SystemCreator : Editor
{
    [MenuItem("System/CreateSystemAsset")]
    public static void CreateSystemAsset()
    {
        var systemAssembly = typeof(MonoSystem).Assembly;

        foreach (Type systemType in systemAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(MonoSystem))))
        {
            var systemAsset = ScriptableObject.CreateInstance(systemType);
            AssetDatabase.CreateAsset(systemAsset, AssetLoadHelper.GetSystemAssetPath(systemType));
        }
       
        AssetDatabase.Refresh();
    }
}
