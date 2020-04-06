using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using BayatGames.SaveGamePro.Extensions;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// List saved files example.
    /// </summary>
    public class ListSavedFiles : MonoBehaviour
    {

        #region Fields

        [Header("UI References")]

        /// <summary>
        /// The list container.
        /// </summary>
        public Transform listContainer;

        /// <summary>
        /// The list item prefab.
        /// </summary>
        public ListItem listItemPrefab;

        /// <summary>
        /// The identifier input field.
        /// </summary>
        public InputField identifierInputField;

        /// <summary>
        /// The status text.
        /// </summary>
        public Text statusText;

        #endregion

        #region Methods

        void Start()
        {
            this.statusText.text = "Waiting for input ...";

            // Update list at start
            UpdateList();
        }

        /// <summary>
        /// Save the specified identifier with a dummy data.
        /// </summary>
        public void Save()
        {
            this.statusText.text = string.Format("The identifier '{0}' saved successfully", identifierInputField.text);
            SaveGame.Save(identifierInputField.text, "Hello World");
            UpdateList();
        }

        /// <summary>
        /// Deletes the specified identifier and item from the list.
        /// </summary>
        /// <param name="item"></param>
        public void Delete(ListItem item)
        {
            this.statusText.text = string.Format("The identifier '{0}' deleted successfully", item.file.Name);
            SaveGame.Delete(item.file.Name);
            Destroy(item.gameObject);
        }

        /// <summary>
        /// Update the list.
        /// </summary>
        public void UpdateList()
        {

            // Destroy all list childs.
            listContainer.DestroyChilds();

            // Retrieve the files.
            FileInfo[] files = SaveGame.GetFiles();

            // Create list items.
            for (int i = 0; i < files.Length; i++)
            {
                ListItem item = GameObject.Instantiate<ListItem>(listItemPrefab, listContainer);
                item.file = files[i];
                item.manager = this;
            }
        }

        #endregion

    }

}