using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// Custom path save example.
    /// Save using Absolute path, relative path, ...
    /// </summary>
    public class CustomPathSave : MonoBehaviour
    {

        #region Fields

        [Header("UI References")]

        /// <summary>
        /// The identifier input field.
        /// </summary>
        public InputField identifierInputField;

        /// <summary>
        /// The data input field.
        /// </summary>
        public InputField dataInputField;

        /// <summary>
        /// The status text.
        /// </summary>
        public Text statusText;

        #endregion

        #region Methods

        void Awake()
        {
            SaveGame.OnSaved += SaveGame_OnSaved;
            SaveGame.OnLoaded += SaveGame_OnLoaded;
            SaveGame.OnDeleted += SaveGame_OnDeleted;
        }

        void Start()
        {
            this.statusText.text = "Waiting for input ...";

            // Set the fisrt dummy path.
            if (string.IsNullOrEmpty(identifierInputField.text) || !Path.IsPathRooted(identifierInputField.text))
            {
                identifierInputField.text = Path.Combine(Application.persistentDataPath, "helloWorld.txt");
            }
        }

        void SaveGame_OnDeleted(string identifier, SaveGameSettings settings)
        {
            Debug.LogFormat("The identifier '{0}' deleted successfully", identifier);
            this.statusText.text = string.Format("The identifier '{0}' deleted successfully", identifier);
        }

        void SaveGame_OnLoaded(string identifier, object result, System.Type type, object defaultValue, SaveGameSettings settings)
        {
            Debug.LogFormat("The identifier '{0}' loaded successfully", identifier);
            this.statusText.text = string.Format("The identifier '{0}' loaded successfully", identifier);
        }

        void SaveGame_OnSaved(string identifier, object value, SaveGameSettings settings)
        {
            Debug.LogFormat("The identifier '{0}' saved successfully", identifier);
            this.statusText.text = string.Format("The identifier '{0}' saved successfully", identifier);
        }

        /// <summary>
        /// Save the data to the specified path.
        /// </summary>
        public void Save()
        {
            SaveGame.Save(identifierInputField.text, dataInputField.text);
        }

        /// <summary>
        /// Load the data from the specified path.
        /// </summary>
        public void Load()
        {
            dataInputField.text = SaveGame.Load<string>(identifierInputField.text, "Hello World");
        }

        /// <summary>
        /// Delete the data from the specified path.
        /// </summary>
        public void Delete()
        {
            SaveGame.Delete(identifierInputField.text);
        }

        #endregion

    }

}