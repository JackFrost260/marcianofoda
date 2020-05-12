
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using MirzaBeig.ParticleSystems;

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

            public class VolumeAndPitchNoiseOverTime : MonoBehaviour
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

                public float baseVolume = 0.5f;
                public float basePitch = 1.0f;

                // ...

                public MirzaBeig.ParticleSystems.PerlinNoise volumeNoise;
                public MirzaBeig.ParticleSystems.PerlinNoise pitchNoise;

                public bool unscaledTime;

                // =================================	
                // Functions.
                // =================================

                // ...

                void Start()
                {
                    audioSource = GetComponent<AudioSource>();

                    volumeNoise.init();
                    pitchNoise.init();
                }

                // ...

                void Update()
                {
                    float time = !unscaledTime ? Time.time : Time.unscaledTime;

                    audioSource.volume = baseVolume + volumeNoise.GetValue(time);
                    audioSource.pitch = basePitch + pitchNoise.GetValue(time);
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
