using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // ----- FIELDS ----- //
    public bool IsOccupied;
    public bool IsReavealed;
    public bool IsDestroyed;
    public bool IsMissed;
    public bool IsAbilitySelected;

    public Room Room;

    public int Row;
    public int Column;

    public Tile LeftTile;
    public Tile RightTile;
    public Tile TopTile;
    public Tile BottomTile;

    public Tile DiagBottomLeftTile;
    public Tile DiagBottomRightTile;
    public Tile DiagTopLeftTile;
    public Tile DiagTopRightTile;

    public List<Tile> RoomOnOtherTiles = new List<Tile>();

    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer RoomTileSpriteRenderer;
    // ----- FIELDS ----- //

    private void Start()
    {
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.instance.GetCurrentMode() == Mode.Construction)
        {
            if (IsOccupied)
            {
                _spriteRenderer.color = Color.red;
            }
            else
            {
                _spriteRenderer.color = Color.green;
            }
        }
        else
        {
            if (IsDestroyed || IsMissed || IsReavealed)
            {
                _spriteRenderer.color = Color.red;
            }
            else if (IsAbilitySelected && ActionPointsManager.instance.TryUseActionPoints(GameManager.instance.PlayerTurn) && AbilityButtonsManager.instance.GetCurrentlySelectedAbilityButton() != null)
            {
                if (GameManager.instance.IsAbilityInCooldown(AbilityButtonsManager.instance.GetCurrentlySelectedAbilityButton().GetAbility()))
                {
                    _spriteRenderer.color = Color.black;
                }
                else
                {
                    _spriteRenderer.color = Color.white;
                }   
            }
            else
            {
                _spriteRenderer.color = Color.black;
            }
        }
    }
}
