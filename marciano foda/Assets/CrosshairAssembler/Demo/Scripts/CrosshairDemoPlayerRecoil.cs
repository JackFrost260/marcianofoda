using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairDemoPlayerRecoil : MonoBehaviour
{
    public GameObject Projectile;
    public Transform Gun;
    public Transform Aim;
    public float ShootRate = 0.086f;
    public float ShootForce = 300;

    private float firingTimeStamp;
    private float accumulatedRecoil;
    private float recoveringRecoilTimeStamp;
    private const float exponentialAlpha = 0.8f;
    private float rPrevious;

    void Update()
    {
        // firing
        firingTimeStamp = Mathf.Max(0, firingTimeStamp - Time.deltaTime);
        accumulatedRecoil = Mathf.Max(0, accumulatedRecoil - (Time.deltaTime * 3));
        recoveringRecoilTimeStamp += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && firingTimeStamp <= 0)
        {
            firingTimeStamp = ShootRate;
            accumulatedRecoil = 1f;
            recoveringRecoilTimeStamp = 0;

            // shoot a projectile
            GameObject go = (GameObject)Instantiate(Projectile, Gun.position, Gun.rotation);
            var recoil = Mathf.Max(0, CrosshairLibs.GetRecoil() * CrosshairLibs.GetRecoilMultiplierPixels()) * 0.003f;
            var recoilX = recoil * (Random.value - 0.5f);
            var recoilY = recoil * (Random.value - 0.5f);
            var randomForwardByRecoil = new Vector3(recoilX, recoilY, 0);
            var foward = (Aim.position - Gun.position) + randomForwardByRecoil * Aim.position.z;
            go.GetComponent<Rigidbody>().AddForce(foward * ShootForce);
        }

        float rEffectiveRecoil = (accumulatedRecoil * exponentialAlpha) + (rPrevious * (1 - exponentialAlpha)); // exponential moving average
        rPrevious = rEffectiveRecoil;

        CrosshairLibs.SetRecoil(rEffectiveRecoil);
    }
}
