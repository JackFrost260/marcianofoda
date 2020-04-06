using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// Save custom data example.
    /// </summary>
    public class SaveCustomData : MonoBehaviour
    {

        /// <summary>
        /// The custom data.
        /// </summary>
        [System.Serializable]
        public class CustomData
        {

            public string playerName = "Hello World";
            public int score = 12;
            public Vector2[] points;

            /// <summary>
            /// Sample non-saved field, this field will not saved, because marked as NonSavable.
            /// </summary>
            [NonSavable]
            public bool test;

        }

        #region Fields

        [Header("Parameters")]

        /// <summary>
        /// The identifier.
        /// </summary>
        public string identifier = "customData.txt";

        /// <summary>
        /// The data.
        /// </summary>
        public CustomData data;

        [Header("UI References")]

        /// <summary>
        /// The status text.
        /// </summary>
        public Text statusText;

        #endregion

        #region Methods

        void Start()
        {
            this.statusText.text = "Waiting for input ...";
            SaveGameSettings settings = SaveGame.DefaultSettings;
            settings.Formatter = new BayatGames.SaveGamePro.Serialization.Formatters.Json.JsonFormatter();
            settings.Encrypt = false;
            SaveGame.DefaultSettings = settings;
            this.data.points = new Vector2[10] {
                Vector2.one,
                Vector2.one,
                Vector2.one,
                Vector2.one,
                Vector2.one,
                Vector2.one,
                Vector2.one,
                Vector2.one,
                Vector2.one,
                Vector2.one,
            };
            if (SaveGame.Exists("points.txt"))
            {
                SaveGame.Load<Vector2[]>("points.txt");
            }
            //SaveGame.Save("points.txt", this.data.points);
        }

        /// <summary>
        /// Save the custom data.
        /// </summary>
        public void Save()
        {
            SaveGame.Save(identifier, data);
            Debug.Log("Data Saved");
            this.statusText.text = "Save Successful";
        }

        /// <summary>
        /// Load the custom data.
        /// </summary>
        public void Load()
        {
            data = SaveGame.Load<CustomData>(identifier);
            Debug.Log("Data Loaded");
            Debug.Log(data.playerName);
            //Debug.Log(data.score);
            this.statusText.text = "Load Successful";
        }

        #endregion

    }

}