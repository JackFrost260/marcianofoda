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
    /// AudioClip save asynchronous.
    /// </summary>
    public class AudioClipSaveAsync : MonoBehaviour
    {

        /// <summary>
        /// The data structure to fetch AudioClip informtion in Main Thread and save it in Async.
        /// </summary>
        public struct ClipData
        {
            public string name;
            public float[] samples;
            public int channels;
            public int frequency;
        }

        #region Fields

        [Header("Parameters")]

        /// <summary>
        /// The identifier.
        /// </summary>
        public string identifier = "audioClipSaveSync.txt";

        /// <summary>
        /// The audio clip.
        /// </summary>
        public AudioClip audioClip;

        [Header("UI Refrences")]

        /// <summary>
        /// The status text.
        /// </summary>
        public Text statusText;

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
            ClipData data = new ClipData();
            data.name = this.audioClip.name;
            data.samples = new float[this.audioClip.samples];
            audioClip.GetData(data.samples, 0);
            data.channels = this.audioClip.channels;
            data.frequency = this.audioClip.frequency;

            // Storing the state to update the status text accordingly, these are not necessary for production, or maybe you have some UI which you want to be updated based on the state code.
            bool isCancelled = false;
            bool isFailed = false;
            SaveGame.SaveAsync(this.identifier, data).ContinueWith((task) =>
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
            Task<ClipData> task = SaveGame.LoadAsync<ClipData>(this.identifier);
            task.ContinueWith((result) =>
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

                    // The loaded data is available at Task.Result property, so we assign it to our data variable when loaded, for more informtion check out the below link:
                    // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1.result
                    // But we can't assign the data directly here, as we are running in another thread than Main Thread, which Unity doesn't allow to use it's API for example, manipulating AudioClip data.
                    //ClipData data = result.Result;
                }
            });
            task.Wait();
            ClipData data = task.Result;
            this.audioClip = AudioClip.Create(data.name, data.samples.Length, data.channels, data.frequency, false);
            audioClip.SetData(data.samples, 0);

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