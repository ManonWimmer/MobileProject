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

    private Sprite _normal;
    private Sprite _revealed;
    private Sprite _destroyed;

    private SpriteRenderer _bordure;
    private SpriteRenderer _inside;
    public SpriteRenderer RoomTileSpriteRenderer;
    // ----- FIELDS ----- //

    private void Start()
    {
        _bordure = transform.GetComponent<SpriteRenderer>();
        _inside = transform.GetChild(0).GetComponent<SpriteRenderer>();

        _normal = RoomsAssetsManager.instance.GetNormalTile();
        _revealed = RoomsAssetsManager.instance.GetReavealedTile();
        _destroyed = RoomsAssetsManager.instance.GetDestroyedTile();
    }

    private void Update()
    {
        if (GameManager.instance.GetCurrentMode() == Mode.Construction)
        {
            if (IsOccupied)
            {
                _bordure.color = Color.red;
            }
            else
            {
                _bordure.color = Color.green;
            }

            _inside.sprite = _normal;
        }
        else
        {
            if (IsDestroyed || IsMissed)
            {
                _bordure.color = Color.red;
            }
            else if (IsReavealed)
            {
                _bordure.color = Color.green;
            }
            else if (IsAbilitySelected && ActionPointsManager.instance.TryUseActionPoints(GameManager.instance.PlayerTurn) && AbilityButtonsManager.instance.GetCurrentlySelectedAbilityButton() != null)
            {
                if (GameManager.instance.IsAbilityInCooldown(AbilityButtonsManager.instance.GetCurrentlySelectedAbilityButton().GetAbility()))
                {
                    _bordure.color = Color.black;
                }
                else
                {
                    _bordure.color = Color.white;
                }   
            }
            else
            {
                _bordure.color = Color.black;
            }

            if (IsDestroyed || IsMissed)
            {
                _inside.sprite = _destroyed;
            }
            else if (IsReavealed)
            {
                _inside.sprite = _revealed;
            }
        }
    }
}
