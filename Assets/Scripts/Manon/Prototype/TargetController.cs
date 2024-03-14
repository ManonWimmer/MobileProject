using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static TargetController instance;
    private bool _canShoot;
    private SpriteRenderer _spriteRenderer;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //_spriteRenderer = GetComponent<SpriteRenderer>();

        HideTarget();
    }

    #region Show / Hide
    public void ShowTarget()
    {
        if (!AbilityButtonsManager.instance.IsInRewind)
        {
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = true;
            }
        }
    }

    public void HideTarget()
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
    }
    #endregion

    public void ChangeTargetPosition(Vector3 pos)
    {
        Debug.Log("change target position");
        ShowTarget();
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    public bool CanShootOnThisTile()
    {
        return true;
    }
}
