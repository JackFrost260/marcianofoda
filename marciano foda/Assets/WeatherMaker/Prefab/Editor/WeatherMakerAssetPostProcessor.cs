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
using System.Reflection;
using System;
using System.Linq;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Pre-process define utility
    /// </summary>
    public class WeatherMakerPreProcessorManager : AssetPostprocessor
    {
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

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (deletedAssets != null && deletedAssets.Length != 0)
            {
                int scriptCount = deletedAssets.Count(a => a.EndsWith(".cs", StringComparison.OrdinalIgnoreCase));
                if (scriptCount > 0)
                {
                    Debug.LogWarningFormat("{0} script{1} been deleted from the project. If you see compile errors, please edit your player settings, scripting define symbols and remove any defines that no longer exist in the project.",
                        scriptCount, (scriptCount > 1 ? "s have" : " has"));
                }
            }
            DidReloadScripts();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        [UnityEditor.InitializeOnLoadMethod]
        private static void DidReloadScripts()
        {
            // executes on first load or whenever code finishes compiling

            CleanupOldFiles();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

            bool hasWeatherMaker = false;
            bool hasLWRP = false;
            bool hasPostProcessV2 = false;
            bool hasPlaymaker = false;
            bool hasMirror = false;
            bool hasCts = false;
            bool hasCrest = false;
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in a.GetTypes())
                {
                    switch (type.FullName.ToLowerInvariant())
                    {
                        case "digitalruby.weathermaker.weathermakerscript":
                            hasWeatherMaker = true;
                            break;

                        case "unityengine.rendering.lwrp.lightweightrenderpipeline":
                            hasLWRP = true;
                            break;

                        case "unityengine.rendering.postprocessing.postprocesslayer":
                            hasPostProcessV2 = true;
                            break;

                        case "hutonggames.playmaker.fsmprocessor":
                            hasPlaymaker = true;
                            break;

                        case "mirror.networkidentity":
                            hasMirror = true;
                            break;

                        case "cts.completeterrainshader":
                            hasCts = true;
                            break;

                        case "crest.oceanrenderer":
                            hasCrest = true;
                            break;
                    }
                }
            }
            UpdatePreProcessor(hasWeatherMaker, "WEATHER_MAKER_PRESENT");
            UpdatePreProcessor(hasLWRP, "UNITY_LWRP");
            UpdatePreProcessor(hasPostProcessV2, "UNITY_POST_PROCESSING_STACK_V2");
            UpdatePreProcessor(hasPlaymaker, "PLAYMAKER_PRESENT");
            UpdatePreProcessor(hasMirror, "MIRROR_NETWORKING_PRESENT");
            UpdatePreProcessor(hasCts, "CTS_PRESENT");
            UpdatePreProcessor(hasCrest, "CREST_OCEAN_PRESENT");
        }
    }
}
