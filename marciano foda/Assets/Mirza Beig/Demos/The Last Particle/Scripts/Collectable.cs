
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

            public class Collectable : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                public GameObject[] deathPrefabs;

                SetParent sp;
                ParticleSystems.ParticleSystems ps;

                // =================================	
                // Functions.
                // =================================

                // ...

                void Start()
                {
                    sp = GetComponent<SetParent>();
                    ps = GetComponentInChildren<ParticleSystems.ParticleSystems>();
                }

                // ...

                void OnTriggerEnter(Collider collider)
                {
                    if (collider.tag == "Player")
                    {
                        sp.run();
                        ps.stop();

                        for (int i = 0; i < deathPrefabs.Length; i++)
                        {
                            Instantiate(deathPrefabs[i], transform.position, transform.rotation);
                        }

                        Destroy(gameObject);
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
