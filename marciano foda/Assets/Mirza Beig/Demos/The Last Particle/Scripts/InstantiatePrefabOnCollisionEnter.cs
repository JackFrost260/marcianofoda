
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

            public class InstantiatePrefabOnCollisionEnter : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                public GameObject[] prefabs;

                // =================================	
                // Functions.
                // =================================

                // ...

                void OnCollisionEnter(Collision collision)
                {
                    ContactPoint contactPoint = collision.contacts[0];

                    for (int i = 0; i < prefabs.Length; i++)
                    {
                        Instantiate(prefabs[i], contactPoint.point, Quaternion.Euler(contactPoint.point));
                    }
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
