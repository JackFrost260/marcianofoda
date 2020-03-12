using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraJogador : MonoBehaviour
{

    public float sensibilidadeMouse;

    public Transform modeloJogador;

    float rotacaoX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * Time.deltaTime;

        rotacaoX -= mouseY;
        rotacaoX = Mathf.Clamp(rotacaoX, -80f, 80f);

        transform.localRotation = Quaternion.Euler(rotacaoX, 0f, 0f);
        modeloJogador.Rotate(Vector2.up * mouseX);
    }
}
