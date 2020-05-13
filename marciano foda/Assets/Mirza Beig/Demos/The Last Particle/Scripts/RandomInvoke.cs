
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

            [ExecuteInEditMode]
            [System.Serializable]

            public class RandomInvoke : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                float timer;
                float nextInvokeTime;

                public Vector2 timeRange = new Vector2(2.0f, 5.0f);

                // =================================	
                // Functions.
                // =================================

                // ...

                protected virtual void Start()
                {
                    // Even though invoke just calls Invoke, DON'T call the local invoke here!
                    // Since it will be overriden and made to do more...

                    // NOTE: Invoke doesn't seem to work with inheritance. So I'll just do it manually.

                    nextInvokeTime = Random.Range(timeRange.x, timeRange.y);
                }

                // ...

                protected virtual void Update()
                {
                    timer += Time.deltaTime;

                    if (timer >= nextInvokeTime)
                    {
                        doSomething();

                        timer = 0.0f;
                        nextInvokeTime = Random.Range(timeRange.x, timeRange.y);

                    }
                }

                // ...

                protected virtual void doSomething()
                {

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
