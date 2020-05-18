using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TextTime : MonoBehaviour
{
    public static bool textAtivado;
    public static string feedbackString;
    public float time = 2;
    private float timer = 2;
    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (textAtivado)
        {
            text.text = feedbackString.ToUpper();
            timer -= Time.deltaTime;


            if (timer <= 0)
            {
                timer = time;
                text.text = " ";
                textAtivado = false;
            }
        }
    }
}
