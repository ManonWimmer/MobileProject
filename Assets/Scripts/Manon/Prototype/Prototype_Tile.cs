using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_Tile : MonoBehaviour
{
    public bool IsOccupied;
    public bool IsDestroyed;
    public Prototype_Building Building;

    public int Row;
    public int Column;

    public Prototype_Tile LeftTile;
    public Prototype_Tile RightTile;
    public Prototype_Tile TopTile;
    public Prototype_Tile BottomTile;

    public List<Prototype_Tile> BuildingOnOtherTiles = new List<Prototype_Tile>();

    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer BuildingTileSpriteRenderer;

    // is destroyed
    // player -> parent grid player
    // is vital

    private void Start()
    {
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Prototype_GameManager.instance.GetCurrentMode() == Mode.Construction)
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
            _spriteRenderer.color = Color.black;
        }
    }
}
