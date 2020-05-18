using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmpliarIcone : MonoBehaviour
{
    private Image Icon;
    private RectTransform TransInicial;


    public void Start()
    {
        Icon = GetComponent<Image>();
        TransInicial.localScale = Icon.rectTransform.localScale;
    }

    public void Ampliar()
    {
        Icon.rectTransform.localScale = new Vector3(TransInicial.localScale.x + 0.2f, TransInicial.localScale.y + 0.2f, TransInicial.localScale.z + 0.2f);
    }

    public void VoltarAoNormal()
    {
        Debug.Log("voltando");
        Icon.rectTransform.localScale = TransInicial.localScale;
    }
}
