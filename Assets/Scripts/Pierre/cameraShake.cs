using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.5f;

    private Vector3 originalPosition;

    private void Start()
    {
        Shake();
    }

    public void Shake()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(ShakeCoroutine());
    }


    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {

            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;


            transform.localPosition = originalPosition + randomOffset;


            yield return null;


            elapsedTime += Time.deltaTime;
        }


        transform.localPosition = originalPosition;
    }
}
