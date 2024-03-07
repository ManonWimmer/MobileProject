using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // ----- FIELDS ----- //
    public SpriteRenderer CenterTileSR;
    public List<SpriteRenderer> LeftTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> RightTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> TopTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> BottomTilesSR = new List<SpriteRenderer>();

    public List<SpriteRenderer> DiagBottomLeftTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> DiagBottomRightTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> DiagTopLeftTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> DiagTopRightTilesSR = new List<SpriteRenderer>();

    public RoomSO RoomData;

    public bool IsRoomDestroyed;

    Tile endDragTile;
    Tile firstTile;

    private bool isDragging;
    private Vector3 offset;
    // ----- FIELDS ----- //

    void OnMouseDown()
    {
        if (GameManager.instance.GetCurrentMode() != Mode.Construction)
            return;

        if (isDragging)
            return;


        // Store offset between touch position and object center
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;

        Tile tile = GameManager.instance.FindNearestTileInGridFromInputPosition(GameManager.instance.PlayerTurn);

        UIManager.instance.ShowFicheRoom(tile.Room.RoomData);
        GameManager.instance.SetBuildingTilesNotOccupied(this, tile);

        firstTile = tile;
    }

    void OnMouseDrag()
    {
        if (GameManager.instance.GetCurrentMode() != Mode.Construction)
            return;

        if (Input.GetMouseButtonDown(0) || !isDragging)
            return;

        Tile tile = GameManager.instance.FindNearestTileInGridFromRoom(GameManager.instance.PlayerTurn, this);
        if (tile != null)
        {
            if (GameManager.instance.CheckCanBuild(this, tile))
            {
                Debug.Log("set end drag tile " + tile.name);
                endDragTile = tile;
            }
        }
        
        // Move room with mouse / finger
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        transform.position = new Vector3(newPosition.x, newPosition.y, -1f);
    }

    void OnMouseUp()
    {
        if (GameManager.instance.GetCurrentMode() != Mode.Construction)
            return;

        //Debug.Log("end drag tile " + endDragTile.name);

        if (endDragTile == null)
        {
            firstTile.Room = this;
            firstTile.IsOccupied = true;

            transform.position = new Vector3(firstTile.transform.position.x, firstTile.transform.position.y, -0.5f);
            isDragging = false;
            //GameManager.instance.SetBuildingTilesOccupied(this, firstTile);
        }
        else
        {
            endDragTile.Room = this;
            endDragTile.IsOccupied = true;

            transform.position = new Vector3(endDragTile.transform.position.x, endDragTile.transform.position.y, -0.5f);
            isDragging = false;
            GameManager.instance.SetBuildingTilesOccupied(this, endDragTile);
        }
       
        endDragTile = null;

        UIManager.instance.HideFicheRoom();
    }
}
