
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

            public class RandomInvoke_PlayRandomSound : RandomInvoke
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                AudioSource audioSource;
                public AudioClip[] audioClips;

                // =================================	
                // Functions.
                // =================================

                // ...

                protected override void Start()
                {
                    base.Start();
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

                    audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                    audioSource.Play();
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
