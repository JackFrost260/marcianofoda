using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// Saving encrypted data example.
    /// </summary>
    public class SaveEncryptedData : MonoBehaviour
    {

        #region Fields

        [Header("Parameters")]

        /// <summary>
        /// The identifier.
        /// </summary>
        public string identifier = "encrypted.dat";

        [Header("UI References")]

        /// <summary>
        /// The password field.
        /// </summary>
        public InputField password;

        /// <summary>
        /// The data field.
        /// </summary>
        public InputField dataField;

        /// <summary>
        /// The status text.
        /// </summary>
        public Text statusText;

        protected SaveGameSettings saveSettings;

        #endregion

        #region Methods

        private void Awake()
        {
            this.statusText.text = "Waiting for input ...";
            this.saveSettings = new SaveGameSettings();
            this.saveSettings.Encrypt = true;
        }

        /// <summary>
        /// Save the data.
        /// </summary>
        public void Save()
        {
            this.saveSettings.EncryptionPassword = this.password.text;
            SaveGame.Save(this.identifier, this.dataField.text, this.saveSettings);
            Debug.Log("Encrypted Data has been saved successfully");
            this.statusText.text = "Save Successful";
        }

        /// <summary>
        /// Load the data.
        /// </summary>
        public void Load()
        {
            this.saveSettings.EncryptionPassword = this.password.text;
            //SaveGame.LoadInto(this.identifier, this.dataField.text, this.saveSettings);
            this.dataField.text = SaveGame.Load<string>(this.identifier, "Default Data", this.saveSettings);
            Debug.Log("Encrypted Data has been loaded successfully");
            Debug.Log(this.dataField.text);
            this.statusText.text = "Load Successful";
        }

        #endregion

    }

}