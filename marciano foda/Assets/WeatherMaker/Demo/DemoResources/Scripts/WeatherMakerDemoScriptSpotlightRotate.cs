//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store at https://www.assetstore.unity3d.com/en/#!/content/60955?aid=1011lGnL. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    [RequireComponent(typeof(Light))]
    public class WeatherMakerDemoScriptSpotlightRotate : MonoBehaviour
    {
        [Tooltip("Maximum rotation range")]
        public float Range;

        private Light _light;
        private MeshRenderer _meshRenderer;
        private MeshRenderer meshRenderer2;
        private Transform moveTransform;
        private float baseIntensity;
        private Vector3 initialPos;
        private Vector3 startPos;
        private Vector3 endPos;
        private float currentDuration;
        private float totalDuration;
        private Quaternion startRotation;
        private Quaternion endRotation;

        private void Start()
        {
            initialPos = startPos = transform.position;
            _light = GetComponent<Light>();
            _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshRenderer == null && transform.parent != null)
            {
                _meshRenderer = transform.parent.GetComponent<MeshRenderer>();
                if (_meshRenderer != null)
                {
                    moveTransform = _meshRenderer.transform;
                }
            }
            if (moveTransform == null)
            {
                moveTransform = transform;
            }
            meshRenderer2 = transform.childCount == 0 ? null : transform.GetChild(0).GetComponent<MeshRenderer>();
            _light.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            _light.intensity = baseIntensity = (_light.intensity * Random.Range(0.5f, 1.5f));
            if (_light.type == LightType.Spot)
            {
                _light.range = Random.Range(100.0f, 200.0f);
                _light.spotAngle = Random.Range(30.0f, 90.0f);
                if (_meshRenderer != null)
                {
                    moveTransform.localScale *= (_light.range / 200.0f);
                    Vector2 movement = Random.insideUnitCircle * Random.Range(-Range, Range);
                    moveTransform.Translate(movement.x, 0.0f, movement.y);
                }
            }
            else if (_light.type == LightType.Point)
            {
                _light.range = Random.Range(32.0f, 64.0f);
                moveTransform.localScale *= (_light.range / 100.0f);
            }
            if (_meshRenderer != null)
            {
                _meshRenderer.sharedMaterial = new Material(_meshRenderer.sharedMaterial);
                _meshRenderer.sharedMaterial.SetColor(WMS._EmissionColor, _light.color);
            }
            if (meshRenderer2 != null)
            {
                meshRenderer2.sharedMaterial = new Material(_meshRenderer.sharedMaterial);
                meshRenderer2.sharedMaterial.SetColor(WMS._EmissionColor, Color.gray);
            }
        }

        private void Update()
        {
            currentDuration -= Time.deltaTime;
            if (_light.type == LightType.Spot || _light.type == LightType.Area)
            {
                if (currentDuration <= 0.0f)
                {
                    totalDuration = currentDuration = UnityEngine.Random.Range(3.0f, 6.0f);
                    Vector3 ray = UnityEngine.Random.insideUnitSphere;
                    ray.y = Mathf.Min(ray.y, -0.25f);
                    startRotation = moveTransform.rotation;
                    endRotation = Quaternion.LookRotation(ray);
                }
                moveTransform.rotation = Quaternion.Lerp(startRotation, endRotation, 1.0f - (currentDuration / totalDuration));
            }
            else if (_light.type == LightType.Point)
            {
                if (currentDuration <= 0.0f)
                {
                    totalDuration = currentDuration = UnityEngine.Random.Range(3.0f, 6.0f);
                    startPos = moveTransform.position;
                    endPos = initialPos + new Vector3(Random.Range(-Range, Range), 0.0f, Random.Range(-Range, Range));
                }
                moveTransform.position = Vector3.Lerp(startPos, endPos, 1.0f - (currentDuration / totalDuration));
            }
            if (_light.type != LightType.Area)
            {
                _light.intensity = baseIntensity * (0.5f + Mathf.PerlinNoise(Time.timeSinceLevelLoad * 0.01f, baseIntensity + _light.range));
            }
        }
    }
}