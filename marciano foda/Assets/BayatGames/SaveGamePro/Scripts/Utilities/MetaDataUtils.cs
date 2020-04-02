using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

using BayatGames.SaveGamePro.IO;
using BayatGames.SaveGamePro.Reflection;
using BayatGames.SaveGamePro.Serialization.Formatters.Json;
using BayatGames.SaveGamePro.Utilities;

namespace BayatGames.SaveGamePro.Utilities
{

    /// <summary>
    /// Provides utility methods for managing saved entities meta data.
    /// </summary>
    public static class MetaDataUtils
    {

        /// <summary>
        /// Saves the specified identifier meta data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="metaData"></param>
        public static void SaveMetaData(string identifier, MetaData metaData)
        {
            SaveMetaData(identifier, metaData, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Saves the specified identifier meta data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="metaData"></param>
        /// <param name="settings"></param>
        public static void SaveMetaData(string identifier, MetaData metaData, SaveGameSettings settings)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException("identifier");
            }
            string metaIdentifier = GetMetaIdentifier(identifier);
            settings.Identifier = metaIdentifier;
            settings.Storage.OnSave(settings);
            Stream stream = settings.Storage.GetWriteStream(settings);
            JsonFormatter formatter = new JsonFormatter(settings);
            formatter.Serialize(stream, metaData);
            settings.Storage.OnSaved(settings);
            stream.Dispose();
        }

        /// <summary>
        /// Loads the specified identifier meta data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static MetaData LoadMetaData(string identifier)
        {
            return LoadMetaData(identifier, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Loads the specified identifier meta data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static MetaData LoadMetaData(string identifier, SaveGameSettings settings)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException("identifier");
            }
            if (!SaveGame.Exists(identifier, settings))
            {
                return null;
            }
            string metaIdentifier = GetMetaIdentifier(identifier);
            settings.Identifier = metaIdentifier;
            if (!SaveGame.Exists(metaIdentifier, settings))
            {
                return null;
            }
            settings.Storage.OnLoad(settings);
            MetaData metaData = null;
            Stream stream = settings.Storage.GetReadStream(settings);
            JsonFormatter formatter = new JsonFormatter(settings);
            metaData = formatter.Deserialize<MetaData>(stream);
            settings.Storage.OnLoaded(settings);
            stream.Dispose();
            return metaData;
        }

        /// <summary>
        /// Checks whether the specified identifier has any meta data or not.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static bool HasMetaData(string identifier)
        {
            return HasMetaData(identifier, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Checks whether the specified identifier has any meta data or not.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static bool HasMetaData(string identifier, SaveGameSettings settings)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException("identifier");
            }
            string metaIdentifier = GetMetaIdentifier(identifier);
            settings.Identifier = metaIdentifier;
            return settings.Storage.Exists(settings);
        }

        /// <summary>
        /// Deletes the specified identifier meta data.
        /// </summary>
        /// <param name="identifier"></param>
        public static void DeleteMetaData(string identifier)
        {
            DeleteMetaData(identifier, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Deletes the specified identifier meta data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="settings"></param>
        public static void DeleteMetaData(string identifier, SaveGameSettings settings)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException("identifier");
            }
            string metaIdentifier = GetMetaIdentifier(identifier);
            settings.Identifier = metaIdentifier;
            settings.Storage.Delete(settings);
        }

        /// <summary>
        /// Copies the meta data from to location.
        /// </summary>
        /// <param name="fromIdentifier"></param>
        /// <param name="toIdentifier"></param>
        public static void CopyMetaData(string fromIdentifier, string toIdentifier)
        {
            CopyMetaData(fromIdentifier, toIdentifier, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Copies the meta data from to location.
        /// </summary>
        /// <param name="fromIdentifier"></param>
        /// <param name="toIdentifier"></param>
        /// <param name="settings"></param>
        public static void CopyMetaData(string fromIdentifier, string toIdentifier, SaveGameSettings settings)
        {
            if (string.IsNullOrEmpty(fromIdentifier))
            {
                throw new ArgumentNullException("fromIdentifier");
            }
            if (string.IsNullOrEmpty(fromIdentifier))
            {
                throw new ArgumentNullException("toIdentifier");
            }
            string metaFromIdentifier = GetMetaIdentifier(fromIdentifier);
            string metaToIdentifier = GetMetaIdentifier(toIdentifier);
            settings.Storage.Copy(metaFromIdentifier, metaToIdentifier, settings);
        }

        /// <summary>
        /// Moves the meta data from to location.
        /// </summary>
        /// <param name="fromIdentifier"></param>
        /// <param name="toIdentifier"></param>
        public static void MoveMetaData(string fromIdentifier, string toIdentifier)
        {
            MoveMetaData(fromIdentifier, toIdentifier, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Moves the meta data from to location.
        /// </summary>
        /// <param name="fromIdentifier"></param>
        /// <param name="toIdentifier"></param>
        /// <param name="settings"></param>
        public static void MoveMetaData(string fromIdentifier, string toIdentifier, SaveGameSettings settings)
        {
            if (string.IsNullOrEmpty(fromIdentifier))
            {
                throw new ArgumentNullException("fromIdentifier");
            }
            if (string.IsNullOrEmpty(fromIdentifier))
            {
                throw new ArgumentNullException("toIdentifier");
            }
            string metaFromIdentifier = GetMetaIdentifier(fromIdentifier);
            string metaToIdentifier = GetMetaIdentifier(toIdentifier);
            settings.Storage.Move(metaFromIdentifier, toIdentifier, settings);
        }

        /// <summary>
        /// Gets the meta data identifier for the specified identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static string GetMetaIdentifier(string identifier)
        {
            if (identifier.Contains(SaveGame.MetaDataExtension))
            {
                return identifier;
            }
            return identifier + "." + SaveGame.MetaDataExtension;
        }

    }

}