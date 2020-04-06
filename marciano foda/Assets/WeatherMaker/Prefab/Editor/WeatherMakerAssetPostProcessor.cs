/*
This one source file is MIT license.

Copyright 2019 Digital Ruby, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Pre-process define utility
    /// </summary>
    public class WeatherMakerAssetPostprocessor : AssetPostprocessor
    {
        // add list of asset files to key off of along with pre-processor definition
        // asset name / pre processor value key / value pair
        internal static readonly KeyValuePair<string, string>[] preProcessors = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("/WeatherMaker/Prefab/Scripts/Manager/WeatherMakerScript.cs", "WEATHER_MAKER_PRESENT"),
            new KeyValuePair<string, string>("/Mirror/Runtime/NetworkIdentity.cs", "MIRROR_NETWORKING_PRESENT"),
            new KeyValuePair<string, string>("/Scripts/CompleteTerrainShader.cs", "CTS_PRESENT")
        };

        /// <summary>
        /// Update pre-processor
        /// </summary>
        /// <param name="hasAsset">Whether the asset exists, if it does pre-processor is added, else it is removed if it exists</param>
        /// <param name="preProcessor">The pre-processor to add or remove</param>
        internal static void UpdatePreProcessor(bool hasAsset, string preProcessor)
        {
            string currBuildSettings = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);
            int currBuildSettingsLength = currBuildSettings.Length;
            if (hasAsset)
            {
                if (!currBuildSettings.Contains(preProcessor))
                {
                    currBuildSettings += ";" + preProcessor;
                }
            }
            else
            {
                currBuildSettings = System.Text.RegularExpressions.Regex.Replace(currBuildSettings, ";?" + preProcessor + ";?", ";");
            }
            currBuildSettings = (currBuildSettings ?? string.Empty).Trim().Trim(';');
            if (currBuildSettingsLength != currBuildSettings.Length)
            {
                Debug.LogWarning("Updating preprocessor to " + currBuildSettings);
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings.Trim(';'));
            }
        }

        internal static void ProcessImportedAssets(params string[] importedAssets)
        {
            if (importedAssets == null)
            {
                return;
            }

            foreach (string assetName in importedAssets)
            {
                foreach (var kv in preProcessors)
                {
                    if (assetName.EndsWith(kv.Key, System.StringComparison.OrdinalIgnoreCase))
                    {
                        UpdatePreProcessor(true, kv.Value);
                        break;
                    }
                }
            }
        }

        internal static void ProcessDeletedAssets(params string[] deletedAssets)
        {
            if (deletedAssets == null)
            {
                return;
            }

            foreach (string assetName in deletedAssets)
            {
                // this only gets called for the directory on deletion, not each file in the directory, so we have to manually scan the dir
                // before it is deleted
                if (Directory.Exists(assetName))
                {
                    foreach (string file in Directory.GetFiles(assetName, "*", SearchOption.AllDirectories))
                    {
                        string normFile = file.Replace("\\", "/");
                        foreach (var kv in WeatherMakerAssetPostprocessor.preProcessors)
                        {
                            if (normFile.EndsWith(kv.Key, System.StringComparison.OrdinalIgnoreCase))
                            {
                                WeatherMakerAssetPostprocessor.UpdatePreProcessor(false, kv.Value);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var kv in WeatherMakerAssetPostprocessor.preProcessors)
                    {
                        if (assetName.EndsWith(kv.Key, System.StringComparison.OrdinalIgnoreCase))
                        {
                            WeatherMakerAssetPostprocessor.UpdatePreProcessor(false, kv.Value);
                            break;
                        }
                    }
                }
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            ProcessImportedAssets(importedAssets);
            ProcessDeletedAssets(deletedAssets);
        }

        private static void CleanupOldFiles()
        {
            string[] found = AssetDatabase.FindAssets("WeatherMakerAuroraShader");
            foreach (string item in found)
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                if (path.EndsWith("Prefab/Shaders/WeatherMakerAuroraShader.cginc", System.StringComparison.OrdinalIgnoreCase))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }

        private static void UpdatePostProcessorFromExistingAssets()
        {
            foreach (var kv in preProcessors)
            {
                string[] found = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(kv.Key));
                if (found != null && found.Length > 0)
                {
                    for (int i = 0; i < found.Length; i++)
                    {
                        found[i] = AssetDatabase.GUIDToAssetPath(found[i]);
                    }
                    OnPostprocessAllAssets(found, new string[0], new string[0], new string[0]);
                }
                else
                {
                    OnPostprocessAllAssets(new string[0], new string[] { kv.Key }, new string[0], new string[0]);
                }
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        [InitializeOnLoadMethod]
        private static void DidReloadScripts()
        {
            // executes whenever code finishes compiling

            CleanupOldFiles();
            UpdatePostProcessorFromExistingAssets();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
    }

    public class WeatherMakerAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        private static void OnWillCreateAsset(string assetName)
        {
            WeatherMakerAssetPostprocessor.ProcessImportedAssets(assetName);
        }

        private static AssetDeleteResult OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
        {
            WeatherMakerAssetPostprocessor.ProcessDeletedAssets(assetName);
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
