
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

            public static class Damping
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                // =================================	
                // Functions.
                // =================================

                // ...

                public static float dampFloat(float from, float to, float speed, float deltaTime)
                {
                    return Mathf.Lerp(from, to, 1.0f - Mathf.Exp(-speed * deltaTime));
                }
                public static Vector3 dampVector3(Vector3 from, Vector3 to, float speed, float deltaTime)
                {
                    return Vector3.Lerp(from, to, 1.0f - Mathf.Exp(-speed * deltaTime));
                }
                public static Quaternion dampQuaternion(Quaternion from, Quaternion to, float speed, float deltaTime)
                {
                    return Quaternion.Slerp(from, to, 1.0f - Mathf.Exp(-speed * deltaTime));
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
