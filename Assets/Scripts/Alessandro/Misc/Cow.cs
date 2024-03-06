using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Cow : MonoBehaviour
{
    AudioSource moo;
    private bool isUpwards = true;
    public float speed = 1.0f;
    
    private void Start()
    {
        moo = GetComponent<AudioSource>();
}
public void moo_rotate()
    {
        isUpwards = !isUpwards;
        Debug.Log("Rotating " + isUpwards);
        if (isUpwards)
        {
            moo.Play(0);
            transform.rotation = Quaternion.Euler(0, 0, 0);

        }
        else
        {
            moo.Stop();
            transform.rotation = Quaternion.Euler(180, 0, 0);
        }

    }
}
