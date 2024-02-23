using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_Target : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static Prototype_Target instance;

    private SpriteRenderer _spriteRenderer;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        HideTarget();
    }

    public void ShowTarget()
    {
        _spriteRenderer.enabled = true;
    }

    public void HideTarget()
    {
        _spriteRenderer.enabled = false;
    }

    public void ChangeTargetPosition(Vector3 pos)
    {
        ShowTarget();
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    public void ChangeTargetColorToRed()
    {
        Debug.Log("target red");
        _spriteRenderer.color = Color.red;
    }

    public void ChangeTargetColorToWhite()
    {
        Debug.Log("target white");
        _spriteRenderer.color = Color.white;
    }

    public bool CanShootOnThisTile()
    {
        if (_spriteRenderer.color == Color.white)
        {
            return true;
        }

        return false;
    }
}
