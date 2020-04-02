using System.Collections;
using System.Collections.Generic;
#if NET_4_6 || NET_STANDARD_2_0
using System.Threading.Tasks;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// Basic save asynchronous.
    /// </summary>
    public class BasicSaveAsync : MonoBehaviour
    {

        #region Fields

        [Header("Parameters")]

        /// <summary>
        /// The identifier.
        /// </summary>
        public string identifier = "saveAsync.txt";

        /// <summary>
        /// The data.
        /// </summary>
        public string data = "This is the data.";

        [Header("UI References")]

        /// <summary>
        /// The status text.
        /// </summary>
        public Text statusText;

        #endregion

        #region Properties

        /// <summary>
        /// Data getter & setter.
        /// </summary>
        public string Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }

        #endregion

        #region Methods

        private void Start()
        {
            this.statusText.text = "Waiting for input ...";
        }

        /// <summary>
        /// Save the data.
        /// </summary>
        public void Save()
        {
#if NET_4_6 || NET_STANDARD_2_0
            Debug.Log("Saving ...");
            this.statusText.text = "Saving ...";

            // Storing the state to update the status text accordingly, these are not necessary for production, or maybe you have some UI which you want to be updated based on the state code.
            bool isCancelled = false;
            bool isFailed = false;
            SaveGame.SaveAsync(this.identifier, this.data).ContinueWith((task) =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Save Task - Cancelled");
                    isCancelled = true;
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Save Tsak - Faulted");
                    Debug.LogException(task.Exception);
                    isFailed = true;
                }
                else
                {
                    Debug.Log("Saved");
                }
            });

            // Now update the UI, which we couldn't do in other thread.
            if (isCancelled)
            {
                this.statusText.text = "Save Cancelled";
            }
            else if (isFailed)
            {
                this.statusText.text = "Save Failed";
            }
            else
            {
                this.statusText.text = "Save Successful";
            }
#endif
        }

        /// <summary>
        /// Load the data.
        /// </summary>
        public void Load()
        {
#if NET_4_6 || NET_STANDARD_2_0
            Debug.Log("Loading ...");
            this.statusText.text = "Loading ...";

            // Storing the state to update the status text accordingly, these are not necessary for production, or maybe you have some UI which you want to be updated based on the state code.
            bool isCancelled = false;
            bool isFailed = false;
            SaveGame.LoadAsync<string>(this.identifier).ContinueWith((task) =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Load Task - Cancelled");
                    isCancelled = true;
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Load Tsak - Faulted");
                    Debug.LogException(task.Exception);
                    isFailed = true;
                }
                else
                {
                    Debug.Log("Loaded");
                    Debug.Log(task.Result);

                    // The loaded data is available at Task.Result property, so we assign it to our data variable when loaded, for more informtion check out the below link:
                    // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1.result
                    this.data = task.Result;
                }
            });

            // Now update the UI, which we couldn't do in other thread.
            if (isCancelled)
            {
                this.statusText.text = "Load Cancelled";
            }
            else if (isFailed)
            {
                this.statusText.text = "Load Failed";
            }
            else
            {
                this.statusText.text = "Load Successful";
            }
#endif
        }

        #endregion

    }

}