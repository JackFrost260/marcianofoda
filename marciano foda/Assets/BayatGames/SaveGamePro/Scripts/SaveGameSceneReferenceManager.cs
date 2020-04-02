using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BayatGames.SaveGamePro
{

    [DisallowMultipleComponent]
    public class SaveGameSceneReferenceManager : MonoBehaviour
    {

        private static SaveGameSceneReferenceManager current;

        public static SaveGameSceneReferenceManager Current
        {
            get
            {
                if (current == null)
                {
                    SaveGameSceneReferenceManager[] managers = FindObjectsOfType<SaveGameSceneReferenceManager>();
                    if (managers.Length == 1)
                    {
                        current = managers[0];
                    }
                    else if (managers.Length > 1)
                    {
                        throw new InvalidOperationException("There is more than one SaveGameSceneReferenceManager in this scene, but there must only be one.");
                    }
                }
                return current;
            }
        }

    }

}