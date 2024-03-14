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
    public bool IsMissedReavealed;
    public bool IsMissedDestroyed;
    public bool IsAbilitySelectedDestroy;
    public bool IsAbilitySelectedReveal;
    public bool IsMovingConstruction;

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
            _bordure.color = new Color(00.5f, 0.5f, 0.5f, 1); // gray
            _inside.sprite = _normal;
        }
        else
        {
            if ((IsDestroyed || IsMissedDestroyed)) 
            {
                _bordure.color = Color.red; // selectionnée ou pas c'est rouge
                _inside.sprite = _destroyed;
            }
            else if ((IsReavealed || IsMissedReavealed))
            {
                if (IsAbilitySelectedDestroy)
                    _bordure.color = Color.white;
                else
                    _bordure.color = Color.green;

                _inside.sprite = _revealed;
            }
            else if (IsAbilitySelectedDestroy || IsAbilitySelectedReveal)
            {
                _bordure.color = Color.white;
                _inside.sprite = _normal;
            } 
            else
            {
                _bordure.color = new Color(00.5f, 0.5f, 0.5f, 1); // gray
                _inside.sprite = _normal;
            }
        }
    }
}
