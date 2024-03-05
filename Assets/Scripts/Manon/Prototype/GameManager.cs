using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Player
{
    Player1,
    Player2
}

public enum Mode
{
    Draft,
    Construction, 
    Combat
}

public class GameManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static GameManager instance;

    [Header("Abilities")]
    [SerializeField] List<scriptablePower> _abilitiesSO = new List<scriptablePower>();
    private List<GameObject> _abilityButtons = new List<GameObject>();

    [Header("Ships")]
    [SerializeField] List<Ship> _draftShipsPlayer1;
    [SerializeField] List<Ship> _draftShipsPlayer2;
    [SerializeField] List<Ship> _rewindShipsPlayer1;
    [SerializeField] List<Ship> _rewindShipsPlayer2;
    private List<Ship> _selectedDraftShips = new List<Ship>();

    private Ship _shipPlayer1;
    private Ship _shipPlayer2;

    private Ship _rewindShipPlayer1;
    private Ship _rewindShipPlayer2;

    [Header("Rooms")]
    [SerializeField] List<Room> _startVitalRooms = new List<Room>();
    [SerializeField] List<Room> _draftRooms = new List<Room>();
    private List<Room> _selectedDraftRooms = new List<Room>();

    private List<Room> _choosenDraftRoomsPlayer1 = new List<Room>();
    private List<Room> _choosenDraftRoomsPlayer2 = new List<Room>();

    private List<Room> _placedRoomsPlayer1 = new List<Room>();
    private List<Room> _placedRoomsPlayer2 = new List<Room>();

    [Header("Construction Timer")]
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
    private Mode _currentMode = Mode.Draft;

    private bool _gameStarted;

    private int _currentRound;

    // (pas possible de les modifier dans les scriptable objects parce que ça dépend des joueurs)

    private int _simpleRevealCooldownPlayer1;
    private int _simpleRevealCooldownPlayer2;

    private int _empCooldownPlayer1;
    private int _empCooldownPlayer2;

    private int _timeAcceleratorCooldownPlayer1;
    private int _timeAcceleratorCooldownPlayer2;

    private int _alternateShotCooldownPlayer1;
    private int _alternateShotCooldownPlayer2;

    private int _scannerCooldownPlayer1;
    private int _scannerCooldownPlayer2;

    private int _capacitorCooldownPlayer1;
    private int _capacitorCooldownPlayer2;

    private int _upgradeShotCooldownPlayer1;
    private int _upgradeShotCooldownPlayer2;

    private int _probeCooldownPlayer1;
    private int _probeCooldownPlayer2;

    public Tile TargetOnTile { get => _targetOnTile; set => _targetOnTile = value; }
    public Player PlayerTurn { get => _playerTurn; set => _playerTurn = value; }

    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _abilityButtons = AbilityButtonsManager.instance.GetAbilityButtonsList();
        StartDraftShips();
    }

    private void Update()
    {
        if (UIManager.instance.ChangingPlayer || !_gameStarted)
        {
            _roomOnMouse = null;
            return;
        }

        if (_roomOnMouse != null) // update room on mouse pos
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _roomOnMouse.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
        }

        if (_currentMode == Mode.Construction)
        {
            ClearPlacedRoomsLists();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("click");
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

    private void StartGame()
    {  
        // Start Construction
        InitGridDicts();
        RandomizeRoomsPlacement();

        // Update UI
        UIManager.instance.UpdateCurrentPlayerTxt(_playerTurn);
        UIManager.instance.UpdateCurrentModeTxt(_currentMode);

        SwitchMode();

        _gameStarted = true;
        UIManager.instance.ShowOrUpdateActionPoints();
        InitAbilitesSOButtons();
    }

    #region CheckClickOnTile
    private void CheckTileClickedInConstruction(Tile nearestTile)
    {
        if (!nearestTile.IsOccupied) // no building
        {
            UIManager.instance.HideFicheRoom();
            if (_roomToPlace != null)
            {
                if (CheckCanBuild(_roomToPlace, nearestTile))
                {
                    CreateNewBuilding(_roomToPlace, nearestTile, _playerTurn);
                }
            }
            else if (_roomToMove != null)
            {
                if (CheckCanBuild(_roomToMove, nearestTile))
                {
                    CreateNewBuilding(_roomToMove, nearestTile, _playerTurn);
                }
            }
            else
            {
                // a voir lequel des deux on mets hide fiche (quand on relache la room ou quand on clique sur une case vide)
                //UIManager.instance.HideFicheRoom();
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

                UIManager.instance.ShowFicheRoom(_roomOnMouse.RoomData);
            }
        }
    }

    public void CheckTileClickedInCombat(Tile nearestTile)
    {
        Debug.Log("check tile combat");
        TargetController.instance.ChangeTargetPosition(nearestTile.transform.position);
        _targetOnTile = nearestTile;


        if (_targetOnTile.IsDestroyed || _targetOnTile.IsMissed)
        {
            TargetController.instance.ChangeTargetColorToRed();
            if (_targetOnTile.Room != null)
            {
                UIManager.instance.ShowFicheRoom(_targetOnTile.Room.RoomData);
            } 
        }
        else if (_targetOnTile.IsReavealed)
        {
            UIManager.instance.ShowFicheRoom(_targetOnTile.Room.RoomData);
        }
        else
        {
            TargetController.instance.ChangeTargetColorToWhite();
            UIManager.instance.HideFicheRoom();
        }

        AbilityButtonsManager.instance.ChangeSelectedTilesOnTargetPos();
        UIManager.instance.CheckAbilityButtonsColor();
    }

    public bool IsTargetOnTile()
    {
        return _targetOnTile;
    }
    #endregion

    #region Construction
    private void InitGridDicts()
    {
        #region Player1Dict
        foreach (Tile tile in _shipPlayer1.gameObject.GetComponentsInChildren<Tile>())
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

            #region Top, Bottom, Left & Right
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
            #endregion

            #region Diag Top Left & Right, Bottom Left & Right
            // diag top left
            if (_dictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row - 1, column - 1)))
            {
                tile.DiagTopLeftTile = _dictTilesRowColumnPlayer1[new Tuple<int, int>(row - 1, column - 1)];
            }

            // diag top right
            if (_dictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row - 1, column + 1)))
            {
                tile.DiagTopRightTile = _dictTilesRowColumnPlayer1[new Tuple<int, int>(row - 1, column + 1)];
            }

            // diag bottom left
            if (_dictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row + 1, column - 1)))
            {
                tile.DiagBottomLeftTile = _dictTilesRowColumnPlayer1[new Tuple<int, int>(row + 1, column - 1)];
            }

            // diag bottom right
            if (_dictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row + 1, column + 1)))
            {
                tile.DiagBottomRightTile = _dictTilesRowColumnPlayer1[new Tuple<int, int>(row + 1, column + 1)];
            }
            #endregion
        }
        #endregion

        #region Player2Dict
        foreach (Tile tile in _shipPlayer2.GetComponentsInChildren<Tile>())
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

            #region Top, Bottom, Left & Right
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
            #endregion

            #region Diag Top Left & Right, Bottom Left & Right
            // diag top left
            if (_dictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row - 1, column - 1)))
            {
                tile.DiagTopLeftTile = _dictTilesRowColumnPlayer2[new Tuple<int, int>(row - 1, column - 1)];
            }

            // diag top right
            if (_dictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row - 1, column + 1)))
            {
                tile.DiagTopRightTile = _dictTilesRowColumnPlayer2[new Tuple<int, int>(row - 1, column + 1)];
            }

            // diag bottom left
            if (_dictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row + 1, column - 1)))
            {
                tile.DiagBottomLeftTile = _dictTilesRowColumnPlayer2[new Tuple<int, int>(row + 1, column - 1)];
            }

            // diag bottom right
            if (_dictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row + 1, column + 1)))
            {
                tile.DiagBottomRightTile = _dictTilesRowColumnPlayer2[new Tuple<int, int>(row + 1, column + 1)];
            }
            #endregion
        }
        #endregion
    }

    public void SelectDraftShip(Ship ship)
    {
        Debug.Log("select draft ship " + ship.ShipData.name);
        if (_playerTurn == Player.Player1)
        {
            // Player Ship
            foreach(Ship shipP1 in _draftShipsPlayer1)
            {
                Debug.Log(shipP1.name);
                if (shipP1.ShipData == ship.ShipData)
                {
                    Debug.Log("active " + shipP1.name);
                    shipP1.gameObject.SetActive(true);
                    _shipPlayer1 = shipP1;
                }
                else
                {
                    Debug.Log("inactive " + shipP1.name);
                    shipP1.gameObject.SetActive(false);
                } 
            }   

            // Player Ship Rewind
            foreach (Ship shipP1 in _rewindShipsPlayer1)
            {
                Debug.Log(shipP1.name);
                if (shipP1.ShipData == ship.ShipData)
                {
                    Debug.Log("active rewind " + shipP1.name);
                    shipP1.gameObject.SetActive(true);
                    _shipPlayer1 = shipP1;
                }
                else
                {
                    Debug.Log("inactive rewind" + shipP1.name);
                    shipP1.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // Player Ship
            foreach (Ship shipP2 in _draftShipsPlayer2)
            {
                if (shipP2.ShipData == ship.ShipData)
                {
                    Debug.Log("active " + shipP2.name);
                    shipP2.gameObject.SetActive(true);
                    _shipPlayer2 = shipP2;
                }
                else
                {
                    Debug.Log("inactive " + shipP2.name);
                    shipP2.gameObject.SetActive(false);
                }
            }

            // Player Ship Rewind
            foreach (Ship shipP1 in _rewindShipsPlayer2)
            {
                Debug.Log(shipP1.name);
                if (shipP1.ShipData == ship.ShipData)
                {
                    Debug.Log("active rewind " + shipP1.name);
                    shipP1.gameObject.SetActive(true);
                    _shipPlayer1 = shipP1;
                }
                else
                {
                    Debug.Log("inactive rewind " + shipP1.name);
                    shipP1.gameObject.SetActive(false);
                }
            }
        }

        SwitchPlayer();
    }

    private void StartDraftShips()
    {
        DraftManagerUI.instance.ShowDraftUI();
        DraftManager.instance.StartDraftShips();
        DraftManagerUI.instance.UpdatePlayerChoosing();
        _selectedDraftShips.Clear();

        int i = 0;

        while (i < 3)
        {
            int randomIndex = Random.Range(0, _draftShipsPlayer1.Count);
            if (!_selectedDraftShips.Contains(_draftShipsPlayer1[randomIndex]))
            {
                _selectedDraftShips.Add(_draftShipsPlayer1[randomIndex]);
                DraftManagerUI.instance.InitDraftShip(_selectedDraftShips.Count - 1, _draftShipsPlayer1[randomIndex]);
                i += 1;
            }
        }
    }

    public Ship GetPlayerShip()
    {
        if (_playerTurn == Player.Player1)
        {
            return _shipPlayer1;
        }
        else
        {
            return _shipPlayer2;
        }
    }

    public Room GetChoosenDraftRoom(int index)
    {
        if (_playerTurn == Player.Player1)
        {
            return _choosenDraftRoomsPlayer1[index];
        }
        else
        {
            return _choosenDraftRoomsPlayer2[index];
        }
    }

    private void StartDraftRooms1()
    {
        DraftManagerUI.instance.ShowDraftUI();
        DraftManager.instance.StartDraftRooms(1);

        _selectedDraftRooms.Clear();
        int i = 0;

        while (i < 3)
        {
            int randomIndex = Random.Range(0, _draftRooms.Count);
            if (!_selectedDraftRooms.Contains(_draftRooms[randomIndex]))
            {
                _selectedDraftRooms.Add(_draftRooms[randomIndex]);
                DraftManagerUI.instance.InitDraftRoom(_selectedDraftRooms.Count - 1, _draftRooms[randomIndex]);
                i += 1;
            }
        }
    }

    public void SelectDraftRoom(Room room)
    {
        if (_playerTurn == Player.Player1)
        {
            _choosenDraftRoomsPlayer1.Add(room);
        }
        else
        {
            _choosenDraftRoomsPlayer2.Add(room);
        }

        SwitchPlayer();
        
    }

    private void StartDraftRooms2()
    {
        DraftManagerUI.instance.ShowDraftUI();
        DraftManager.instance.StartDraftRooms(2);

        int i = 0;

        while (i < 3)
        {
            int randomIndex = Random.Range(0, _draftRooms.Count);
            if (!_selectedDraftRooms.Contains(_draftRooms[randomIndex]))
            {
                _selectedDraftRooms.Add(_draftRooms[randomIndex]);
                DraftManagerUI.instance.InitDraftRoom(_selectedDraftRooms.Count - 4, _draftRooms[randomIndex]);
                i += 1;
            }
        }
    }

    private void RandomizeRoomsPlacement()
    {
        RandomizeRoomsPlayer1();
        RandomizeRoomsPlayer2();
    }

    public void RandomizeRooms()
    {
        Debug.Log("randomize rooms");
        if (_playerTurn == Player.Player1)
        {
            RandomizeRoomsPlayer1();
        }
        else
        {
            RandomizeRoomsPlayer2();
        }
    }

    private void RandomizeRoomsPlayer1()
    {
        Debug.Log("randomize rooms player 1");
        List<Room> player1Rooms = new List<Room>();
        player1Rooms.AddRange(_startVitalRooms);
        player1Rooms.AddRange(_choosenDraftRoomsPlayer1);

        // Reset if already some rooms
        _placedRoomsPlayer1.Clear();

        Debug.Log("reset rooms");
        foreach(Tile tile in _tilesPlayer1)
        {
            if (tile.Room != null)
            {
                Debug.Log("la");
                tile.IsOccupied = false;
                tile.RoomOnOtherTiles.Clear();
                tile.RoomTileSpriteRenderer = null;
                Destroy(tile.Room.gameObject);
                tile.Room = null;
            }
        }

        Debug.Log("add rooms");
        if (player1Rooms.Count > 0)
        {
            foreach (Room player1Room in player1Rooms)
            {
                bool roomBuilt = false;
                Debug.Log(player1Room.name);
                while (!roomBuilt)
                {
                    Tile tempTile = _tilesPlayer1[Random.Range(0, _tilesPlayer1.Count - 1)];
                    if (CheckCanBuild(player1Room, tempTile))
                    {
                        CreateNewBuilding(player1Room, tempTile, Player.Player1);
                        roomBuilt = true;
                    }
                }
            }
        }
    }

    private void RandomizeRoomsPlayer2()
    {
        List<Room> player2Rooms = new List<Room>();
        player2Rooms.AddRange(_startVitalRooms);
        player2Rooms.AddRange(_choosenDraftRoomsPlayer2);

        // Reset if already some rooms
        _placedRoomsPlayer2.Clear();

        Debug.Log("reset rooms");
        foreach (Tile tile in _tilesPlayer2)
        {
            if (tile.Room != null)
            {
                Debug.Log("la");
                Destroy(tile.Room.gameObject);
                tile.IsOccupied = false;
                tile.Room = null;
            }
        }

        Debug.Log("add rooms");
        if (player2Rooms.Count > 0)
        {
            foreach (Room player2Room in player2Rooms)
            {
                bool roomBuilt = false;
                Debug.Log(player2Room.name);
                while (!roomBuilt)
                {
                    Tile tempTile = _tilesPlayer2[Random.Range(0, _tilesPlayer2.Count - 1)];
                    if (CheckCanBuild(player2Room, tempTile))
                    {
                        CreateNewBuilding(player2Room, tempTile, Player.Player2);
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
                if (currentTile.LeftTile != null)
                {
                    if (currentTile.LeftTile.IsOccupied)
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

                currentTile = currentTile.LeftTile;
            }
        }
        // right
        if (building.RightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.RightTilesSR.Count; i++)
            {
                if (currentTile.RightTile != null)
                {
                    if (currentTile.RightTile.IsOccupied)
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

                currentTile = currentTile.RightTile;
            }
        }
        // bottom
        if (building.BottomTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.BottomTilesSR.Count; i++)
            {
                if (currentTile.BottomTile != null)
                {
                    if (currentTile.BottomTile.IsOccupied)
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

                currentTile = currentTile.BottomTile;
            }
        }
        // top
        if (building.TopTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.TopTilesSR.Count; i++)
            {
                if (currentTile.TopTile != null)
                {
                    if (currentTile.TopTile.IsOccupied)
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

                currentTile = currentTile.TopTile;
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
                if (currentTile.DiagBottomLeftTile != null)
                {
                    if (currentTile.DiagBottomLeftTile.IsOccupied)
                    {
                        Debug.Log("diag bottom left tile occupied " + i);
                        return false;
                    }

                }
                else
                {
                    Debug.Log("no tile at diag bottomleft " + i);
                    return false;
                }

                currentTile = currentTile.DiagBottomLeftTile;
            }
        }
        // diag right bottom
        if (building.DiagBottomRightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagBottomRightTilesSR.Count; i++)
            {
                if (currentTile.DiagBottomRightTile != null)
                {
                    if (currentTile.DiagBottomRightTile.IsOccupied)
                    {
                        Debug.Log("diag bottom right tile occupied " + i);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("no tile at diag bottom right " + i);
                    return false;
                }

                currentTile = currentTile.DiagBottomRightTile;
            }
        }
        // diag left top
        if (building.DiagTopLeftTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagTopLeftTilesSR.Count; i++)
            {
                if (currentTile.DiagTopLeftTile != null)
                {
                    if (currentTile.DiagTopLeftTile.IsOccupied)
                    {
                        Debug.Log("diag top left tile occupied " + i);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("no tile at diag top left " + i);
                    return false;
                }

                currentTile = currentTile.DiagTopLeftTile;
            }
        }
        // diag right top
        if (building.DiagTopRightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagTopRightTilesSR.Count; i++)
            {
                if (currentTile.DiagTopRightTile != null)
                {
                    if (currentTile.DiagTopRightTile.IsOccupied)
                    {
                        Debug.Log("diag top right tile occupied " + i);
                        return false;
                    }
                }
                else
                {
                    Debug.Log("no tile at diag top right " + i);
                    return false;
                }

                currentTile = currentTile.DiagTopRightTile;
            }
        }
        #endregion
        Debug.Log("can build");
        return true;
    }

    private void CreateNewBuilding(Room building, Tile tile, Player player)
    {
        // place new building
        //Prototype_Building newBuilding = _buildingOnMouse;
        Debug.Log("instantiate new building");
        Room newBuilding = Instantiate(building, new Vector3(tile.transform.position.x, tile.transform.position.y, -0.5f), Quaternion.identity);
        //newBuilding.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -5); // adjust to tile position

        if (player == Player.Player1)
        {
            _placedRoomsPlayer1.Add(newBuilding);
        }
        else
        {
            _placedRoomsPlayer2.Add(newBuilding);

        }

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

    private void ClearPlacedRoomsLists()
    {
        //Debug.Log("clear room lists");
        _placedRoomsPlayer1.RemoveAll(s => s == null);
        _placedRoomsPlayer2.RemoveAll(s => s == null);
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
                currentTile.LeftTile.IsOccupied = true;
                currentTile.LeftTile.Room = building;
                currentTile.LeftTile.RoomTileSpriteRenderer = building.LeftTilesSR[i];

                currentTile = currentTile.LeftTile;
                tiles.Add(currentTile);
            }
        }
        // right
        if (building.RightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.RightTilesSR.Count; i++)
            {
                currentTile.RightTile.IsOccupied = true;
                currentTile.RightTile.Room = building;
                currentTile.RightTile.RoomTileSpriteRenderer = building.RightTilesSR[i];

                currentTile = currentTile.RightTile;
                tiles.Add(currentTile);
            }
        }
        // top
        if (building.TopTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.TopTilesSR.Count; i++)
            {
                currentTile.TopTile.IsOccupied = true;
                currentTile.TopTile.Room = building;
                currentTile.TopTile.RoomTileSpriteRenderer = building.TopTilesSR[i];

                currentTile = currentTile.TopTile; 
                tiles.Add(currentTile);
            }
        }
        // bottom
        if (building.BottomTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.BottomTilesSR.Count; i++)
            {
                currentTile.BottomTile.IsOccupied = true;
                currentTile.BottomTile.Room = building;
                currentTile.BottomTile.RoomTileSpriteRenderer = building.BottomTilesSR[i];

                currentTile = currentTile.BottomTile;
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
                currentTile.DiagBottomLeftTile.IsOccupied = true;
                currentTile.DiagBottomLeftTile.Room = building;
                currentTile.DiagBottomLeftTile.RoomTileSpriteRenderer = building.DiagBottomLeftTilesSR[i];

                currentTile = currentTile.DiagBottomLeftTile;
                tiles.Add(currentTile);
            }
        }
        // diag right bottom
        if (building.DiagBottomRightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagBottomRightTilesSR.Count; i++)
            {
                currentTile.DiagBottomRightTile.IsOccupied = true;
                currentTile.DiagBottomRightTile.Room = building;
                currentTile.DiagBottomRightTile.RoomTileSpriteRenderer = building.DiagBottomRightTilesSR[i];

                currentTile = currentTile.DiagBottomRightTile;
                tiles.Add(currentTile);
            }
        }
        // diag left top
        if (building.DiagTopLeftTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagTopLeftTilesSR.Count; i++)
            {
                currentTile.DiagTopLeftTile.IsOccupied = true;
                currentTile.DiagTopLeftTile.Room = building;
                currentTile.DiagTopLeftTile.RoomTileSpriteRenderer = building.DiagTopLeftTilesSR[i];

                currentTile = currentTile.DiagTopLeftTile;
                tiles.Add(currentTile);
            }
        }
        // diag right top
        if (building.DiagTopRightTilesSR.Count > 0)
        {
            Tile currentTile = tile;
            for (int i = 0; i < building.DiagTopRightTilesSR.Count; i++)
            {
                currentTile.DiagTopRightTile.IsOccupied = true;
                currentTile.DiagTopRightTile.Room = building;
                currentTile.DiagTopRightTile.RoomTileSpriteRenderer = building.DiagTopRightTilesSR[i];

                currentTile = currentTile.DiagTopRightTile;
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
            CheckPlayerAbilityButtonsEnabled();
        }
    }

    private IEnumerator StartConstructionTimer()
    {
        _constructionTimerElapsedSeconds = 0f;
        _constructionTimerRemainingSeconds = _constructionTimerSeconds;

        while (_constructionTimerElapsedSeconds < _constructionTimerSeconds && _currentMode == Mode.Construction)
        {
            _constructionTimerElapsedSeconds += Time.deltaTime;
            _constructionTimerRemainingSeconds = _constructionTimerSeconds - _constructionTimerElapsedSeconds;

            UIManager.instance.UpdateConstructionTimerTxt(_constructionTimerRemainingSeconds);
            yield return null;
        }

        if (_currentMode == Mode.Construction)
        {
            Debug.Log("Timer terminé !");
            ValidateConstruction();
        }
    }

    public void CheckIfStartConstructionTimer()
    {
        if (_currentMode == Mode.Construction)
        {
            Debug.Log("start construction timer");
            _constructionTimerCoroutine = StartCoroutine(StartConstructionTimer());
        }
    }
    #endregion

    #region Combat
    public void CheckVictory()
    {
        Debug.Log("check victory");
        bool player1Dead = true;
        foreach (Room room in _placedRoomsPlayer1)
        {
            Debug.Log(room.name);
            if (room.RoomData.IsVital && !room.IsRoomDestroyed)
            {
                player1Dead = false;
            }
        }

        bool player2Dead = true;
        foreach (Room room in _placedRoomsPlayer2)
        {
            Debug.Log(room.name);
            if (room.RoomData.IsVital && !room.IsRoomDestroyed)
            {
                player2Dead = false;
            }
        }

        if (player1Dead)
        {
            UIManager.instance.ShowVictoryCanvas(Player.Player2);
        }
        else if (player2Dead)
        {
            UIManager.instance.ShowVictoryCanvas(Player.Player1);
        }
    }

    private void InitAbilitesSOButtons()
    {
        Debug.Log("init abilities so buttons");
        foreach (scriptablePower ability in _abilitiesSO)
        {
            switch (ability.AbilityName)
            {
                case ("Simple Hit"):
                    for (int i = 0; i < _abilityButtons.Count; i++)
                    {
                        if (_abilityButtons[i].name == "SimpleHit")
                        {
                            ability.AbilityButton = _abilityButtons[i];
                            Debug.Log("found simple hit button");
                            break;
                        }
                    }
                    break;
                case ("EMP"):
                    _empCooldownPlayer1 = 0;
                    _empCooldownPlayer2 = 0;

                    for (int i = 0; i < _abilityButtons.Count; i++)
                    {
                        if (_abilityButtons[i].name == "EMP")
                        {
                            ability.AbilityButton = _abilityButtons[i];
                            Debug.Log("found emp button");
                            break;
                        }
                    }
                    break;
                case ("Time Accelerator"):
                    _timeAcceleratorCooldownPlayer1 = 0;
                    _timeAcceleratorCooldownPlayer2 = 0;

                    for (int i = 0; i < _abilityButtons.Count; i++)
                    {
                        if (_abilityButtons[i].name == "TimeAccelerator")
                        {
                            ability.AbilityButton = _abilityButtons[i];
                            Debug.Log("found time accelerator button");
                            break;
                        }
                    }
                    break;
                case ("Simple Reveal"):
                    _simpleRevealCooldownPlayer1 = 0;
                    _simpleRevealCooldownPlayer2 = 0;

                    for (int i = 0; i < _abilityButtons.Count; i++)
                    {
                        if (_abilityButtons[i].name == "SimpleReveal")
                        {
                            ability.AbilityButton = _abilityButtons[i];
                            Debug.Log("found simple reveal button");
                            break;
                        }
                    }
                    break;
                case ("Alternate Shot"):
                    _alternateShotCooldownPlayer1 = 0;
                    _alternateShotCooldownPlayer2 = 0;

                    for (int i = 0; i < _abilityButtons.Count; i++)
                    {
                        if (_abilityButtons[i].name == "AlternateShot")
                        {
                            ability.AbilityButton = _abilityButtons[i];
                            Debug.Log("found alternate shot button");
                            break;
                        }
                    }
                    break;
                case ("Scanner"):
                    _scannerCooldownPlayer1 = 0;
                    _scannerCooldownPlayer2 = 0;

                    for (int i = 0; i < _abilityButtons.Count; i++)
                    {
                        if (_abilityButtons[i].name == "Scanner")
                        {
                            ability.AbilityButton = _abilityButtons[i];
                            Debug.Log("found scanner button");
                            break;
                        }
                    }
                    break;
                case ("Capacitor"):
                    _capacitorCooldownPlayer1 = 0;
                    _capacitorCooldownPlayer2 = 0;

                    for (int i = 0; i < _abilityButtons.Count; i++)
                    {
                        if (_abilityButtons[i].name == "Capacitor")
                        {
                            ability.AbilityButton = _abilityButtons[i];
                            Debug.Log("found capacitor button");
                            break;
                        }
                    }
                    break;
                case ("Upgrade Shot"):
                    _upgradeShotCooldownPlayer1 = 0;
                    _upgradeShotCooldownPlayer2 = 0;

                    for(int i = 0; i < _abilityButtons.Count; i++)
                    {
                        if (_abilityButtons[i].name == "UpgradeShot")
                        {
                            ability.AbilityButton = _abilityButtons[i];
                            Debug.Log("found upgrade shot button");
                            break;
                        }
                    }
                    break;
                case ("Probe"):
                    _probeCooldownPlayer1 = 0;
                    _probeCooldownPlayer2 = 0;

                    for (int i = 0; i < _abilityButtons.Count; i++)
                    {
                        if (_abilityButtons[i].name == "Probe")
                        {
                            ability.AbilityButton = _abilityButtons[i];
                            Debug.Log("found probe button");
                            break;
                        }
                    }
                    break;
            }
        }
    }

    private void CheckPlayerAbilityButtonsEnabled()
    {
        Debug.Log("check player ability buttons enabled");
        List<scriptablePower> inRoomsAbilities = new List<scriptablePower>();

        if (_playerTurn == Player.Player1)
        {
            foreach (Room room in _placedRoomsPlayer1)
            {
                Debug.Log(room.name);
                if (room.IsRoomDestroyed)
                {
                    Debug.Log("room destroyed " + room.name);
                    if (room.RoomData.RoomAbility != null )
                    {
                        if (room.RoomData.name != "SimpleHit")
                        {
                            Debug.Log("room inactive" + room.RoomData.RoomAbility.name);
                            room.RoomData.RoomAbility.AbilityButton.GetComponentInChildren<AbilityButton>().SetOffline();
                        }
                    }
                }
                else
                {
                    if (room.RoomData.RoomAbility != null)
                    {
                        Debug.Log("room active" + room.RoomData.RoomAbility.name);
                        room.RoomData.RoomAbility.AbilityButton.GetComponentInChildren<AbilityButton>().SetOnline();
                    }
                }
              
                foreach (scriptablePower ability in _abilitiesSO)
                {
                    if (room.RoomData.RoomAbility != null)
                    {
                        if (room.RoomData.RoomAbility == ability)
                        {
                            inRoomsAbilities.Add(ability);
                        }
                    }
                }
            }
        }
        else
        {
            foreach (Room room in _placedRoomsPlayer2)
            {
                if (room.IsRoomDestroyed)
                {
                    Debug.Log("room destroyed " + room.name);
                    if (room.RoomData.RoomAbility != null)
                    {
                        if (room.RoomData.RoomAbility.name != "SimpleHit")
                        {
                            Debug.Log("room inactive");
                            room.RoomData.RoomAbility.AbilityButton.GetComponentInChildren<AbilityButton>().SetOffline();
                        }
                    }
                }
                else
                {
                    if (room.RoomData.RoomAbility != null)
                    {
                        room.RoomData.RoomAbility.AbilityButton.GetComponentInChildren<AbilityButton>().SetOnline();
                    }
                }


                foreach (scriptablePower ability in _abilitiesSO)
                {
                    if (room.RoomData.RoomAbility != null)
                    {
                        if (room.RoomData.RoomAbility == ability)
                        {
                            inRoomsAbilities.Add(ability);
                        }
                    }
                }
            } 
        }

        foreach(scriptablePower ability in _abilitiesSO)
        {
            if (inRoomsAbilities.Contains(ability))
            {
                ability.AbilityButton.gameObject.SetActive(true);
            }
            else
            {
                ability.AbilityButton.gameObject.SetActive(false);
            }
        }

    }

    public void ShowOnlyDestroyedAndReavealedRooms(Player playerShip)
    {
        Debug.Log("show only destroyed and revealed rooms");
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

    public void CheckIfTargetRoomIsCompletelyDestroyed()
    {
        if (_targetOnTile.Room != null)
        {
            Debug.Log("check if target room is completely destoyed");
            bool roomCompletelyDestroyed = true;

            foreach (Tile tile in _targetOnTile.RoomOnOtherTiles)
            {
                if (!tile.IsDestroyed)
                {
                    Debug.Log(tile.name + "not destroyed");
                    roomCompletelyDestroyed = false;
                }
            }

            if (roomCompletelyDestroyed)
            {
                Debug.Log("room completely destroyed");
                _targetOnTile.RoomTileSpriteRenderer.color = Color.red;

                foreach (Tile tile in _targetOnTile.RoomOnOtherTiles)
                {
                    tile.RoomTileSpriteRenderer.color = Color.red;
                    tile.Room.IsRoomDestroyed = true;
                }
            }

        } 
        CheckVictory();
    }

    public void CheckIfTileRoomIsCompletelyDestroyed(Tile targetTile)
    {
        if (targetTile.Room != null)
        {
            Debug.Log("check if tile room is completely destoyed");
            bool roomCompletelyDestroyed = true;

            foreach (Tile tile in targetTile.RoomOnOtherTiles)
            {
                if (!tile.IsDestroyed)
                {
                    Debug.Log(tile.name + "not destroyed");
                    roomCompletelyDestroyed = false;
                }
            }

            if (roomCompletelyDestroyed)
            {
                Debug.Log("room completely destroyed");
                targetTile.RoomTileSpriteRenderer.color = Color.red;

                foreach (Tile tile in targetTile.RoomOnOtherTiles)
                {
                    tile.RoomTileSpriteRenderer.color = Color.red;
                    tile.Room.IsRoomDestroyed = true;
                }
            }

        }
        CheckVictory();
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

        if (_playerTurn == Player.Player1 && _currentMode == Mode.Draft)
        {
            switch (DraftManager.instance.CurrentDraft)
            {
                case (0):
                    StartDraftRooms1();
                    break;
                case (1):
                    StartDraftRooms2();
                    break;
                case (2): // start game
                    StartGame();
                    DraftManagerUI.instance.HideDraftUI();
                    UIManager.instance.ShowGameCanvas();
                    break;
            }
        }

        SwitchCamera();

        // update ui
        UIManager.instance.UpdateCurrentPlayerTxt(_playerTurn);


        UIManager.instance.ShowChangerPlayerCanvas(_playerTurn);

        //TargetController.instance.HideTarget();
        _targetOnTile = null;

        if (_currentMode == Mode.Combat)
        {
            CheckPlayerAbilityButtonsEnabled();
            AbilityButtonsManager.instance.ResetRoundAbilityButtons();
            //SetRoundTargetPos();
            UIManager.instance.CheckAlternateShotDirectionImgRotation();
            UIManager.instance.CheckSimpleHitX2Img();
            AbilityButtonsManager.instance.ResetCurrentProbeCount();
            AbilityButtonsManager.instance.Rewind();
        }

        if (_currentMode == Mode.Draft)
        {
            if (DraftManager.instance.ShipDraft)
            {
                DraftManager.instance.SelectShip(0);
            }
            else
            {
                DraftManager.instance.SelectRoom(0);
            }
            DraftManagerUI.instance.UpdatePlayerChoosing();
        }
    }

    public void SetRoundTargetPos()
    {
        Debug.Log("set round target pos");
        if (_playerTurn == Player.Player1)
        {
            CheckTileClickedInCombat(_shipPlayer2.StartRoundTargetTile);
        }
        else
        {
            CheckTileClickedInCombat(_shipPlayer1.StartRoundTargetTile);
        }

    }

    private void SwitchCamera()
    {
        if (_currentMode == Mode.Construction)
        {
            CameraController.instance.SwitchPlayerShipCameraDirectly(_playerTurn);
        }
        else if (_currentMode == Mode.Combat)// combat -> vaisseau ennemi
        {
            if (_playerTurn == Player.Player1)
            {
                CameraController.instance.SwitchPlayerShipCameraDirectly(Player.Player2);
                ShowOnlyDestroyedAndReavealedRooms(Player.Player2);
                ShowAllRooms(Player.Player1);

                // new round
                _currentRound++;
                Debug.Log("+1 round : " + _currentRound);
                ActionPointsManager.instance.InitRoundActionPoints(_currentRound);
            }
            else
            {
                CameraController.instance.SwitchPlayerShipCameraDirectly(Player.Player1);
                ShowOnlyDestroyedAndReavealedRooms(Player.Player1);
                UIManager.instance.ShowOrUpdateActionPoints();
                ShowAllRooms(Player.Player2);
            }

            if (_currentRound >= 1)
            {
                SetRoundCooldowns(_playerTurn);
            }
        }
        // Draft on s'en fout
    }

    public void SwitchMode()
    {
        if (_currentMode == Mode.Construction)
        {
            _currentMode = Mode.Combat;
            _currentRound = 0;
            UIManager.instance.ShowOrUpdateActionPoints();
            UIManager.instance.HideRandomizeRoomsButton();
            UIManager.instance.ShowShitchShipButton();
            //SetRoundTargetPos();
            UIManager.instance.CheckSimpleHitX2Img();
            AbilityButtonsManager.instance.ResetCurrentProbeCount();
            UIManager.instance.StartGameCanvas();
        }
        else if (_currentMode == Mode.Draft)
        {
            _currentMode = Mode.Construction;
            
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

    public int GetCurrentRound()
    {
        return _currentRound;
    }

    public Player GetCurrentPlayer()
    {
        return _playerTurn;
    }
    #endregion

    public bool CanUseAbility(scriptablePower ability)
    {
        Debug.Log("can use ability ?");

        if (ActionPointsManager.instance.TryUseActionPoints(_playerTurn))
        {
            if (!IsAbilityInCooldown(ability))
            {
                Debug.Log("current ability cooldown 0");
                //ActionPointsManager.instance.UseActionPoint(_playerTurn);
                //SetAbilityCooldown(ability);
                Debug.Log("can use ability");
                return true;
            }
            else
            {
                Debug.Log("ability en cooldown");
            }
        }
        else
        {
            Debug.Log("no action points and / or no room ability on target");
        }
            
        
        Debug.Log("can't use ability");
        return false;
    }

    #region Cooldown 
    public void SetAbilityCooldown(scriptablePower ability)
    {
        Debug.Log("set ability cooldown " + ability.name + " " + _playerTurn);
        
        switch (ability.AbilityName)
        {
            case ("EMP"):
                if (_playerTurn == Player.Player1)
                {
                    _empCooldownPlayer1 = ability.Cooldown;
                }
                else
                {
                    _empCooldownPlayer2 = ability.Cooldown;
                }
                break;
            case ("Time Accelerator"):
                if (_playerTurn == Player.Player1)
                {
                    _timeAcceleratorCooldownPlayer1 = ability.Cooldown;
                }
                else
                {
                    _timeAcceleratorCooldownPlayer2 = ability.Cooldown;
                }
                break;
            case ("Simple Reveal"):
                if (_playerTurn == Player.Player1)
                {
                    _simpleRevealCooldownPlayer1 = ability.Cooldown;
                }
                else
                {
                    _simpleRevealCooldownPlayer2 = ability.Cooldown;
                }
                break;
            case ("Alternate Shot"):
                if (_playerTurn == Player.Player1)
                {
                    _alternateShotCooldownPlayer1 = ability.Cooldown;
                }
                else
                {
                    _alternateShotCooldownPlayer2 = ability.Cooldown;
                }
                break;
            case ("Scanner"):
                if (_playerTurn == Player.Player1)
                {
                    _scannerCooldownPlayer1 = ability.Cooldown;
                }
                else
                {
                    _scannerCooldownPlayer2 = ability.Cooldown;
                }
                break;
            case ("Capacitor"):
                if (_playerTurn == Player.Player1)
                {
                    _capacitorCooldownPlayer1 = ability.Cooldown;
                }
                else
                {
                    _capacitorCooldownPlayer2 = ability.Cooldown;
                }
                break;
            case ("Upgrade Shot"):
                if (_playerTurn == Player.Player1)
                {
                    _upgradeShotCooldownPlayer1 = ability.Cooldown;
                }
                else
                {
                    _upgradeShotCooldownPlayer2 = ability.Cooldown;
                }
                break;
            case ("Probe"):
                if (_playerTurn == Player.Player1)
                {
                    _probeCooldownPlayer1 = ability.Cooldown;
                }
                else
                {
                    _probeCooldownPlayer2 = ability.Cooldown;
                }
                break;
        }
    }

    public void AddEnemyAbilityOneCooldown(scriptablePower ability)
    {
        Debug.Log("set ability cooldown " + ability.name + " " + _playerTurn);

        switch (ability.AbilityName)
        {
            case ("EMP"):
                if (_playerTurn == Player.Player1)
                {
                    _empCooldownPlayer2 += 2;
                }
                else
                {
                    _empCooldownPlayer1 += 2;
                }
                break;
            case ("Time Accelerator"):
                if (_playerTurn == Player.Player1)
                {
                    _timeAcceleratorCooldownPlayer2 += 2;
                }
                else
                {
                    _timeAcceleratorCooldownPlayer1 += 2;
                }
                break;
            case ("Simple Reveal"):
                if (_playerTurn == Player.Player1)
                {
                    _simpleRevealCooldownPlayer2 += 2;
                }
                else
                {
                    _simpleRevealCooldownPlayer1 += 2;
                }
                break;
            case ("Alternate Shot"):
                if (_playerTurn == Player.Player1)
                {
                    _alternateShotCooldownPlayer2 += 2;
                }
                else
                {
                    _alternateShotCooldownPlayer1 += 2;
                }
                break;
            case ("Scanner"):
                if (_playerTurn == Player.Player1)
                {
                    _scannerCooldownPlayer2 += 2;
                }
                else
                {
                    _scannerCooldownPlayer1 += 2;
                }
                break;
            case ("Capacitor"):
                if (_playerTurn == Player.Player1)
                {
                    _capacitorCooldownPlayer2 += 2;
                }
                else
                {
                    _capacitorCooldownPlayer1 += 2;
                }
                break;
            case ("Upgrade Shot"):
                if (_playerTurn == Player.Player1)
                {
                    _upgradeShotCooldownPlayer2 += 2;
                }
                else
                {
                    _upgradeShotCooldownPlayer1 += 2;
                }
                break;
            case ("Probe"):
                if (_playerTurn == Player.Player1)
                {
                    _probeCooldownPlayer2 += 2;
                }
                else
                {
                    _probeCooldownPlayer1 += 2;
                }
                break;
        }
    }

    public bool IsAbilityInCooldown(scriptablePower ability)
    {
        Debug.Log("is ability in cooldown");

        switch (ability.AbilityName)
        {
            case ("Simple Hit"):
                return false; // jamais de cooldown
            case ("EMP"):
                if (_playerTurn == Player.Player1)
                {
                    if (_empCooldownPlayer1 == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (_empCooldownPlayer2 == 0)
                    {
                        return false;
                    }
                }
                break;
            case ("Time Accelerator"):
                if (_playerTurn == Player.Player1)
                {
                    if (_timeAcceleratorCooldownPlayer1 == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (_timeAcceleratorCooldownPlayer2 == 0)
                    {
                        return false;
                    }
                }
                break;
            case ("Simple Reveal"):
                if (_playerTurn == Player.Player1)
                {
                    if (_simpleRevealCooldownPlayer1 == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (_simpleRevealCooldownPlayer2 == 0)
                    {
                        return false;
                    }
                }
                break;
            case ("Alternate Shot"):
                if (_playerTurn == Player.Player1)
                {
                    if (_alternateShotCooldownPlayer1 == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (_alternateShotCooldownPlayer2 == 0)
                    {
                        return false;
                    }
                }
                break;
            case ("Scanner"):
                if (_playerTurn == Player.Player1)
                {
                    if (_scannerCooldownPlayer1 == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (_scannerCooldownPlayer2 == 0)
                    {
                        return false;
                    }
                }
                break;
            case ("Capacitor"):
                if (_playerTurn == Player.Player1)
                {
                    if (_capacitorCooldownPlayer1 == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (_capacitorCooldownPlayer2 == 0)
                    {
                        return false;
                    }
                }
                break;
            case ("Upgrade Shot"):
                if (_playerTurn == Player.Player1)
                {
                    if (_upgradeShotCooldownPlayer1 == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (_upgradeShotCooldownPlayer2 == 0)
                    {
                        return false;
                    }
                }
                break;
            case ("Probe"):
                if (_playerTurn == Player.Player1)
                {
                    if (_probeCooldownPlayer1 == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (_probeCooldownPlayer2 == 0)
                    {
                        return false;
                    }
                }
                break;
        }

        return true;
    }

    private void SetRoundCooldowns(Player player)
    {
        Debug.Log("set round cooldowns player " + player);
        CurrentPlayerLessCooldown(1);
    }

    public void CurrentPlayerLessCooldown(int amount)
    {
        Debug.Log("current player less coldown"); 

        if (_playerTurn == Player.Player1)
        {
            _simpleRevealCooldownPlayer1 = (int)Mathf.Clamp(_simpleRevealCooldownPlayer1 - amount, 0, Mathf.Infinity);
            _empCooldownPlayer1 = (int)Mathf.Clamp(_empCooldownPlayer1 - amount, 0, Mathf.Infinity);
            _timeAcceleratorCooldownPlayer1 = (int)Mathf.Clamp(_timeAcceleratorCooldownPlayer1 - amount, 0, Mathf.Infinity);
            _alternateShotCooldownPlayer1 = (int)Mathf.Clamp(_alternateShotCooldownPlayer1 - amount, 0, Mathf.Infinity);
            _scannerCooldownPlayer1 = (int)Mathf.Clamp(_scannerCooldownPlayer1 - amount, 0, Mathf.Infinity);
            _capacitorCooldownPlayer1 = (int)Mathf.Clamp(_capacitorCooldownPlayer1 - amount, 0, Mathf.Infinity);
            _upgradeShotCooldownPlayer1 = (int)Mathf.Clamp(_upgradeShotCooldownPlayer1 - amount, 0, Mathf.Infinity);
        }
        else
        {
            _simpleRevealCooldownPlayer2 = (int)Mathf.Clamp(_simpleRevealCooldownPlayer2 - amount, 0, Mathf.Infinity);
            _empCooldownPlayer2 = (int)Mathf.Clamp(_empCooldownPlayer2 - amount, 0, Mathf.Infinity);
            _timeAcceleratorCooldownPlayer2 = (int)Mathf.Clamp(_timeAcceleratorCooldownPlayer2 - amount, 0, Mathf.Infinity);
            _alternateShotCooldownPlayer2 = (int)Mathf.Clamp(_alternateShotCooldownPlayer2 - amount, 0, Mathf.Infinity);
            _scannerCooldownPlayer2 = (int)Mathf.Clamp(_scannerCooldownPlayer2 - amount, 0, Mathf.Infinity);
            _capacitorCooldownPlayer2 = (int)Mathf.Clamp(_capacitorCooldownPlayer2 - amount, 0, Mathf.Infinity);
            _upgradeShotCooldownPlayer2 = (int)Mathf.Clamp(_upgradeShotCooldownPlayer2 - amount, 0, Mathf.Infinity);
        }

        AbilityButtonsManager.instance.UpdateAllAbilityButtonsCooldown();
        UIManager.instance.CheckAbilityButtonsColor();
    }

    public int GetCurrentCooldown(scriptablePower ability)
    {
        switch (ability.AbilityName)
        {
            case ("EMP"):
                if (_playerTurn == Player.Player1)
                {
                    return _empCooldownPlayer1;
                }
                else
                {
                    return _empCooldownPlayer2;
                }
            case ("Time Accelerator"):
                if (_playerTurn == Player.Player1)
                {
                    return _timeAcceleratorCooldownPlayer1;
                }
                else
                {
                    return _timeAcceleratorCooldownPlayer2;
                }
            case ("Simple Reveal"):
                if (_playerTurn == Player.Player1)
                {
                    return _simpleRevealCooldownPlayer1;
                }
                else
                {
                    return _simpleRevealCooldownPlayer2;
                }
            case ("Alternate Shot"):
                if (_playerTurn == Player.Player1)
                {
                    return _alternateShotCooldownPlayer1;
                }
                else
                {
                    return _alternateShotCooldownPlayer2;
                }
            case ("Scanner"):
                if (_playerTurn == Player.Player1)
                {
                    return _scannerCooldownPlayer1;
                }
                else
                {
                    return _scannerCooldownPlayer2;
                }
            case ("Capacitor"):
                if (_playerTurn == Player.Player1)
                {
                    return _capacitorCooldownPlayer1;
                }
                else
                {
                    return _capacitorCooldownPlayer2;
                }
            case ("Upgrade Shot"):
                if (_playerTurn == Player.Player1)
                {
                    return _upgradeShotCooldownPlayer1;
                }
                else
                {
                    return _upgradeShotCooldownPlayer2;
                }
            case ("Probe"):
                if (_playerTurn == Player.Player1)
                {
                    return _probeCooldownPlayer1;
                }
                else
                {
                    return _probeCooldownPlayer2;
                }
        }

        return 0;
    }

    public int GetEnemyCurrentCooldown(scriptablePower ability)
    {
        switch (ability.AbilityName)
        {
            case ("EMP"):
                if (_playerTurn == Player.Player1)
                {
                    return _empCooldownPlayer2;
                }
                else
                {
                    return _empCooldownPlayer1;
                }
            case ("Time Accelerator"):
                if (_playerTurn == Player.Player1)
                {
                    return _timeAcceleratorCooldownPlayer2;
                }
                else
                {
                    return _timeAcceleratorCooldownPlayer1;
                }
            case ("Simple Reveal"):
                if (_playerTurn == Player.Player1)
                {
                    return _simpleRevealCooldownPlayer2;
                }
                else
                {
                    return _simpleRevealCooldownPlayer1;
                }
            case ("Alternate Shot"):
                if (_playerTurn == Player.Player1)
                {
                    return _alternateShotCooldownPlayer2;
                }
                else
                {
                    return _alternateShotCooldownPlayer1;
                }
            case ("Scanner"):
                if (_playerTurn == Player.Player1)
                {
                    return _scannerCooldownPlayer2;
                }
                else
                {
                    return _scannerCooldownPlayer1;
                }
            case ("Capacitor"):
                if (_playerTurn == Player.Player1)
                {
                    return _capacitorCooldownPlayer2;
                }
                else
                {
                    return _capacitorCooldownPlayer1;
                }
            case ("Upgrade Shot"):
                if (_playerTurn == Player.Player1)
                {
                    return _upgradeShotCooldownPlayer2;
                }
                else
                {
                    return _upgradeShotCooldownPlayer1;
                }
            case ("Probe"):
                if (_playerTurn == Player.Player1)
                {
                    return _probeCooldownPlayer2;
                }
                else
                {
                    return _probeCooldownPlayer1;
                }
        }

        return 0;
    }
    #endregion
}
