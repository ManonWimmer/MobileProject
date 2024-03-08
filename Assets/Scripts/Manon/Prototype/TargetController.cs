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

    #region Color
    public void ChangeTargetColorToRed()
    {
        Debug.Log("target red (desactivated)");
        //_spriteRenderer.color = Color.magenta;
        UIManager.instance.CheckAbilityButtonsColor();
    }

    public void ChangeTargetColorToWhite()
    {
        Debug.Log("target white");
        //_spriteRenderer.color = Color.magenta; // white on voit pas avec les sprite blanches temp, a changer plus tard avec les assets
    }
    #endregion

    public bool CanShootOnThisTile()
    {
        return true;
    }
}
