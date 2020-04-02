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
    /// Provides utility methods for Encryption which allows you to migrate unencrypted data to encrypted or vice versa.
    /// </summary>
    public static class EncryptionUtils
    {

        /// <summary>
        /// Encrypts all files recursively inside directories and in the base directory.
        /// </summary>
        /// <param name="settings"></param>
        public static void EncryptAll(SaveGameSettings settings)
        {

            // Encrypt files in the home folder
            EncryptFiles(settings);

            // Encrypt all directories
            EncryptDirectories(true);
        }

        /// <summary>
        /// Decrypts all files recursively inside directories and in the base directory.
        /// </summary>
        /// <param name="settings"></param>
        public static void DecryptAll(SaveGameSettings settings)
        {

            // Decrypt files in the home folder
            DecryptFiles();

            // Decrypt all directories
            DecryptDirectories(true);
        }

        /// <summary>
        /// Encrypts the files inside the directories.
        /// </summary>
        public static void EncryptDirectories()
        {
            EncryptDirectories(string.Empty, false, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Encrypts the files inside the directories.
        /// </summary>
        /// <param name="identifier"></param>
        public static void EncryptDirectories(string identifier)
        {
            EncryptDirectories(identifier, false, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Encrypts the files inside the directories.
        /// </summary>
        /// <param name="settings"></param>
        public static void EncryptDirectories(SaveGameSettings settings)
        {
            EncryptDirectories(string.Empty, false, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Encrypts the files inside the directories.
        /// </summary>
        /// <param name="recursive"></param>
        public static void EncryptDirectories(bool recursive)
        {
            EncryptDirectories(string.Empty, recursive, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Encrypts the files inside the directories.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="recursive"></param>
        public static void EncryptDirectories(string identifier, bool recursive)
        {
            EncryptDirectories(identifier, recursive, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Encrypts the files inside the directories.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="recursive"></param>
        /// <param name="settings"></param>
        public static void EncryptDirectories(string identifier, bool recursive, SaveGameSettings settings)
        {
            settings.Identifier = identifier;
            DirectoryInfo[] directories = SaveGame.GetDirectories(identifier, settings);
            for (int i = 0; i < directories.Length; i++)
            {
                DirectoryInfo directory = directories[i];
                string directoryIdentifier = PathUtils.GetRelativePath(directory.FullName, settings.BasePath);
                EncryptFiles(directoryIdentifier);

                // Repeat until there are no directories
                if (recursive)
                {
                    EncryptDirectories(directoryIdentifier, recursive, settings);
                }
            }
        }

        /// <summary>
        /// Decrypts the files inside the directories in the specified directory.
        /// </summary>
        public static void DecryptDirectories()
        {
            DecryptDirectories(string.Empty, false, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Decrypts the files inside the directories in the specified directory.
        /// </summary>
        /// <param name="identifier"></param>
        public static void DecryptDirectories(string identifier)
        {
            DecryptDirectories(identifier, false, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Decrypts the files inside the directories in the specified directory.
        /// </summary>
        /// <param name="recursive"></param>
        public static void DecryptDirectories(bool recursive)
        {
            DecryptDirectories(string.Empty, recursive, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Decrypts the files inside the directories in the specified directory.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="recursive"></param>
        public static void DecryptDirectories(string identifier, bool recursive)
        {
            DecryptDirectories(identifier, recursive, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Decrypts the files inside the directories in the specified directory.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="recursive"></param>
        /// <param name="settings"></param>
        public static void DecryptDirectories(string identifier, bool recursive, SaveGameSettings settings)
        {
            settings.Identifier = identifier;
            DirectoryInfo[] directories = SaveGame.GetDirectories(identifier, settings);
            for (int i = 0; i < directories.Length; i++)
            {
                DirectoryInfo directory = directories[i];
                string directoryIdentifier = PathUtils.GetRelativePath(directory.FullName, settings.BasePath);
                DecryptFiles(directoryIdentifier);

                // Repeat until there are no directories
                if (recursive)
                {
                    DecryptDirectories(directoryIdentifier, recursive, settings);
                }
            }
        }

        /// <summary>
        /// Encrypts the files in the specified directory and overwrites them with encrypted data.
        /// </summary>
        public static void EncryptFiles()
        {
            EncryptFiles(string.Empty, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Encrypts the files in the specified directory and overwrites them with encrypted data.
        /// </summary>
        /// <param name="identifier"></param>
        public static void EncryptFiles(string identifier)
        {
            EncryptFiles(identifier, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Encrypts the files in the specified directory and overwrites them with encrypted data.
        /// </summary>
        /// <param name="settings"></param>
        public static void EncryptFiles(SaveGameSettings settings)
        {
            EncryptFiles(string.Empty, settings);
        }

        /// <summary>
        /// Encrypts the files in the specified directory and overwrites them with encrypted data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="settings"></param>
        public static void EncryptFiles(string identifier, SaveGameSettings settings)
        {
            settings.Identifier = identifier;
            FileInfo[] files = SaveGame.GetFiles(identifier, settings);
            foreach (FileInfo file in files)
            {
                string fileIdentifier = PathUtils.GetRelativePath(file.FullName, settings.BasePath);
                Encrypt(fileIdentifier, settings);
            }
        }

        /// <summary>
        /// Decrypts the files in the specified directory and overwrites them with decrypted data.
        /// </summary>
        public static void DecryptFiles()
        {
            DecryptFiles(string.Empty, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Decrypts the files in the specified directory and overwrites them with decrypted data.
        /// </summary>
        /// <param name="identifier"></param>
        public static void DecryptFiles(string identifier)
        {
            DecryptFiles(identifier, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Decrypts the files in the specified directory and overwrites them with decrypted data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="settings"></param>
        public static void DecryptFiles(string identifier, SaveGameSettings settings)
        {
            settings.Identifier = identifier;
            FileInfo[] files = SaveGame.GetFiles(identifier, settings);
            foreach (FileInfo file in files)
            {
                string fileIdentifier = PathUtils.GetRelativePath(file.FullName, settings.BasePath);
                Decrypt(fileIdentifier, settings);
            }
        }

        /// <summary>
        /// Encrypts the data and overwrites the identifier with encrypted data.
        /// </summary>
        /// <param name="identifier"></param>
        public static void Encrypt(string identifier)
        {
            Encrypt(identifier, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Encrypts the data and overwrites the identifier with encrypted data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="settings"></param>
        public static void Encrypt(string identifier, SaveGameSettings settings)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException("identifier");
            }

            // Load existing meta data or create a new one
            MetaData metaData = new MetaData();
            if (MetaDataUtils.HasMetaData(identifier))
            {
                metaData = MetaDataUtils.LoadMetaData(identifier, settings);
                if (metaData.Has("Encrypted") && metaData.Get<bool>("Encrypted"))
                {
                    return;
                }
            }
            settings.Identifier = identifier;
            settings.Encrypt = false;
            settings.Storage.OnSave(settings);
            byte[] data = SaveGame.LoadRaw(identifier, settings);
            Stream stream = settings.Storage.GetWriteStream(settings);
            CryptoStream encStream = new CryptoStream(stream, settings.Encryptor, CryptoStreamMode.Write);
            encStream.Write(data, 0, data.Length);
            encStream.FlushFinalBlock();
            settings.Storage.OnSaved(settings);
            stream.Dispose();

            // Update and save meta data.
            metaData.Set("Encrypted", true);
            MetaDataUtils.SaveMetaData(identifier, metaData);
        }

        /// <summary>
        /// Decrypts the data and overwrites the identifier with decrypted data.
        /// </summary>
        /// <param name="identifier"></param>
        public static void Decrypt(string identifier)
        {
            Decrypt(identifier, SaveGame.DefaultSettings);
        }

        /// <summary>
        /// Decrypts the data and overwrites the identifier with decrypted data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="settings"></param>
        public static void Decrypt(string identifier, SaveGameSettings settings)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException("identifier");
            }

            // Load existing meta data or create a new one
            MetaData metaData = new MetaData();
            if (MetaDataUtils.HasMetaData(identifier))
            {
                metaData = MetaDataUtils.LoadMetaData(identifier, settings);
                if (metaData.Has("Encrypted") && !metaData.Get<bool>("Encrypted"))
                {
                    return;
                }
            }
            settings.Identifier = identifier;
            settings.Encrypt = true;
            settings.Storage.OnLoad(settings);
            Stream stream = settings.Storage.GetReadStream(settings);
            byte[] data;
            using (MemoryStream dataOut = new MemoryStream())
            {
                using (MemoryStream memoryStream = new MemoryStream(stream.ReadFully()))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, settings.Decryptor, CryptoStreamMode.Read))
                    {
                        byte[] decryptedData = cryptoStream.ReadFully();
                        dataOut.Write(decryptedData, 0, decryptedData.Length);
                    }
                }
                dataOut.Flush();
                data = dataOut.ToArray();
            }
            settings.Storage.OnLoaded(settings);
            stream.Dispose();
            SaveGame.SaveRaw(settings.Identifier, data, settings);

            // Update and save meta data.
            metaData.Set("Encrypted", false);
            MetaDataUtils.SaveMetaData(identifier, metaData);
        }

    }

}