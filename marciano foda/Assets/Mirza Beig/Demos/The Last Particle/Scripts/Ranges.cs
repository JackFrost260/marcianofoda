
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
            
            [System.Serializable]
            public class IntRange
            {
                public IntRange(int min, int max)
                {
                    this.min = min;
                    this.max = max;
                }

                public int min = 0;
                public int max = 1;

                public int random
                {
                    get
                    {
                        return Random.Range(min, max);
                    }
                }
            }

            // ...

            [System.Serializable]
            public class FloatRange
            {
                public FloatRange(float min, float max)
                {
                    this.min = min;
                    this.max = max;
                }

                public float min = 0;
                public float max = 1;

                public float random
                {
                    get
                    {
                        return Random.Range(min, max);
                    }
                }
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
