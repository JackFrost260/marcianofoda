using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoDoJogador : MonoBehaviour
{
    CharacterController controlador;
    public float velocidade = 15f;
    public float offset;
    public float gravidade = -9.81f;
    public float distanciaChao = 0.5f;
    public float distanciaPulo = 20f;
    public LayerMask mundo;
    public Transform objetoDeDetecao;
    
    Vector3 velocidadeDeQueda;
    Vector3 velocidadePulo;
    bool noChao;
    float z;
    public float raioDetecao;
    

    // Start is called before the first frame update
    void Start()
    {
        controlador = gameObject.GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //if (Input.GetButtonDown("Fire1"))
       // {
        //    z = 1 * distanciaPulo;
       // }

        Vector3 movimento = transform.right * x + transform.forward * y;
        controlador.Move(movimento * velocidade * Time.deltaTime);


        noChao = Physics.CheckSphere(objetoDeDetecao.position, raioDetecao, mundo, QueryTriggerInteraction.Ignore);

        if (noChao == false)
        {
            velocidadeDeQueda.y += gravidade * Time.deltaTime;
        }

        if (noChao == true)
        {
            velocidadeDeQueda.y = 0;
        }

        velocidadePulo.y = distanciaPulo * Time.deltaTime * z;

        controlador.Move(velocidadeDeQueda * Time.deltaTime);
        controlador.Move(Time.deltaTime * velocidadePulo);

        //print(velocidadeDeQueda);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 120, 20, 170);
        Gizmos.DrawSphere(objetoDeDetecao.position, raioDetecao);
    }
}

