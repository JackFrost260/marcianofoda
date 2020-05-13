
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

            public class RandomInvoke_EmitParticle : RandomInvoke
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                ParticleSystem ps;

                public IntRange count = new IntRange(1, 2);
                public UnityEvent OnEmit;

                // =================================	
                // Functions.
                // =================================

                // ...

                protected override void Start()
                {
                    base.Start();

                    ps = GetComponent<ParticleSystem>();
                }

                // ...

                protected override void Update()
                {
                    base.Update();
                }

                // ...

                protected override void doSomething()
                {
                    base.doSomething();

                    ps.Emit(count.random);

                    if (OnEmit != null)
                    {
                        OnEmit.Invoke();
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
