using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BayatGames.SaveGamePro
{

    /// <summary>
    /// This reference manager takes
    /// </summary>
    public class SaveGameReferenceManager : MonoBehaviour
    {

        private static SaveGameReferenceManager current;

        public static SaveGameReferenceManager Current
        {
            get
            {
                if (current == null)
                {
                    SaveGameReferenceManager[] managers = FindObjectsOfType<SaveGameReferenceManager>();
                    if (managers.Length == 1)
                    {
                        current = managers[0];
                    }
                    else if (managers.Length > 1)
                    {
                        throw new InvalidOperationException("There is more than one SaveGameReferenceManager in this scene, but there must only be one.");
                    }
                }
                return current;
            }
        }

        [SerializeField]
        protected List<string> guids;
        [SerializeField]
        protected List<UnityEngine.Object> sceneDependencies;

        public List<string> Guids
        {
            get
            {
                return this.guids;
            }
        }

        public List<UnityEngine.Object> SceneDependencies
        {
            get
            {
                return this.sceneDependencies;
            }
        }

        public UnityEngine.Object Get(string guid)
        {
            int index = this.guids.IndexOf(guid);
            if (index == -1)
            {
                return null;
            }
            else
            {
                return this.sceneDependencies[index];
            }
        }

        public string Get(UnityEngine.Object obj)
        {
            int index = this.sceneDependencies.IndexOf(obj);
            if (index == -1)
            {
                return null;
            }
            else
            {
                return this.guids[index];
            }
        }

        public void Add(UnityEngine.Object obj)
        {
            if (this.sceneDependencies.Contains(obj))
            {
                return;
            }
            Guid guid = SaveGameReferenceManager.GetNewGuid();
            this.guids.Add(guid.ToString());
            this.sceneDependencies.Add(obj);
        }

        public static Guid GetNewGuid()
        {
            return Guid.NewGuid();
        }

    }

}