using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarInstance : MonoBehaviour
{
    protected SpriteRenderer visuals;
    private Vector2 _StarSpawnPoint;
    private bool _isActive = false;

    private Color transparent;
    private Color opaque;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    public Vector2 StarSpawnPoint { get => _StarSpawnPoint; set => _StarSpawnPoint = value; }

    private void Awake()
    {
        visuals = this.GetComponent<SpriteRenderer>();
        if(visuals)
        {
            transparent = new Color(visuals.color.r, visuals.color.g, visuals.color.b, 0);
            opaque = new Color(visuals.color.r, visuals.color.g, visuals.color.b, 255);
        }

        this.transform.position = StarSpawnPoint;
        visuals.color = opaque;
    }


    public void Activate(Vector2 newPos)
    {
        Debug.Log("Star activating going to pos " + newPos.x + " | " + newPos.y);
        visuals.color = transparent;
        IsActive = true;
        // activate them (and they do their own silly things)
        this.transform.position = newPos;
        visuals.color = opaque;
        gameObject.SetActive(true);
        StartCoroutine(BirthStar());
    }

    IEnumerator BirthStar()
    {
        Debug.Log("Birth");
        // iterate from transparent to opaque
        // shine effect when fully opaque ?
        yield return new WaitForSeconds(1.0f);
        DeathStar();
    }

    private void DeathStar()
    {
        Debug.Log("Death");
        // iterate from opaque to transparent
        // disables itself afterwards
        visuals.color = opaque;
        this.transform.position = StarSpawnPoint;
        gameObject.SetActive(false);
    }
    void ChangeColor(Color newColor)
    {
        visuals.color = newColor;
    }
}
