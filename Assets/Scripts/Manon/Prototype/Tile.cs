using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // ----- FIELDS ----- //
    public bool IsOccupied;
    public bool IsDestroyed;
    public bool IsMissed;

    public Room Room;

    public int Row;
    public int Column;

    public Tile LeftTile;
    public Tile RightTile;
    public Tile TopTile;
    public Tile BottomTile;

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
            if (IsDestroyed || IsMissed)
            {
                _spriteRenderer.color = Color.red;
            }
            else
            {
                _spriteRenderer.color = Color.black;
            }
        }
    }
}
