using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

public class Prototype_GameManager : MonoBehaviour
{
    [SerializeField] GameObject _grid;
    //[SerializeField] Prototype_CustomCursor _customCursor;
    [SerializeField] List<Prototype_Building> _startBuildings = new List<Prototype_Building>();

    private Dictionary<Tuple<int, int>, Prototype_Tile> _dictTilesRowColumn = new Dictionary<Tuple<int, int>, Prototype_Tile>();

    private List<Prototype_Tile> _tiles = new List<Prototype_Tile>();
    private Prototype_Building _buildingToPlace;
    private Prototype_Building _buildingToMove;
    private Prototype_Building _buildingOnMouse;



    private void Start()
    {
        foreach (Prototype_Tile tile in _grid.GetComponentsInChildren<Prototype_Tile>())
        {
            _tiles.Add(tile);
            int row = tile.Row;
            int column = tile.Column;
            _dictTilesRowColumn[new Tuple<int, int>(row, column)] = tile;
        }

        // search adjacent tiles for each tile
        foreach (Prototype_Tile tile in _tiles)
        {
            int row = tile.Row;
            int column = tile.Column;

            // top
            if (_dictTilesRowColumn.ContainsKey(new Tuple<int, int>(row - 1, column)))
            {
                tile.TopTile = _dictTilesRowColumn[new Tuple<int, int>(row - 1, column)];
            }

            // bottom
            if (_dictTilesRowColumn.ContainsKey(new Tuple<int, int>(row + 1, column)))
            {
                tile.BottomTile = _dictTilesRowColumn[new Tuple<int, int>(row + 1, column)];
            }

            // right
            if (_dictTilesRowColumn.ContainsKey(new Tuple<int, int>(row, column + 1)))
            {
                tile.RightTile = _dictTilesRowColumn[new Tuple<int, int>(row, column + 1)];
            }

            // left
            if (_dictTilesRowColumn.ContainsKey(new Tuple<int, int>(row, column - 1)))
            {
                tile.LeftTile = _dictTilesRowColumn[new Tuple<int, int>(row, column - 1)];
            }
        }

        if (_startBuildings.Count > 0)
        {
            foreach (Prototype_Building startBuilding in _startBuildings)
            {
                bool buildingBuilt = false;
                Debug.Log(startBuilding.name);
                while (!buildingBuilt)
                {
                    Prototype_Tile tempTile = _tiles[Random.Range(0, _tiles.Count - 1)];
                    if (CheckCanBuild(startBuilding, tempTile))
                    {
                        CreateNewBuilding(startBuilding, tempTile);
                        buildingBuilt = true;
                    }
                }           
            }
        }
    }

    private void Update()
    {
        if (_buildingOnMouse != null) // update building on mouse pos
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _buildingOnMouse.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Prototype_Tile nearestTile = null;
            float shortestDistance = float.MaxValue;

            // Find nearest tile  
            foreach(Prototype_Tile tile in _tiles)
            {
                float distance = Vector2.Distance(tile.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTile = tile;
                }
            }

            if (!nearestTile.IsOccupied) // no building
            {
                if (_buildingToPlace != null)
                {
                    if (CheckCanBuild(_buildingToPlace, nearestTile))
                    {
                        CreateNewBuilding(_buildingToPlace, nearestTile);
                    }
                }
                else if (_buildingToMove != null)
                {
                    if (CheckCanBuild(_buildingToMove, nearestTile))
                    {
                        CreateNewBuilding(_buildingToMove, nearestTile);
                    }
                }
            }
            else // already a building
            {
                Debug.Log("occupied");
                if (_buildingToMove == null)
                {
                    Debug.Log("new building to move");
                    // select move building
                    _buildingToMove = nearestTile.Building;
                    nearestTile.IsOccupied = false;

                    SetBuildingTilesNotOccupied(_buildingToMove, nearestTile);

                    // building to mouse
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _buildingOnMouse = _buildingToMove;
                    //_buildingOnMouse = Instantiate(_buildingToMove, new Vector3(mousePosition.x, mousePosition.y, -5), Quaternion.identity);

                    //Destroy(nearestTile.Building.gameObject);
                }
            }
        }
    }

    public void TakeBuilding(Prototype_Building building)
    {
        _buildingToPlace = building;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _buildingOnMouse = Instantiate(_buildingToPlace, new Vector3(mousePosition.x, mousePosition.y, -5), Quaternion.identity);
    }

    private bool CheckCanBuild(Prototype_Building building, Prototype_Tile tile)
    {
        // center
        if (tile.IsOccupied) 
        {
            Debug.Log("can't build at center");
            return false;
        }

        // left
        if (building.LeftTiles > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.LeftTiles; i++)
            {
                if (tile.LeftTile != null)
                {
                    if (tile.LeftTile.IsOccupied)
                    {
                        Debug.Log("left tile occupied " + i);
                        return false;
                    } 
                }
                else
                {
                    Debug.Log("no tile at left " + i);
                    return false;
                }

                currentTile = tile.LeftTile;
            }
        }
        // right
        if (building.RightTiles > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.RightTiles; i++)
            {
                if (tile.RightTile != null)
                {
                    if (tile.RightTile.IsOccupied)
                    {
                        Debug.Log("right tile occupied " + i);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("no tile at right " + i);
                    return false;
                }

                currentTile = tile.RightTile;
            }
        }
        // bottom
        if (building.BottomTiles > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.BottomTiles; i++)
            {
                if (tile.BottomTile != null)
                {
                    if (tile.BottomTile.IsOccupied)
                    {
                        Debug.Log("bottom tile occupied " + i);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("no tile at bottom " + i);
                    return false;
                }

                currentTile = tile.BottomTile;
            }
        }
        // top
        if (building.TopTiles > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.TopTiles; i++)
            {
                if (tile.TopTile != null)
                {
                    if (tile.TopTile.IsOccupied)
                    {
                        Debug.Log("bottom tile occupied " + i);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("no tile at bottom " + i);
                    return false;
                }

                currentTile = tile.TopTile;
            }
        }


        Debug.Log("can build");
        return true;
    }

    private void CreateNewBuilding(Prototype_Building building, Prototype_Tile tile)
    {
        // place new building
        //Prototype_Building newBuilding = _buildingOnMouse;
        Debug.Log("instantiate new building");
        Prototype_Building newBuilding = Instantiate(building, new Vector3(tile.transform.position.x, tile.transform.position.y, -5), Quaternion.identity);
        //newBuilding.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -5); // adjust to tile position

        tile.Building = newBuilding;
        tile.IsOccupied = true;

        SetBuildingTilesOccupied(newBuilding, tile);

        _buildingToPlace = null;

        // no building on mouse
        Debug.Log("destroy on mouse");
        if (_buildingOnMouse != null)
        {
            Destroy(_buildingOnMouse.gameObject);
        }
    }

    private void SetBuildingTilesOccupied(Prototype_Building building, Prototype_Tile tile)
    {
        List<Prototype_Tile> tiles = new List<Prototype_Tile>();
        tiles.Add(tile); // center

        // left
        if (building.LeftTiles > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.LeftTiles; i++)
            {
                tile.LeftTile.IsOccupied = true;
                tile.LeftTile.Building = building;

                currentTile = tile.LeftTile;
                tiles.Add(currentTile);
            }
        }
        // right
        if (building.RightTiles > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.RightTiles; i++)
            {
                tile.RightTile.IsOccupied = true;
                tile.RightTile.Building = building;

                currentTile = tile.RightTile;
                tiles.Add(currentTile);
            }
        }
        // top
        if (building.TopTiles > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.TopTiles; i++)
            {
                tile.TopTile.IsOccupied = true;
                tile.TopTile.Building = building;

                currentTile = tile.TopTile; 
                tiles.Add(currentTile);
            }
        }
        // bottom
        if (building.BottomTiles > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.BottomTiles; i++)
            {
                tile.BottomTile.IsOccupied = true;
                tile.BottomTile.Building = building;

                currentTile = tile.BottomTile;
                tiles.Add(currentTile);
            }
        }
        Debug.Log(tiles.Count);


        foreach (Prototype_Tile buildingTile in tiles)
        {
            foreach (Prototype_Tile buildingTile2 in tiles)
            {
                if (buildingTile != buildingTile2)
                {
                    buildingTile.BuildingOnOtherTiles.Add(buildingTile2);
                }
            }
        }

        
    }

    private void SetBuildingTilesNotOccupied(Prototype_Building building, Prototype_Tile tile)
    {
        List<Prototype_Tile> tiles = new List<Prototype_Tile>();
        tiles.Add(tile); // center

        foreach(Prototype_Tile tile2 in tile.BuildingOnOtherTiles)
        {
            tiles.Add(tile2);
        }

        foreach (Prototype_Tile tile3 in tiles)
        {
            tile3.IsOccupied = false;
            tile3.BuildingOnOtherTiles.Clear();
        }
    }
}
