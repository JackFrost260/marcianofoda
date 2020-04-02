using System;
using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGamePro.Serialization;
using UnityEngine;

namespace BayatGames.SaveGamePro
{

    /// <summary>
    /// Meta data which stores the meta information about each saved entity.
    /// You can also add your own data to it, by using Set and Get methods.
    /// </summary>
    public class MetaData : ISavable
    {

        #region Fields

        protected Dictionary<string, string> data = new Dictionary<string, string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new instance of MetaData class.
        /// </summary>
        public MetaData()
        {
            this.data = new Dictionary<string, string>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks whether there is a value with the given key or not.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Has(string key)
        {
            if (this.data == null)
            {
                this.data = new Dictionary<string, string>();
            }
            return this.data.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value from the given key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            string value;
            this.data.TryGetValue(key, out value);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Sets the value at the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            string strValue = value.ToString();
            if (this.data.ContainsKey(key))
            {
                this.data[key] = strValue;
            }
            else
            {
                this.data.Add(key, strValue);
            }
        }

        public void OnWrite(ISaveGameWriter writer)
        {
            writer.Write(this.data);
        }

        public void OnRead(ISaveGameReader reader)
        {
            this.data = reader.Read<Dictionary<string, string>>();
        }

        #endregion

    }

}