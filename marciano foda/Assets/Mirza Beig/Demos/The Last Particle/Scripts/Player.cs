
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

            public class Player : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                public Rigidbody rb { get; set; }

                // ...

                public float thrust = 0.125f;
                public float boostThrustMult = 2.0f;

                // ...

                new Camera camera;
                public float rotateToCameraForwardSpeed = Mathf.Infinity;

                // ...

                public float cameraFieldOfViewMaxSpeed = 20.0f;
                public Vector2 cameraFieldOfViewAngleRange = new Vector2(50.0f, 75.0f);

                // ...

                Vector3 startPosition;
                Quaternion startRotation;

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
                    rb = GetComponent<Rigidbody>();

                    startPosition = transform.position;
                    startRotation = transform.rotation;

                    lockMouse();

                    camera = Camera.main;
                }

                // ...

                void lockMouse()
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                void unlockMouse()
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

                // ...

                void FixedUpdate()
                {
                    // Input.

                    float vertical = Input.GetAxis("Vertical");
                    float horizontal = Input.GetAxis("Horizontal");

                    //float forward = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 0.0f;

                    Vector3 force = Vector3.zero;

                    force += Vector3.forward * vertical;
                    force += Vector3.right * horizontal;

                    force *= thrust;

                    bool boost = Input.GetKey(KeyCode.LeftShift);

                    if (boost)
                    {
                        force *= boostThrustMult;
                    }

                    rb.AddRelativeForce(force, ForceMode.VelocityChange);

                    // ...

                    float currentSpeed = rb.velocity.magnitude;
                    float cameraFieldOfViewSpeedTime = currentSpeed / cameraFieldOfViewMaxSpeed;

                    cameraFieldOfViewSpeedTime = Mathf.Clamp01(cameraFieldOfViewSpeedTime);

                    camera.fieldOfView = Mathf.Lerp(
                        cameraFieldOfViewAngleRange.x, cameraFieldOfViewAngleRange.y, cameraFieldOfViewSpeedTime);
                }

                // ...

                void Update()
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        lockMouse();
                    }
                    else if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        unlockMouse();
                    }

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        transform.position = startPosition;
                        transform.rotation = startRotation;
                    }
                }

                // ...

                void LateUpdate()
                {
                    transform.rotation = Damping.dampQuaternion(transform.rotation,
                        Quaternion.LookRotation(camera.transform.forward), rotateToCameraForwardSpeed, Time.deltaTime);
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
