
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using UnityEngine.Events;

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

            public class OnTriggerEvents : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                public UnityEvent OnTriggerEvent;

                // =================================	
                // Functions.
                // =================================

                // ...

                void OnTriggerEnter(Collider collider)
                {
                    if (OnTriggerEvent != null)
                    {
                        OnTriggerEvent.Invoke();
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
