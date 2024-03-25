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

    public bool IsDragging { get => isDragging; set => isDragging = value; }

    // ----- FIELDS ----- //

    void OnMouseDown()
    {
        if (GameManager.instance.GetCurrentMode() == Mode.Construction)
        {
            if (isDragging)
                return;

            // Store offset between touch position and object center
            offset = transform.position - CameraController.instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;

            Tile tile = GameManager.instance.FindNearestTileInGridFromInputPosition(GameManager.instance.PlayerTurn);
            
            if (tile == null)
            {
                firstTile = null;
                return;
            }


            if (tile.Room != null)
            {
                /*
                // faut que je get la tile ou y'a le center sprite renderer surement
                if (tile.RoomTileSpriteRenderer.transform.parent == null) // c'est pas la center (la center a pas de parent)
                {
                    foreach (Tile otherTile in tile.RoomOnOtherTiles)
                    {
                        if (otherTile.RoomTileSpriteRenderer.transform.parent == null) // c'est le center
                        {
                            tile = otherTile;
                            break;
                        }
                    }
                }
                */ 


                UIManager.instance.ShowFicheRoom(tile.Room.RoomData);
                tile.IsMovingConstruction = true;
                GameManager.instance.SetBuildingTilesNotOccupied(this, tile);
                firstTile = tile;
                Debug.Log("set first tile : " + firstTile.name);
            }
            else
            {
                firstTile = null;
            }
            
        }
        else if (GameManager.instance.GetCurrentMode() == Mode.Combat && GameManager.instance.TargetIsDestroyed() || GameManager.instance.TargetIsVisible())
        {
            UIManager.instance.ShowFicheRoom(RoomData);
        }
    }


    void OnMouseDrag()
    {
        if (firstTile == null)
        {
            isDragging = false;
            return;
        }
            

        if (GameManager.instance.GetCurrentMode() != Mode.Construction)
        {
            isDragging = false;
            return;
        }

        if (Input.GetMouseButtonDown(0) || !isDragging) 
            return;


        Tile tile = GameManager.instance.FindNearestTileInGridFromRoom(GameManager.instance.PlayerTurn, this);
        if (tile != null)
        {
            if (GameManager.instance.CheckCanBuild(this, tile))
            {
                Debug.Log("set end drag tile " + tile.name);
                endDragTile = tile;
                GameManager.instance.SetBuildingTilesNotOccupied(this, tile);
            }
        }
        
        // Move room with mouse / finger
        Vector3 newPosition = CameraController.instance.MainCamera.ScreenToWorldPoint(Input.mousePosition) + offset;
        transform.position = new Vector3(newPosition.x, newPosition.y, -1f);
    }

    void OnMouseUp()
    {
        SetPositionAtLastPosMouse(); 

        //UIManager.instance.HideFicheRoom();
    }

    public void SetPositionAtLastPosMouse()
    {
        Debug.Log("set position at last pos mouse");
        isDragging = false;

        if (firstTile == null)
        {
            Debug.Log("return first tile null");
            return;
        }
            
        if (GameManager.instance.GetCurrentMode() != Mode.Construction)
            return;

        //Debug.Log("end drag tile " + endDragTile.name);

        if (endDragTile == null)
        {
            Debug.Log("set pos at first tile");
            firstTile.Room = this;
            firstTile.IsOccupied = true;

            transform.position = new Vector3(firstTile.transform.position.x, firstTile.transform.position.y, -0.5f);

            Debug.Log(firstTile.name);
            GameManager.instance.SetBuildingTilesOccupied(this, firstTile);
        }
        else
        {
            Debug.Log("set pos at end tile");
            endDragTile.Room = this;
            endDragTile.IsOccupied = true;

            transform.position = new Vector3(endDragTile.transform.position.x, endDragTile.transform.position.y, -0.5f);
            
            Debug.Log(endDragTile.name);
            GameManager.instance.SetBuildingTilesOccupied(this, endDragTile);

            firstTile.IsMovingConstruction = false;
        }

        endDragTile = null;
        firstTile = null;
    }
}
