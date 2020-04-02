using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// Save raw data example.
    /// This example showcases saving and loading raw data, which means no serialization or formatting applied, the raw binary data is saved and loaded.
    /// Also, if you enable the Encrypt option, the data will be encrypted while saving or decrypted when loading.
    /// </summary>
    public class SaveRawData : MonoBehaviour
    {

        /// <summary>
        /// The identifier.
        /// </summary>
        public string identifier = "rawData.bin";

        /// <summary>
        /// The data.
        /// </summary>
        public byte[] data;

        /// <summary>
        /// Save the data.
        /// This method is fired with Button press.
        /// </summary>
        public void Save()
        {
            SaveGame.SaveRaw(identifier, data);
            Debug.Log("Data Saved");
        }

        /// <summary>
        /// Load the data.
        /// This method is fired with Button press.
        /// </summary>
        public void Load()
        {
            data = SaveGame.LoadRaw(identifier);
            Debug.Log("Data Loaded");
            Debug.Log(data.Length);
        }

    }

}