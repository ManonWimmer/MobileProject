using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

public class GameManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static GameManager instance;

    [SerializeField] GameObject _gridPlayer1;
    [SerializeField] GameObject _gridPlayer2;
    [SerializeField] List<Room> _startRooms = new List<Room>();

    [SerializeField] float _constructionTimerSeconds = 120f;
    private float _constructionTimerElapsedSeconds = 0f;
    private float _constructionTimerRemainingSeconds;
    private Coroutine _constructionTimerCoroutine;

    private Dictionary<Tuple<int, int>, Tile> _dictTilesRowColumnPlayer1 = new Dictionary<Tuple<int, int>, Tile>();
    private Dictionary<Tuple<int, int>, Tile> _dictTilesRowColumnPlayer2 = new Dictionary<Tuple<int, int>, Tile>();

    private List<Tile> _tilesPlayer1 = new List<Tile>();
    private List<Tile> _tilesPlayer2 = new List<Tile>();

    private Room _roomToPlace;
    private Room _roomToMove;
    private Room _roomOnMouse;

    private Tile _targetOnTile;

    private Player _playerTurn;
    private Mode _currentMode = Mode.Construction;

    public Tile TargetOnTile { get => _targetOnTile; set => _targetOnTile = value; }
    public Player PlayerTurn { get => _playerTurn; set => _playerTurn = value; }

    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Start Construction
        InitGridDicts();
        RandomizeRoomsPlacement();

        // Update UI
        UIManager.instance.UpdateCurrentPlayerTxt(_playerTurn);
        UIManager.instance.UpdateCurrentModeTxt(_currentMode);

        // a changer plus tard, mettre après les choix de compétences
        if (_currentMode == Mode.Construction)
        {
            _constructionTimerCoroutine = StartCoroutine(StartConstructionTimer());
        }
    }

    private void InitGridDicts()
    {
        #region Player1Dict
        foreach (Tile tile in _gridPlayer1.GetComponentsInChildren<Tile>())
        {
            _tilesPlayer1.Add(tile);
            int row = tile.Row;
            int column = tile.Column;
            _dictTilesRowColumnPlayer1[new Tuple<int, int>(row, column)] = tile;
        }

        // search adjacent tiles for each tile
        foreach (Tile tile in _tilesPlayer1)
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

        if (_startRooms.Count > 0)
        {
            foreach (Room startBuilding in _startRooms)
            {
                bool buildingBuilt = false;
                Debug.Log(startBuilding.name);
                while (!buildingBuilt)
                {
                    Tile tempTile = _tilesPlayer1[Random.Range(0, _tilesPlayer1.Count - 1)];
                    if (CheckCanBuild(startBuilding, tempTile))
                    {
                        CreateNewBuilding(startBuilding, tempTile);
                        buildingBuilt = true;
                    }
                }
            }
        }
        #endregion

        #region Player2Dict
        foreach (Tile tile in _gridPlayer2.GetComponentsInChildren<Tile>())
        {
            _tilesPlayer2.Add(tile);
            int row = tile.Row;
            int column = tile.Column;
            _dictTilesRowColumnPlayer2[new Tuple<int, int>(row, column)] = tile;
        }

        // search adjacent tiles for each tile
        foreach (Tile tile in _tilesPlayer2)
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
        #endregion
    }

    private void Update()
    {
        if (UIManager.instance.ChangingPlayer)
        {
            return;
        }

        if (_roomOnMouse != null) // update room on mouse pos
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _roomOnMouse.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Tile nearestTileGridPlayer = null;

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
                CheckTileClickedInConstruction(nearestTileGridPlayer);
            }

            //combat
            if (_currentMode == Mode.Combat && nearestTileGridPlayer != null)
            {
                CheckTileClickedInCombat(nearestTileGridPlayer);
            }
        }
    }

    #region CheckClickOnTile
    private void CheckTileClickedInConstruction(Tile nearestTile)
    {
        if (!nearestTile.IsOccupied) // no building
        {
            if (_roomToPlace != null)
            {
                if (CheckCanBuild(_roomToPlace, nearestTile))
                {
                    CreateNewBuilding(_roomToPlace, nearestTile);
                }
            }
            else if (_roomToMove != null)
            {
                if (CheckCanBuild(_roomToMove, nearestTile))
                {
                    CreateNewBuilding(_roomToMove, nearestTile);
                }
            }
        }
        else // already a building
        {
            Debug.Log("occupied");
            if (_roomToMove == null)
            {
                Debug.Log("new building to move");
                // select move building
                _roomToMove = nearestTile.Room;
                nearestTile.IsOccupied = false;

                SetBuildingTilesNotOccupied(_roomToMove, nearestTile);

                // building to mouse
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _roomOnMouse = _roomToMove;
            }
        }
    }

    private void CheckTileClickedInCombat(Tile nearestTile)
    {
        Debug.Log("combat");
        TargetController.instance.ChangeTargetPosition(nearestTile.transform.position);
        _targetOnTile = nearestTile;

        if (_targetOnTile.IsDestroyed || _targetOnTile.IsMissed)
        {
            TargetController.instance.ChangeTargetColorToRed();
            UIManager.instance.ShowFicheRoom(_targetOnTile.Room.RoomData);
        }
        else
        {
            TargetController.instance.ChangeTargetColorToWhite();
            UIManager.instance.HideFicheRoom();
        }

        UIManager.instance.CheckAbilityButtonsColor();
    }

    public bool IsTargetOnTile()
    {
        return _targetOnTile;
    }
    #endregion

    #region Construction
    private void RandomizeRoomsPlacement()
    {
        if (_startRooms.Count > 0)
        {
            foreach (Room startRoom in _startRooms)
            {
                bool roomBuilt = false;
                Debug.Log(startRoom.name);
                while (!roomBuilt)
                {
                    Tile tempTile = _tilesPlayer2[Random.Range(0, _tilesPlayer2.Count - 1)];
                    if (CheckCanBuild(startRoom, tempTile))
                    {
                        CreateNewBuilding(startRoom, tempTile);
                        roomBuilt = true;
                    }
                }
            }
        }
    }

    public void TakeBuilding(Room building)
    {
        _roomToPlace = building;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _roomOnMouse = Instantiate(_roomToPlace, new Vector3(mousePosition.x, mousePosition.y, -5), Quaternion.identity);
    }

    private bool CheckCanBuild(Room building, Tile tile)
    {
        // center
        if (tile.IsOccupied) 
        {
            Debug.Log("can't build at center");
            return false;
        }

        #region Left, Right, Bottom Top
        // left
        if (building.LeftTilesSR.Count > 0) 
        {
            Tile currentTile = tile;
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
            Tile currentTile = tile;
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
            Tile currentTile = tile;
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
            Tile currentTile = tile;
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
        #endregion

        #region Diag Left & Right Botton - Left & Right Top
        // diag left bottom
        if (building.DiagBottomLeftTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagBottomLeftTilesSR.Count; i++)
            {
                if (tile.LeftTile.BottomTile != null)
                {
                    if (tile.LeftTile.BottomTile.IsOccupied)
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

                currentTile = tile.LeftTile.BottomTile;
            }
        }
        // diag right bottom
        if (building.DiagBottomRightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagBottomRightTilesSR.Count; i++)
            {
                if (tile.RightTile.BottomTile != null)
                {
                    if (tile.RightTile.BottomTile.IsOccupied)
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

                currentTile = tile.RightTile.BottomTile;
            }
        }
        // diag left top
        if (building.DiagTopLeftTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagTopLeftTilesSR.Count; i++)
            {
                if (tile.LeftTile.TopTile != null)
                {
                    if (tile.LeftTile.TopTile.IsOccupied)
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

                currentTile = tile.LeftTile.TopTile;
            }
        }
        // diag right top
        if (building.DiagTopRightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagTopRightTilesSR.Count; i++)
            {
                if (tile.RightTile.TopTile != null)
                {
                    if (tile.RightTile.TopTile.IsOccupied)
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

                currentTile = tile.RightTile.TopTile;
            }
        }
        #endregion
        Debug.Log("can build");
        return true;
    }

    private void CreateNewBuilding(Room building, Tile tile)
    {
        // place new building
        //Prototype_Building newBuilding = _buildingOnMouse;
        Debug.Log("instantiate new building");
        Room newBuilding = Instantiate(building, new Vector3(tile.transform.position.x, tile.transform.position.y, -5), Quaternion.identity);
        //newBuilding.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -5); // adjust to tile position

        tile.Room = newBuilding;
        tile.IsOccupied = true;

        SetBuildingTilesOccupied(newBuilding, tile);

        _roomToPlace = null;

        // no building on mouse
        Debug.Log("destroy on mouse");
        if (_roomOnMouse != null)
        {
            Destroy(_roomOnMouse.gameObject);
        }
    }

    private void SetBuildingTilesOccupied(Room building, Tile tile)
    {
        List<Tile> tiles = new List<Tile>();
        tiles.Add(tile); // center
        tile.RoomTileSpriteRenderer = building.CenterTileSR;

        #region Left, Right, Top & Bottom
        // left
        if (building.LeftTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.LeftTilesSR.Count; i++)
            {
                tile.LeftTile.IsOccupied = true;
                tile.LeftTile.Room = building;
                tile.LeftTile.RoomTileSpriteRenderer = building.LeftTilesSR[i];

                currentTile = tile.LeftTile;
                tiles.Add(currentTile);
            }
        }
        // right
        if (building.RightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.RightTilesSR.Count; i++)
            {
                tile.RightTile.IsOccupied = true;
                tile.RightTile.Room = building;
                tile.RightTile.RoomTileSpriteRenderer = building.RightTilesSR[i];

                currentTile = tile.RightTile;
                tiles.Add(currentTile);
            }
        }
        // top
        if (building.TopTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.TopTilesSR.Count; i++)
            {
                tile.TopTile.IsOccupied = true;
                tile.TopTile.Room = building;
                tile.TopTile.RoomTileSpriteRenderer = building.TopTilesSR[i];

                currentTile = tile.TopTile; 
                tiles.Add(currentTile);
            }
        }
        // bottom
        if (building.BottomTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.BottomTilesSR.Count; i++)
            {
                tile.BottomTile.IsOccupied = true;
                tile.BottomTile.Room = building;
                tile.BottomTile.RoomTileSpriteRenderer = building.BottomTilesSR[i];

                currentTile = tile.BottomTile;
                tiles.Add(currentTile);
            }
        }
        #endregion

        #region Diag Left & Right Bottom - Left & Right Top
        // diag left bottom
        if (building.DiagBottomLeftTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagBottomLeftTilesSR.Count; i++)
            {
                tile.LeftTile.BottomTile.IsOccupied = true;
                tile.LeftTile.BottomTile.Room = building;
                tile.LeftTile.BottomTile.RoomTileSpriteRenderer = building.DiagBottomLeftTilesSR[i];

                currentTile = tile.LeftTile.BottomTile;
                tiles.Add(currentTile);
            }
        }
        // diag right bottom
        if (building.DiagBottomRightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagBottomRightTilesSR.Count; i++)
            {
                tile.RightTile.BottomTile.IsOccupied = true;
                tile.RightTile.BottomTile.Room = building;
                tile.RightTile.BottomTile.RoomTileSpriteRenderer = building.DiagBottomRightTilesSR[i];

                currentTile = tile.RightTile.BottomTile;
                tiles.Add(currentTile);
            }
        }
        // diag left top
        if (building.DiagTopLeftTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagTopLeftTilesSR.Count; i++)
            {
                tile.LeftTile.TopTile.IsOccupied = true;
                tile.LeftTile.TopTile.Room = building;
                tile.LeftTile.TopTile.RoomTileSpriteRenderer = building.DiagTopLeftTilesSR[i];

                currentTile = tile.LeftTile.TopTile;
                tiles.Add(currentTile);
            }
        }
        // diag right top
        if (building.DiagTopRightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagTopRightTilesSR.Count; i++)
            {
                tile.RightTile.TopTile.IsOccupied = true;
                tile.RightTile.TopTile.Room = building;
                tile.RightTile.TopTile.RoomTileSpriteRenderer = building.DiagTopRightTilesSR[i];

                currentTile = tile.RightTile.TopTile;
                tiles.Add(currentTile);
            }
        }
        #endregion

        foreach (Tile buildingTile in tiles)
        {
            foreach (Tile buildingTile2 in tiles)
            {
                if (buildingTile != buildingTile2)
                {
                    buildingTile.RoomOnOtherTiles.Add(buildingTile2);
                }
            }
        }
    }

    private void SetBuildingTilesNotOccupied(Room building, Tile tile)
    {
        List<Tile> tiles = new List<Tile>();
        tiles.Add(tile); // center

        foreach(Tile tile2 in tile.RoomOnOtherTiles)
        {
            tiles.Add(tile2);
        }

        foreach (Tile tile3 in tiles)
        {
            tile3.IsOccupied = false;
            tile3.RoomOnOtherTiles.Clear();
            tile3.RoomTileSpriteRenderer = null;
        }
    }

    private Tile FindNearestTileInGrid(Player player)
    {
        Tile nearestTile = null;
        float shortestDistance = float.MaxValue;

        if (player == Player.Player1)
        {
            foreach (Tile tile in _tilesPlayer1)
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
            foreach (Tile tile in _tilesPlayer2)
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

    public void ValidateConstruction()
    {
        if (_constructionTimerCoroutine != null)
        {
            StopCoroutine(_constructionTimerCoroutine);
        }
        SwitchPlayer();

        if (_playerTurn == Player.Player1)
        {
            SwitchMode(); // -> Combat
            UIManager.instance.HideButtonValidateConstruction();
            UIManager.instance.ShowButtonsCombat();
        }
    }

    private IEnumerator StartConstructionTimer()
    {
        _constructionTimerElapsedSeconds = 0f;
        _constructionTimerRemainingSeconds = _constructionTimerSeconds;

        while (_constructionTimerElapsedSeconds < _constructionTimerSeconds)
        {
            _constructionTimerElapsedSeconds += Time.deltaTime;
            _constructionTimerRemainingSeconds = _constructionTimerSeconds - _constructionTimerElapsedSeconds;

            UIManager.instance.UpdateConstructionTimerTxt(_constructionTimerRemainingSeconds);
            yield return null;
        }

        Debug.Log("Timer terminé !");
        ValidateConstruction();
    }

    public void CheckIfStartConstructionTimer()
    {
        if (_currentMode == Mode.Construction)
        {
            _constructionTimerCoroutine = StartCoroutine(StartConstructionTimer());
        }
    }
    #endregion

    #region Combat
    public void ShowOnlyDestroyedAndReavealedRooms(Player playerShip)
    {
        List<Tile> tiles = new List<Tile>();

        if (playerShip == Player.Player1)
        {
            tiles = _tilesPlayer1;
        }
        else
        {
            tiles = _tilesPlayer2;
        }

        foreach (Tile tile in tiles)
        {
            if (tile.IsOccupied)
            {
                if (!tile.IsDestroyed && !tile.IsReavealed)
                {
                    tile.RoomTileSpriteRenderer.enabled = false;
                }
                else
                {
                    tile.RoomTileSpriteRenderer.enabled = true;
                }
            }
            
        }
    }

    public void ShowAllRooms(Player playerShip)
    {
        List<Tile> tiles = new List<Tile>();

        if (playerShip == Player.Player1)
        {
            tiles = _tilesPlayer1;
        }
        else
        {
            tiles = _tilesPlayer2;
        }

        foreach (Tile tile in tiles)
        {
            if (tile.IsOccupied)
            {
                tile.RoomTileSpriteRenderer.enabled = true;
            }

        }
    }

    #endregion

    #region Camera, Mode & Player
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
        UIManager.instance.UpdateCurrentPlayerTxt(_playerTurn);

        UIManager.instance.ShowChangerPlayerCanvas(_playerTurn);

        TargetController.instance.HideTarget();
        _targetOnTile = null;
    }

    private void SwitchCamera()
    {
        if (_currentMode == Mode.Construction)
        {
            CameraController.instance.SwitchPlayerShipCamera(_playerTurn);
        }
        else // combat -> vaisseau ennemi
        {
            if (_playerTurn == Player.Player1)
            {
                CameraController.instance.SwitchPlayerShipCamera(Player.Player2);
                ShowOnlyDestroyedAndReavealedRooms(Player.Player2);
                ShowAllRooms(Player.Player1);
            }
            else
            {
                CameraController.instance.SwitchPlayerShipCamera(Player.Player1);
                ShowOnlyDestroyedAndReavealedRooms(Player.Player1);
                ShowAllRooms(Player.Player2);
            }

            EnergySystem.instance.GetRoundEnergy(_playerTurn);
        }
    }

    public void SwitchMode()
    {
        if (_currentMode == Mode.Construction)
        {
            _currentMode = Mode.Combat;
            UIManager.instance.ShowEnergySlider();
        }
        else
        {
            _currentMode = Mode.Construction;
        }

        SwitchCamera();

        // update ui
        UIManager.instance.UpdateCurrentModeTxt(_currentMode);
    }

    public Mode GetCurrentMode()
    {
        return _currentMode;
    }
    #endregion

}
