using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BayatGames.SaveGamePro.Editor
{

    [CustomEditor(typeof(SaveGameReferenceManager))]
    public class SaveGameReferenceManagerEditor : UnityEditor.Editor
    {

        protected SaveGameReferenceManager referenceManager;

        private void OnEnable()
        {
            this.referenceManager = (SaveGameReferenceManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();
            GUILayout.Label(string.Format("{0} References Available", this.referenceManager.SceneDependencies.Count), new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter
            });
            EditorGUILayout.Separator();
            if (GUILayout.Button("Generate References"))
            {
                GenerateReferences();

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        public void GenerateReferences()
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            ArrayUtility.Remove(ref roots, this.referenceManager.gameObject);
            var dependencies = EditorUtility.CollectDependencies(roots);
            for (int i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];
                this.referenceManager.Add(dependency);
            }
        }

    }

}