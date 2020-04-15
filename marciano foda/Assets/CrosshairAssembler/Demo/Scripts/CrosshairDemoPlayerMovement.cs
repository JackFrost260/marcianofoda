using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairDemoPlayerMovement : MonoBehaviour
{
    public float MoveForce = 10f;
    public float Sensitivity = 2f;

    private CharacterController playerController;
    private Vector2 recoiling = new Vector2();
    private Vector3 currentSpeed;
    private float jumpingPower;

    void Awake()
    {
        var controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            gameObject.AddComponent<CharacterController>();
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // hide cursor
        playerController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // character movement
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        Vector3 moveDirSide = transform.right * horiz * MoveForce;
        Vector3 moveDirForward = transform.forward * vert * MoveForce;
        currentSpeed = moveDirSide + moveDirForward;

        if (playerController.isGrounded)
        {
            playerController.SimpleMove(currentSpeed);
        }
        else
        {
            currentSpeed.y = 0;
            playerController.Move(currentSpeed / 200);
            playerController.SimpleMove(Physics.gravity * Time.deltaTime);
        }

        // character jump
        if (Input.GetKeyDown("space") && playerController.isGrounded)
        {
            jumpingPower = 15;
        }
        else
        {
            transform.Translate(Vector3.up * jumpingPower * Time.deltaTime, Space.World);
            if (jumpingPower > 0)
                --jumpingPower;
        }

        // character rotation
        float mouseX = Input.GetAxis("Mouse X") * Sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Sensitivity;
        Vector3 targetRot = transform.rotation.eulerAngles;
        targetRot.y += mouseX + recoiling.x;
        targetRot.x -= mouseY + recoiling.y;

        float rotationX = targetRot.x < 180 ? targetRot.x : targetRot.x - 360;        
        rotationX = Mathf.Clamp(rotationX, -60f, 60f);
        rotationX = rotationX >= 0 ? rotationX : rotationX + 360f;
        targetRot.x = rotationX;

        transform.rotation = Quaternion.Euler(targetRot);
        recoiling = Vector2.zero;
    }
}
