using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// Save Abstract class example.
    /// </summary>
    public class SaveAbstractClass : MonoBehaviour
    {

        #region Fields

        [Header("Data References")]

        /// <summary>
        /// The weapon.
        /// </summary>
        public ItemWeapon weapon;

        /// <summary>
        /// The collection of items.
        /// </summary>
        public List<ItemBase> items;

        [Header("UI References")]

        /// <summary>
        /// The status text.
        /// </summary>
        public Text statusText;

        #endregion

        #region Methods

        private void Start()
        {
            this.statusText.text = "Waiting for input ...";
            this.items = new List<ItemBase>();
            items.Add(weapon);
        }

        /// <summary>
        /// Save the abstract data.
        /// </summary>
        public void Save()
        {
            this.statusText.text = string.Format("The identifier '{0}' saved successfully", "abstract.dat");
            SaveGame.Save("abstract.dat", items);
        }

        /// <summary>
        /// Load the abstract data.
        /// </summary>
        public void Load()
        {
            this.statusText.text = string.Format("The identifier '{0}' loaded successfully", "abstract.dat");
            SaveGame.LoadInto("abstract.dat", this.items);
            Debug.Log(this.items[0]);
        }

        #endregion

    }

}