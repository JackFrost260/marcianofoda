using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    #region Singleton

    public static Compass instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject iconPrefab;
    public RawImage compassImage;
    public Transform player;

    public static List<QuestMarker> questMarkers = new List<QuestMarker>();

    public float maxDistanece = 300f;

    float compassUnit;

    private void Start()
    {
        compassUnit = compassImage.rectTransform.rect.width / 360f;
    }
    void Update()
    {
        compassImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0, 1f, 1f);

        foreach(QuestMarker marker in questMarkers)
        {
            marker.image.rectTransform.anchoredPosition = GetPosOmCompass(marker);

            float dst = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), marker.position);
            //float scale = 0f;

          //if (dst < maxDistanece)
         // {
          //      scale = 1f -(dst / maxDistanece);
        // }

         // marker.image.rectTransform.localScale = Vector3.one * scale;
        }
    }

    public void AddQuestMarker (QuestMarker marker)
    {
        GameObject newMarker = Instantiate(iconPrefab, compassImage.transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        questMarkers.Add(marker);
    }

    Vector2 GetPosOmCompass(QuestMarker marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
