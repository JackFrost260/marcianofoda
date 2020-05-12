
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

            public class AudioPitchAndVolumeByVelocity : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                public float maxSpeed = 20.0f;

                public Vector2 volumeRange = new Vector2(0.5f, 1.0f);
                public Vector2 pitchRange = new Vector2(0.5f, 2.0f);

                Rigidbody rb;
                AudioSource audioSource;

                // =================================	
                // Functions.
                // =================================

                // ...

                void Awake()
                {

                }

                // ...

                void Start()
                {
                    rb = GetComponentInParent<Rigidbody>();
                    audioSource = GetComponent<AudioSource>();
                }

                // ...

                void LateUpdate()
                {
                    float speed = rb.velocity.magnitude;
                    float lerpTimePosition = speed / maxSpeed;

                    lerpTimePosition = Mathf.Clamp01(lerpTimePosition);

                    audioSource.volume = Mathf.Lerp(volumeRange.x, volumeRange.y, lerpTimePosition);
                    audioSource.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, lerpTimePosition);
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
