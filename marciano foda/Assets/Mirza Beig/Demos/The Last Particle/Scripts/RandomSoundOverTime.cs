
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

            public class RandomSoundOverTime : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                public class RandomInt
                {
                    int value;
                    bool used;
                }

                // =================================	
                // Variables.
                // =================================

                // ...

                public Vector2 timeRange = new Vector2(5.0f, 10.0f);
                public Vector2 volumeRange = new Vector2(0.5f, 1.0f);

                public Vector2 pitchRange = new Vector2(0.5f, 2.0f);

                float timer;
                float nextPlayTime;

                AudioSource audioSource;

                RandomSequence randomSequence;
                public AudioClip[] audioClips;

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
                    setNextPlayTime();
                    audioSource = GetComponent<AudioSource>();

                    randomSequence = new RandomSequence(audioClips.Length, 8);

                    for (int i = 0; i < 128; i++)
                    {
                        //print(randomSequence.get());
                    }
                }

                // ...

                void setNextPlayTime()
                {
                    nextPlayTime = Random.Range(timeRange.x, timeRange.y);
                }

                // ...

                void Update()
                {
                    timer += Time.deltaTime;

                    if (timer >= nextPlayTime)
                    {
                        timer = 0.0f;
                        setNextPlayTime();

                        AudioClip ac = audioClips[randomSequence.get()];
                        float volume = Random.Range(volumeRange.x, volumeRange.y);

                        float pitch = Random.Range(pitchRange.x, pitchRange.y);

                        audioSource.pitch = pitch;
                        audioSource.PlayOneShot(ac, volume);
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
