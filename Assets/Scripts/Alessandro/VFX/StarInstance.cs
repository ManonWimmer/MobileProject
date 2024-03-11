using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StarInstance : MonoBehaviour
{
    protected SpriteRenderer visuals;
    private Vector2 _StarSpawnPoint;
    private bool _isActive = false;

    private Color transparent;
    private Color opaque;
    private float fadeTime = 1.0F;
    private float starLifetime = 1.0F;

    [SerializeField] float _speed = 2.0f;
    [SerializeField] float duration = 4.0f;
    private Vector2 destination = new Vector2(-9, 0);
    private float t = 0f;


    float randomFactor = 1.0f;

    Vector2 iterationOrigin;
    public bool IsActive { get => _isActive; set => _isActive = value; }
    public Vector2 StarSpawnPoint { get => _StarSpawnPoint; set => _StarSpawnPoint = value; }

    private void Awake()
    {
        visuals = this.GetComponent<SpriteRenderer>();
        this.transform.position = new Vector2(0, 10);
        if (visuals)
        {
            transparent = new Color(visuals.color.r, visuals.color.g, visuals.color.b, 0);
            opaque = new Color(visuals.color.r, visuals.color.g, visuals.color.b, 255);

        }
        IsActive = false;
        Reset();
    }


    private void Update()
    {
        if(IsActive)
        {
            //t = Mathf.Repeat(Time.time * _speed, duration) / duration;
            //transform.position = Vector2.Lerp(iterationOrigin, destination, t);
            // Move the star to the left
            t = _speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, destination, t);
            // if we get futher than the destination, reset the position of the star and its destination
            if (transform.position.x < destination.x + 0.5)
            {
                transform.position = new Vector2(UnityEngine.Random.Range(iterationOrigin.x - (6 +randomFactor), iterationOrigin.x + 6),
                    UnityEngine.Random.Range(iterationOrigin.y - 6, iterationOrigin.y + (6 + randomFactor)));
                destination = new Vector2(-9, transform.position.y);

            }
        }
    }


    public void Activate(Vector2 newPos)
    {
        // setup
        // random factor to make everything seem less "samey"
        randomFactor = UnityEngine.Random.Range(0.7f, 2f);
        duration *= randomFactor;
        _speed *= randomFactor;
        //Debug.Log("ACTIVATE NEWPOS");
        visuals.color = transparent;
        this.transform.position = newPos;
        iterationOrigin = newPos;
        destination = new Vector2(-9, iterationOrigin.y);
        // activate them (and they do their own silly things)
        IsActive = true;
        visuals.color = opaque;
        gameObject.SetActive(true);
        StartCoroutine(BirthStar());
    }

    IEnumerator BirthStar()
    {
        //Debug.Log("Birth");
        yield return StartCoroutine(FadeInAndOutLoop());
        //Reset();
    }
    void ChangeColor(Color newColor)
    {
        visuals.color = newColor;
    }

    public void Reset()
    {
        StopAllCoroutines();
        this.transform.position = new Vector2(0, 10);
        visuals.color = opaque;
        IsActive = false;
    }

    IEnumerator FadeInAndOutLoop()
    {
        while (1 == 1)
        {
            for (float i = 0.2f; i < fadeTime + randomFactor; i += Time.deltaTime)
            {
                visuals.color = new Color(visuals.color.r, visuals.color.g, visuals.color.b, i);
                yield return null;
            }
            yield return new WaitForSeconds(starLifetime);
            for (float i = fadeTime - randomFactor; i >= 0.2f; i -= Time.deltaTime)
            {
                visuals.color = new Color(visuals.color.r, visuals.color.g, visuals.color.b, i);
                yield return null;
            }
        }
    }
}
