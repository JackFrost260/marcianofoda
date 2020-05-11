using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMarker : MonoBehaviour
{

    public Sprite icon;
    public Image image;

    private void Start()
    {
        
        Compass.instance.AddQuestMarker(this);
      
    }

    public Vector2 position
    {
        get { return new Vector2(transform.position.x, transform.position.y); }
    }

    
}
