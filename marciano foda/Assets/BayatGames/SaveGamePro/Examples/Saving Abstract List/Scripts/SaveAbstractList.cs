using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// Save abstract list example.
    /// This example showcases saving and loading abstract list data.
    /// </summary>
    public class SaveAbstractList : MonoBehaviour
    {

        #region Fields

        [Header("Parameters")]

        /// <summary>
        /// The identifier.
        /// </summary>
        public string identifier = "abstractList.bin";

        /// <summary>
        /// The data.
        /// </summary>
        public List<Item> data;

        [Header("UI References")]

        /// <summary>
        /// The status text.
        /// </summary>
        public Text statusText;

        #endregion

        private void Start()
        {
            this.statusText.text = "Waiting for input ...";
            this.data = new List<Item>();
            Sword sword = new Sword("Arthur's Sword", 100, true);
            this.data.Add(sword);
        }

        /// <summary>
        /// Save the data.
        /// This method is fired with Button press.
        /// </summary>
        public void Save()
        {
            this.statusText.text = string.Format("The identifier '{0}' saved successfully", this.identifier);
            SaveGame.Save(identifier, this.data);
            Debug.Log("Data Saved");
        }

        /// <summary>
        /// Load the data.
        /// This method is fired with Button press.
        /// </summary>
        public void Load()
        {
            this.statusText.text = string.Format("The identifier '{0}' loaded successfully", this.identifier);
            this.data = SaveGame.Load<List<Item>>(identifier);
            Debug.Log("Data Loaded");
            Debug.Log(this.data[0]);
        }

    }

    /// <summary>
    /// Base item class.
    /// </summary>
    [Serializable]
    public abstract class Item
    {

        [SerializeField]
        protected string name;

        public virtual string Name
        {
            get
            {
                return this.name;
            }
        }

    }

    /// <summary>
    /// Base weapon class.
    /// </summary>
    [Serializable]
    public abstract class Weapon : Item
    {

        [SerializeField]
        protected int damage;

        public virtual int Damage
        {
            get
            {
                return this.damage;
            }
        }

    }

    /// <summary>
    /// Sample weapon class, called Sword in this case.
    /// </summary>
    [Serializable]
    public class Sword : Weapon
    {

        [SerializeField]
        protected bool isLongSword;

        public virtual bool IsLongSword
        {
            get
            {
                return this.isLongSword;
            }
        }

        public Vector3 testVector;

        public Sword(string name, int damage, bool isLongSword)
        {
            this.name = name;
            this.damage = damage;
            this.isLongSword = isLongSword;
        }

    }

}