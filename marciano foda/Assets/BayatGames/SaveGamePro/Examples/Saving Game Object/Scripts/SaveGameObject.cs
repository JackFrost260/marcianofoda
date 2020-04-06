using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BayatGames.SaveGamePro.Examples
{

    /// <summary>
    /// Save game object example.
    /// </summary>
    public class SaveGameObject : MonoBehaviour
    {

        #region Fields

        /// <summary>
        /// The target to save.
        /// </summary>
        public GameObject target;

        /// <summary>
        /// The target renderer.
        /// </summary>
        public Renderer targetRenderer;

        /// <summary>
        /// The red slider.
        /// </summary>
        public Slider redSlider;

        /// <summary>
        /// The green slider.
        /// </summary>
        public Slider greenSlider;

        /// <summary>
        /// The blue slider.
        /// </summary>
        public Slider blueSlider;

        /// <summary>
        /// The alpha slider.
        /// </summary>
        public Slider alphaSlider;

        /// <summary>
        /// The status text.
        /// </summary>
        public Text statusText;

        #endregion

        #region Methods

        private void Start()
        {
            SaveGameData saveGameData = new SaveGameData();
            SaveGame.Save("test", saveGameData);
            saveGameData = SaveGame.Load<SaveGameData>("test");
            this.statusText.text = "Waiting for input ...";
        }

        public class SaveGameData
        {
            public Dictionary<int, SavableData> data = new Dictionary<int, SavableData>();
        }

        public class SavableData
        {
            public int ID;
        }

        /// <summary>
        /// Update the target renderer color.
        /// </summary>
        public void UpdateColor()
        {
            if (target == null)
            {
                Debug.LogWarning("Target object is destroyed.");
                this.statusText.text = "Target object is destroyed.";
                return;
            }
            if (targetRenderer == null)
            {
                targetRenderer = target.GetComponent<Renderer>();
            }
            targetRenderer.material.color = new Color(
                redSlider.value,
                greenSlider.value,
                blueSlider.value,
                alphaSlider.value);
        }

        /// <summary>
        /// Destroy the target.
        /// </summary>
        public void DestroyTarget()
        {
            Destroy(target);
        }

        /// <summary>
        /// Save the target.
        /// </summary>
        public void Save()
        {
            SaveGame.Save("gameObject.txt", target);
        }

        /// <summary>
        /// Load the target, if exists, all the values will be loaded into the Game Object fields.
        /// </summary>
        public void Load()
        {
            if (target == null)
            {
                target = SaveGame.Load<GameObject>("gameObject.txt");
            }
            else
            {
                SaveGame.LoadInto("gameObject.txt", target);
            }
            targetRenderer = target.GetComponent<Renderer>();
            redSlider.value = targetRenderer.material.color.r;
            greenSlider.value = targetRenderer.material.color.g;
            blueSlider.value = targetRenderer.material.color.b;
            alphaSlider.value = targetRenderer.material.color.a;
        }

        #endregion

    }

}