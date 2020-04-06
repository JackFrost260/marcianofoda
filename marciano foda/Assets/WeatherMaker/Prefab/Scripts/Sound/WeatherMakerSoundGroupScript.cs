﻿//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store at https://www.assetstore.unity3d.com/en/#!/content/60955?aid=1011lGnL. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    [CreateAssetMenu(fileName = "WeatherMakerSoundGroup", menuName = "WeatherMaker/Sound Group", order = 101)]
    [System.Serializable]
    public class WeatherMakerSoundGroupScript : WeatherMakerBaseScriptableObjectScript
    {
        [Tooltip("All sounds in the group")]
        public List<WeatherMakerSoundScript> Sounds;
        
        /// <summary>
        /// Parent game object to put sounds in
        /// </summary>
        public GameObject Parent { get; set; }

        public bool CanPlay
        {
            get
            {
                return Sounds == null || Sounds.Count == 0 || Sounds[0] == null ? false : Sounds[0].CanPlay;
            }
            set
            {
                if (Sounds != null)
                {
                    foreach (WeatherMakerSoundScript script in Sounds)
                    {
                        if (script != null)
                        {
                            script.CanPlay = value;
                        }
                    }
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Sounds != null)
            {
                for (int i = 0; i < Sounds.Count; i++)
                {
                    WeatherMakerSoundScript script = Sounds[i];
                    if (script != null)
                    {
                        // make a copy of the profile as it uses internal state variables
                        Sounds[i] = script = ScriptableObject.Instantiate(script);
                        script.Parent = Parent;
                        script.Initialize();
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (Sounds != null)
            {
                // update all sounds in group
                for (int i = Sounds.Count - 1; i >= 0; i--)
                {
                    WeatherMakerSoundScript script = Sounds[i];

                    // if the sound is null or audio source is null, sound was destroyed, remove it
                    if (script == null || script.AudioSourceLoop == null || script.AudioSourceLoop.AudioSource == null)
                    {
                        script.OnDestroy();
                        Sounds.RemoveAt(i);
                    }
                    else
                    {
                        script.Update();
                    }
                }
            }
        }

        public override void OnEnable()
        {
            if (Sounds != null)
            {
                foreach (WeatherMakerSoundScript sound in Sounds)
                {
                    if (sound != null)
                    {
                        sound.OnEnable();
                    }
                }
            }
        }

        public override void OnDisable()
        {
            if (Sounds != null)
            {
                foreach (WeatherMakerSoundScript sound in Sounds)
                {
                    if (sound != null)
                    {
                        sound.OnDisable();
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            if (Sounds != null)
            {
                foreach (WeatherMakerSoundScript sound in Sounds)
                {
                    sound.OnDestroy();
                    GameObject.Destroy(sound);
                }
            }
        }

        public void Stop(float seconds, bool destroySounds = false)
        {
            if (Sounds != null)
            {
                foreach (WeatherMakerSoundScript sound in Sounds)
                {
                    if (sound != null)
                    {
                        sound.Stop(seconds, destroySounds);
                        sound.CanPlay = false;
                    }
                }
            }
        }
    }
}