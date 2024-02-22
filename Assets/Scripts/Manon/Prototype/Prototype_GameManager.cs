using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static UnityEditor.Experimental.GraphView.GraphView;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

public enum Player
{
    Player1,
    Player2
}

public enum Mode
{
    Construction, 
    Combat
}

public class Prototype_GameManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static Prototype_GameManager instance;

    [SerializeField] GameObject _gridPlayer1;
    [SerializeField] GameObject _gridPlayer2;
    [SerializeField] List<Prototype_Building> _startBuildings = new List<Prototype_Building>();

    private Dictionary<Tuple<int, int>, Prototype_Tile> _dictTilesRowColumnPlayer1 = new Dictionary<Tuple<int, int>, Prototype_Tile>();
    private Dictionary<Tuple<int, int>, Prototype_Tile> _dictTilesRowColumnPlayer2 = new Dictionary<Tuple<int, int>, Prototype_Tile>();

    private List<Prototype_Tile> _tilesPlayer1 = new List<Prototype_Tile>();
    private List<Prototype_Tile> _tilesPlayer2 = new List<Prototype_Tile>();
    private Prototype_Building _buildingToPlace;
    private Prototype_Building _buildingToMove;
    private Prototype_Building _buildingOnMouse;

    private Player _playerTurn;
    private Mode _currentMode = Mode.Construction;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        #region StartConstruction
        // Player 1
        foreach (Prototype_Tile tile in _gridPlayer1.GetComponentsInChildren<Prototype_Tile>())
        {
            _tilesPlayer1.Add(tile);
            int row = tile.Row;
            int column = tile.Column;
            _dictTilesRowColumnPlayer1[new Tuple<int, int>(row, column)] = tile;
        }

        // search adjacent tiles for each tile
        foreach (Prototype_Tile tile in _tilesPlayer1)
        {
            int row = tile.Row;
            int column = tile.Column;

            // top
            if (_dictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row - 1, column)))
            {
                tile.TopTile = _dictTilesRowColumnPlayer1[new Tuple<int, int>(row - 1, column)];
            }

            // bottom
            if (_dictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row + 1, column)))
            {
                tile.BottomTile = _dictTilesRowColumnPlayer1[new Tuple<int, int>(row + 1, column)];
            }

            // right
            if (_dictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row, column + 1)))
            {
                tile.RightTile = _dictTilesRowColumnPlayer1[new Tuple<int, int>(row, column + 1)];
            }

            // left
            if (_dictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row, column - 1)))
            {
                tile.LeftTile = _dictTilesRowColumnPlayer1[new Tuple<int, int>(row, column - 1)];
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
                    Prototype_Tile tempTile = _tilesPlayer1[Random.Range(0, _tilesPlayer1.Count - 1)];
                    if (CheckCanBuild(startBuilding, tempTile))
                    {
                        CreateNewBuilding(startBuilding, tempTile);
                        buildingBuilt = true;
                    }
                }           
            }
        }

        // Same for player 2
        foreach (Prototype_Tile tile in _gridPlayer2.GetComponentsInChildren<Prototype_Tile>())
        {
            _tilesPlayer2.Add(tile);
            int row = tile.Row;
            int column = tile.Column;
            _dictTilesRowColumnPlayer2[new Tuple<int, int>(row, column)] = tile;
        }

        // search adjacent tiles for each tile
        foreach (Prototype_Tile tile in _tilesPlayer2)
        {
            int row = tile.Row;
            int column = tile.Column;

            // top
            if (_dictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row - 1, column)))
            {
                tile.TopTile = _dictTilesRowColumnPlayer2[new Tuple<int, int>(row - 1, column)];
            }

            // bottom
            if (_dictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row + 1, column)))
            {
                tile.BottomTile = _dictTilesRowColumnPlayer2[new Tuple<int, int>(row + 1, column)];
            }

            // right
            if (_dictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row, column + 1)))
            {
                tile.RightTile = _dictTilesRowColumnPlayer2[new Tuple<int, int>(row, column + 1)];
            }

            // left
            if (_dictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row, column - 1)))
            {
                tile.LeftTile = _dictTilesRowColumnPlayer2[new Tuple<int, int>(row, column - 1)];
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
                    Prototype_Tile tempTile = _tilesPlayer2[Random.Range(0, _tilesPlayer2.Count - 1)];
                    if (CheckCanBuild(startBuilding, tempTile))
                    {
                        CreateNewBuilding(startBuilding, tempTile);
                        buildingBuilt = true;
                    }
                }
            }
        }
        #endregion

        // Update UI
        Prototype_ManagerUI.instance.UpdateCurrentPlayerTxt(_playerTurn);
        Prototype_ManagerUI.instance.UpdateCurrentModeTxt(_currentMode);
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
            Prototype_Tile nearestTileGridPlayer = null;

            // Select tile in specific grid
            if (_currentMode == Mode.Construction)
            {
                nearestTileGridPlayer = FindNearestTileInGrid(_playerTurn);
            }
            else
            {
                // combat -> tile dans la grid du vaisseau ennemi
                if (_playerTurn == Player.Player1)
                {
                    nearestTileGridPlayer = FindNearestTileInGrid(Player.Player2);
                }
                else
                {
                    nearestTileGridPlayer = FindNearestTileInGrid(Player.Player1);
                }
            }

            // construction
            if (_currentMode == Mode.Construction && nearestTileGridPlayer != null)
            {
                #region ClickOnTileInConstruction
                if (!nearestTileGridPlayer.IsOccupied) // no building
                {
                    if (_buildingToPlace != null)
                    {
                        if (CheckCanBuild(_buildingToPlace, nearestTileGridPlayer))
                        {
                            CreateNewBuilding(_buildingToPlace, nearestTileGridPlayer);
                        }
                    }
                    else if (_buildingToMove != null)
                    {
                        if (CheckCanBuild(_buildingToMove, nearestTileGridPlayer))
                        {
                            CreateNewBuilding(_buildingToMove, nearestTileGridPlayer);
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
                        _buildingToMove = nearestTileGridPlayer.Building;
                        nearestTileGridPlayer.IsOccupied = false;

                        SetBuildingTilesNotOccupied(_buildingToMove, nearestTileGridPlayer);

                        // building to mouse
                        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _buildingOnMouse = _buildingToMove;
                        //_buildingOnMouse = Instantiate(_buildingToMove, new Vector3(mousePosition.x, mousePosition.y, -5), Quaternion.identity);

                        //Destroy(nearestTile.Building.gameObject);
                    }
                }
                #endregion
            }

            //combat
            if (_currentMode == Mode.Combat && nearestTileGridPlayer != null)
            {
                Debug.Log("combat");
                if (nearestTileGridPlayer.IsOccupied)
                {
                    Debug.Log("hit room " + nearestTileGridPlayer.Building.name);
                    nearestTileGridPlayer.BuildingTileSpriteRenderer.color = Color.magenta;
                    nearestTileGridPlayer.IsDestroyed = true;

                    // update hidden rooms
                    if (_playerTurn == Player.Player1)
                    {
                        ShowOnlyDestroyedBuildings(Player.Player2);
                    }
                    else
                    {
                        ShowOnlyDestroyedBuildings(Player.Player1);
                    }
                }
                else
                {
                    Debug.Log("no room on hit");
                }
            }
        }
    }

    #region Construction
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
        if (building.LeftTilesSR.Count > 0) 
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.LeftTilesSR.Count; i++)
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
        if (building.RightTilesSR.Count > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.RightTilesSR.Count; i++)
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
        if (building.BottomTilesSR.Count > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.BottomTilesSR.Count; i++)
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
        if (building.TopTilesSR.Count > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.TopTilesSR.Count; i++)
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
        tile.BuildingTileSpriteRenderer = building.CenterTileSR;

        // left
        if (building.LeftTilesSR.Count > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.LeftTilesSR.Count; i++)
            {
                tile.LeftTile.IsOccupied = true;
                tile.LeftTile.Building = building;
                tile.LeftTile.BuildingTileSpriteRenderer = building.LeftTilesSR[i];

                currentTile = tile.LeftTile;
                tiles.Add(currentTile);
            }
        }
        // right
        if (building.RightTilesSR.Count > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.RightTilesSR.Count; i++)
            {
                tile.RightTile.IsOccupied = true;
                tile.RightTile.Building = building;
                tile.RightTile.BuildingTileSpriteRenderer = building.RightTilesSR[i];

                currentTile = tile.RightTile;
                tiles.Add(currentTile);
            }
        }
        // top
        if (building.TopTilesSR.Count > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.TopTilesSR.Count; i++)
            {
                tile.TopTile.IsOccupied = true;
                tile.TopTile.Building = building;
                tile.TopTile.BuildingTileSpriteRenderer = building.TopTilesSR[i];

                currentTile = tile.TopTile; 
                tiles.Add(currentTile);
            }
        }
        // bottom
        if (building.BottomTilesSR.Count > 0)
        {
            Prototype_Tile currentTile = tile;
            for (int i = 0; i < building.BottomTilesSR.Count; i++)
            {
                tile.BottomTile.IsOccupied = true;
                tile.BottomTile.Building = building;
                tile.BottomTile.BuildingTileSpriteRenderer = building.BottomTilesSR[i];

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
            tile3.BuildingTileSpriteRenderer = null;
        }
    }

    private Prototype_Tile FindNearestTileInGrid(Player player)
    {
        Prototype_Tile nearestTile = null;
        float shortestDistance = float.MaxValue;

        if (player == Player.Player1)
        {
            foreach (Prototype_Tile tile in _tilesPlayer1)
            {
                float distance = Vector2.Distance(tile.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if (distance < shortestDistance && distance < 1)
                {
                    shortestDistance = distance;
                    nearestTile = tile;
                }
            }
        }
        else
        {
            foreach (Prototype_Tile tile in _tilesPlayer2)
            {
                float distance = Vector2.Distance(tile.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if (distance < shortestDistance && distance < 1)
                {
                    shortestDistance = distance;
                    nearestTile = tile;
                }
            }
        }

        return nearestTile;
    }
    #endregion

    #region Combat
    private void ShowOnlyDestroyedBuildings(Player playerShip)
    {
        List<Prototype_Tile> tiles = new List<Prototype_Tile>();

        if (playerShip == Player.Player1)
        {
            tiles = _tilesPlayer1;
        }
        else
        {
            tiles = _tilesPlayer2;
        }

        foreach (Prototype_Tile tile in tiles)
        {
            if (tile.IsOccupied)
            {
                if (!tile.IsDestroyed)
                {
                    tile.BuildingTileSpriteRenderer.enabled = false;
                }
                else
                {
                    tile.BuildingTileSpriteRenderer.enabled = true;
                }
            }
            
        }
    }

    private void ShowAllBuildings(Player playerShip)
    {
        List<Prototype_Tile> tiles = new List<Prototype_Tile>();

        if (playerShip == Player.Player1)
        {
            tiles = _tilesPlayer1;
        }
        else
        {
            tiles = _tilesPlayer2;
        }

        foreach (Prototype_Tile tile in tiles)
        {
            if (tile.IsOccupied)
            {
                tile.BuildingTileSpriteRenderer.enabled = true;
            }

        }
    }

    #endregion


    public void SwitchPlayer()
    {
        if (_playerTurn == Player.Player1)
        {
            _playerTurn = Player.Player2;
        }
        else
        {
            _playerTurn = Player.Player1;
        }

        SwitchCamera();

        // update ui
        Prototype_ManagerUI.instance.UpdateCurrentPlayerTxt(_playerTurn);
    }

    private void SwitchCamera()
    {
        if (_currentMode == Mode.Construction)
        {
            Prototype_CameraController.instance.SwitchPlayerShipCamera(_playerTurn);
        }
        else // combat -> vaisseau ennemi
        {
            if (_playerTurn == Player.Player1)
            {
                Prototype_CameraController.instance.SwitchPlayerShipCamera(Player.Player2);
                ShowOnlyDestroyedBuildings(Player.Player2);
                ShowAllBuildings(Player.Player1);
            }
            else
            {
                Prototype_CameraController.instance.SwitchPlayerShipCamera(Player.Player1);
                ShowOnlyDestroyedBuildings(Player.Player1);
                ShowAllBuildings(Player.Player2);
            }
        }
    }

    public void SwitchMode()
    {
        if (_currentMode == Mode.Construction)
        {
            _currentMode = Mode.Combat;
        }
        else
        {
            _currentMode = Mode.Construction;
        }

        SwitchCamera();

        // update ui
        Prototype_ManagerUI.instance.UpdateCurrentModeTxt(_currentMode);
    }

    public Mode GetCurrentMode()
    {
        return _currentMode;
    }
            
}
