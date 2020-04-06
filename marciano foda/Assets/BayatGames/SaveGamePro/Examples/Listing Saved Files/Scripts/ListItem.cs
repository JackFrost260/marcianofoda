using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// List Saved Files example.
    /// List Item.
    /// </summary>
    public class ListItem : MonoBehaviour
    {

        #region Methods

        /// <summary>
        /// The name text.
        /// </summary>
        public Text nameText;

        /// <summary>
        /// The date text.
        /// </summary>
        public Text dateText;

        /// <summary>
        /// The file.
        /// </summary>
        public FileInfo file;

        /// <summary>
        /// The example manager.
        /// </summary>
        public ListSavedFiles manager;

        #endregion

        #region Methods

        void Start()
        {
            nameText.text = file.Name;
            dateText.text = file.LastWriteTime.ToString();
        }

        /// <summary>
        /// Delete the saved file.
        /// </summary>
        public void Delete()
        {
            this.manager.Delete(this);
        }

        #endregion

    }

}