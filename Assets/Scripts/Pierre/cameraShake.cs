using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    public static cameraShake instance;

    private Vector3 originalPosition;

    public void StartShakeCamera(float duration, float force) => Shake(duration, force);
    public void StartPhoneVibrate(int milliseconds) => PhoneVibrate(milliseconds);
    public void StartPhoneAndCameraVibrate(float durationShake, float forceShake, int millisecondsVibration) {PhoneVibrate(millisecondsVibration); Shake(durationShake, forceShake);}

    private void Awake()
    {
        instance = this;
    }

    private void PhoneVibrate(int milliseconds)
    {
        Vibrator.Vibrate(milliseconds);
    }

    private void Shake(float duration, float force)
    {
        originalPosition = transform.localPosition;
        StartCoroutine(ShakeCoroutine(duration, force));
    }


    private IEnumerator ShakeCoroutine(float d, float f)
    {
        float elapsedTime = 0f;

        while (elapsedTime < d)
        {

            Vector3 randomOffset = Random.insideUnitSphere * f;


            transform.localPosition = originalPosition + randomOffset;


            yield return null;


            elapsedTime += Time.deltaTime;
        }


        transform.localPosition = originalPosition;
    }
}
