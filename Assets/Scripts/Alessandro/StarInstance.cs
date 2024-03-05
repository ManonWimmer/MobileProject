using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarInstance : MonoBehaviour
{
    protected SpriteRenderer visuals;

    private void Start()
    {
        visuals = this.GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        visuals.color = new Color(visuals.color.r, visuals.color.g, visuals.color.b, 255);
        BirthStar();
    }

    IEnumerator BirthStar()
    {
        gameObject.SetActive(true);
        // iterate from transparent to opaque
        // shine effect when fully opaque ?
        yield return new WaitForSeconds(1.0F);
        DeathStar();
    }

    private void DeathStar()
    {
        // iterate from opaque to transparent
        // disables itself afterwards
        gameObject.SetActive(false);
    }
    void ChangeColor(Color newColor)
    {
        visuals.color = newColor;
    }
}
