using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButton : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] scriptablePower _ability;
    private SpriteRenderer _spriteRenderer;
    private bool _isSelected;
    // ----- FIELDS ----- //

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public scriptablePower GetAbility()
    {
        return _ability;
    }
}
