
// =================================	
// Namespaces.
// =================================

using UnityEngine;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

    namespace Demos
    {

        namespace TheLastParticle
        {

            // =================================	
            // Classes.
            // =================================

            //[ExecuteInEditMode]
            [System.Serializable]

            public class CollectableSpawner : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                public Collectable collectable;

                public float collectablesPerSecond = 0.25f;
                public Vector3 spawnBoxScale = Vector3.one * 10.0f;

                float timer;

                // =================================	
                // Functions.
                // =================================

                // ...

                void Start()
                {
                    timer = 0.0f;
                }

                // ...

                void Update()
                {
                    timer += Time.deltaTime * collectablesPerSecond;

                    if (timer >= 1.0f)
                    {
                        timer = 0.0f;

                        Vector3 position = transform.position;
                        Vector3 scale = spawnBoxScale / 2.0f;

                        position.x += Random.Range(-scale.x, scale.x);
                        position.y += Random.Range(-scale.y, scale.y);
                        position.z += Random.Range(-scale.z, scale.z);
                                                
                        Instantiate(collectable, position, Quaternion.identity);
                    }
                }

                // ...

                void OnDrawGizmos()
                {
                    Gizmos.DrawWireCube(transform.position, spawnBoxScale);
                }

                // =================================	
                // End functions.
                // =================================

            }

            // =================================	
            // End namespace.
            // =================================

        }

    }

}

// =================================	
// --END-- //
// =================================
