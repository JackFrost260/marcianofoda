using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BayatGames.SaveGamePro.Utilities
{

    /// <summary>
    /// Provides utility methods for handling paths.
    /// </summary>
    public static class PathUtils
    {

        /// <summary>
        /// Gets the relative path to the project assets folder path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetAssetsRelativePath(string path)
        {
            if (path.StartsWith(Application.dataPath))
            {
                return "Assets" + path.Substring(Application.dataPath.Length);
            }
            return path;
        }

        /// <summary>
        /// Gets the relative path to the specified directory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string GetRelativePath(string path, string directory)
        {
            Uri pathUri = new Uri(path);

            // Folders must end in a slash
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                directory += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(directory);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

    }

}