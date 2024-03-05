using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AlternateShotDirection
{
    Horizontal,
    Vertical
}

public enum UpgradeShotStep
{
    RevealOneTile,
    DestroyOneTile,
    DestroyThreeTilesInDiagonal,
    DestroyFiveTilesInCross
}

public class AbilityButtonsManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static AbilityButtonsManager instance;

    [SerializeField] List<GameObject> _abilitiesButtonsGameObjects = new List<GameObject>();
    private List<AbilityButton> _abilitiesButtons = new List<AbilityButton>();

    private Tile _target;

    private AbilityButton _selectedButton = null;
    private List<Tile> _selectedTiles = new List<Tile>();

    // ----- Alternate Shot ----- //
    private AlternateShotDirection _currentAlternateShotDirectionPlayer1 = AlternateShotDirection.Horizontal;
    private AlternateShotDirection _currentAlternateShotDirectionPlayer2 = AlternateShotDirection.Horizontal;

    private AlternateShotDirection _currentAlternateShotDirection;
    // ----- Alternate Shot ----- //

    // ----- Capacitor ----- //
    private bool _simpleHitX2Player1;
    private bool _simpleHitX2Player2;
    public bool SimpleHitX2Player1 { get => _simpleHitX2Player1; set => _simpleHitX2Player1 = value; }
    public bool SimpleHitX2Player2 { get => _simpleHitX2Player2; set => _simpleHitX2Player2 = value; }
    // ----- Capacitor ----- //

    // ----- Upgrade Shot ----- //
    private UpgradeShotStep _currentUpgradeShotStepPlayer1 = UpgradeShotStep.RevealOneTile;
    private UpgradeShotStep _currentUpgradeShotStepPlayer2 = UpgradeShotStep.RevealOneTile;

    private UpgradeShotStep _currentUpgradeShotStep = UpgradeShotStep.RevealOneTile;
    // ----- Upgrade Shot ----- //

    private int _currentProbeCount = 0;

    private List<Tuple<String, List<Tile>>> _lastRoundActionsPlayer1 = new List<Tuple<String, List<Tile>>>();
    private List<Tuple<String, List<Tile>>> _lastRoundActionsPlayer2 = new List<Tuple<String, List<Tile>>>();
    private List<Tile> _currentActionTargetTiles = new List<Tile>();
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (GameObject button in _abilitiesButtonsGameObjects)
        {
            _abilitiesButtons.Add(button.GetComponentInChildren<AbilityButton>());
        }
    }
    
    public void Rewind()
    {
        UpdateRoomsRewind();
        StartCoroutine(RewindCoroutine());
    }

    public IEnumerator RewindCoroutine()
    {
        // hide ui

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            CameraController.instance.SwitchRewindPlayerShipCameraDirectly(Player.Player1);

            yield return new WaitForSeconds(2f);

            if (_lastRoundActionsPlayer2.Count > 0)
            {
                Debug.Log("last round actions player 2 count > 0");
                foreach (var action in _lastRoundActionsPlayer2)
                {
                    Debug.Log("Last round action " + action.Item1);
                    RewindAction(action.Item1, action.Item2, Player.Player1);
                    yield return new WaitForSeconds(2f);
                }
            }

            CameraController.instance.SwitchPlayerShipCameraDirectly(Player.Player2);
        }
        else
        {
            CameraController.instance.SwitchRewindPlayerShipCameraDirectly(Player.Player2);

            yield return new WaitForSeconds(2f);

            if (_lastRoundActionsPlayer1 != null)
            {
                Debug.Log("last round actions player 1 count > 0");
                foreach (var action in _lastRoundActionsPlayer1)
                {
                    Debug.Log("Last round action " + action.Item1);
                    RewindAction(action.Item1, action.Item2, Player.Player2);
                    yield return new WaitForSeconds(2f);
                }
            }

            CameraController.instance.SwitchPlayerShipCameraDirectly(Player.Player1);
        }

        // show ui
        yield return null;
    }

    private void RewindAction(string actionName, List<Tile> targets, Player player)
    {
        Debug.Log("rewind action");
        // important -> target sur le ship normal à convertir en target sur le ship rewind
        List<Tile> targetsOnRewind = new List<Tile>();
 
        foreach(Tile target in targets)
        {
            if (player == Player.Player1)
            {
                foreach(Tile rewindTile in GameManager.instance.TilesRewindPlayer1)
                {
                    if (rewindTile.name == target.name)
                    {
                        Debug.Log("found rewind tile " + rewindTile.name);
                        targetsOnRewind.Add(rewindTile);
                    }
                }
            }
            else
            {
                foreach (Tile rewindTile in GameManager.instance.TilesRewindPlayer2)
                {
                    if (rewindTile.name == target.name)
                    {
                        Debug.Log("found rewind tile " + rewindTile.name);
                        targetsOnRewind.Add(rewindTile);
                    }
                }
            }
        }

        _target = targetsOnRewind[0];

        switch (actionName)
        {
            case ("AlternateShot"):
                UseAlternateShot();
                break;
            case ("SimpleHit"):
                DestroyRoom(_target);
                
                break;
            case ("SimpleReveal"):
                RevealRoom(_target);
                break;
            case ("EMP"):
                UseEMP();
                break;
            case ("Scanner"):
                UseScanner();
                break;
            case ("TimeAccelerator"):
                UseTimeAccelerator();
                break;
            case ("Capacitor"):
                UseCapacitor();
                break;
            case ("UpgradeShot"):
                UseUpgradeShot();
                break;
            case ("Probe"):
                UseProbe();
                break;
        }

        UpdateRoomsRewind();
    }

    public void UpdateAllAbilityButtonsCooldown()
    {
        Debug.Log("update all ability buttons cooldown");
        foreach(AbilityButton button in _abilitiesButtons)
        {
            button.UpdateCooldown();
        }
    }

    public void ResetRoundAbilityButtons()
    {
        if (_selectedButton != null)
        {
            _selectedButton.DeselectButton();
            _selectedButton = null;
        }

        DeselectAbilityTiles();

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            if (_lastRoundActionsPlayer1.Count > 0)
                _lastRoundActionsPlayer1.Clear();
        }
        else
        {
            if (_lastRoundActionsPlayer2.Count > 0)
                _lastRoundActionsPlayer2.Clear();
        }
    }

    public void SelectAbilityButton(AbilityButton button)
    {
        Debug.Log("select ability button");
        _selectedButton = button;
        _target = GameManager.instance.TargetOnTile;

        // Deselect
        foreach (AbilityButton abilityButton in _abilitiesButtons)
        {
            if (abilityButton != button)
            {
                abilityButton.DeselectButton();
            }
        }

        DeselectAbilityTiles();

        button.SelectButton();
        SelectTilesAroundTarget();

        UIManager.instance.ShowFireButton();
        UIManager.instance.CheckAbilityButtonsColor();
        UIManager.instance.ShowFicheAbility(_selectedButton.GetAbility());
    }

    public void DeselectAbilityButton(AbilityButton button)
    {
        button.DeselectButton();
        _selectedButton = null;
        DeselectAbilityTiles();

        UIManager.instance.ShowValidateCombat();
        UIManager.instance.CheckAbilityButtonsColor();
        UIManager.instance.HideFicheAbility();
    }

    public void ChangeSelectedTilesOnTargetPos()
    {
        _target = GameManager.instance.TargetOnTile;
        DeselectAbilityTiles();
        SelectTilesAroundTarget();
    }

    public void SelectTilesAroundTarget()
    {
        Debug.Log("select tiles around target");
        if (_selectedButton == null)
        {
            DeselectAbilityTiles();
            return;
        }

        switch (_selectedButton.GetAbility().name)
        {
            case ("AlternateShot"):
                AlternateShot_SelectAbilityTiles();
                break;
            case ("SimpleHit"):
                SimpleHit_SelectAbilityTiles();
                break;
            case ("SimpleReveal"):
                SimpleReveal_SelectAbilityTiles();
                break;
            case ("EMP"):
                EMP_SelectAbilityTiles();
                break;
            case ("Scanner"):
                Scanner_SelectAbilityTiles();
                break;
            case ("UpgradeShot"):
                UpgradeShot_SelectAbilityTiles();
                break;
            case ("Probe"):
                Probe_SelectAbilityTiles();
                break;
            case ("TimeAccelerator"):
            case ("Capacitor"):
                break; // Pas de tile à sélectionner lol

        }
    }

    public void DeselectAbilityTiles()
    {
        Debug.Log("deselect ability tiles");
        if (_selectedTiles == null)
        {
            return;
        }

        foreach (Tile tile in _selectedTiles)
        {
            tile.IsAbilitySelected = false;
        }

        _selectedTiles.Clear();
    }

    #region Selections

    #region Scanner Selection
    private void Scanner_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles scanner");
        if (_selectedTiles != null)
        {
            DeselectAbilityTiles();
            _selectedTiles.Clear();
        }

        // Center
        _target.IsAbilitySelected = true;
        _selectedTiles.Add(_target);

        #region Top
        // Top
        bool canGoTop = true;
        Tile currentTile = _target;
        while (canGoTop)
        {
            if (currentTile.TopTile != null)
            {
                currentTile.TopTile.IsAbilitySelected = true;
                _selectedTiles.Add(currentTile.TopTile);

                currentTile = currentTile.TopTile;
            }
            else
            {
                canGoTop = false;
            }
        }
        #endregion

        #region Bottom
        // Bottom
        bool canGoBottom = true;
        currentTile = _target;
        while (canGoBottom)
        {
            if (currentTile.BottomTile != null)
            {
                currentTile.BottomTile.IsAbilitySelected = true;
                _selectedTiles.Add(currentTile.BottomTile);

                currentTile = currentTile.BottomTile;
            }
            else
            {
                canGoBottom = false;
            }
        }
        #endregion
    }
    #endregion

    #region EMP Selection
    private void EMP_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles emp");
        if (_selectedTiles != null)
        {
            DeselectAbilityTiles();
            _selectedTiles.Clear();
        }

        // Center
        _target.IsAbilitySelected = true;
        _selectedTiles.Add(_target);

        #region Right, Left, Bottom & Top
        // Right
        if (_target.RightTile != null)
        {
            _target.RightTile.IsAbilitySelected = true;
            _selectedTiles.Add(_target.RightTile);
        }

        // Left
        if (_target.LeftTile != null)
        {
            _target.LeftTile.IsAbilitySelected = true;
            _selectedTiles.Add(_target.LeftTile);
        }

        // Bottom
        if (_target.BottomTile != null)
        {
            _target.BottomTile.IsAbilitySelected = true;
            _selectedTiles.Add(_target.BottomTile);
        }

        // Top
        if (_target.TopTile != null)
        {
            _target.TopTile.IsAbilitySelected = true;
            _selectedTiles.Add(_target.TopTile);
        }
        #endregion

        #region Diag Top Left & Right, Bottom Left & Right
        // Diag top left
        if (_target.DiagTopLeftTile != null)
        {
            _target.DiagTopLeftTile.IsAbilitySelected = true;
            _selectedTiles.Add(_target.DiagTopLeftTile);
        }

        // Diag top right
        if (_target.DiagTopRightTile != null)
        {
            _target.DiagTopRightTile.IsAbilitySelected = true;
            _selectedTiles.Add(_target.DiagTopRightTile);
        }

        // Diag bottom left
        if (_target.DiagBottomLeftTile != null)
        {
            _target.DiagBottomLeftTile.IsAbilitySelected = true;
            _selectedTiles.Add(_target.DiagBottomLeftTile);
        }

        // Diag bottom right
        if (_target.DiagBottomRightTile != null)
        {
            _target.DiagBottomRightTile.IsAbilitySelected = true;
            _selectedTiles.Add(_target.DiagBottomRightTile);
        }
        #endregion
    }
    #endregion

    #region Alternate Shot Selection
    private void AlternateShot_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles alternate shot");
        if (_selectedTiles != null)
        {
            DeselectAbilityTiles();
            _selectedTiles.Clear();
        }

        _currentAlternateShotDirection = GetCurrentPlayerAlternateShotDirection();

        if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
        {
            if (_target.LeftTile != null)
            {
                _target.LeftTile.IsAbilitySelected = true;
                _selectedTiles.Add(_target.LeftTile);
            }

            if (_target.RightTile != null)
            {
                _target.RightTile.IsAbilitySelected = true;
                _selectedTiles.Add(_target.RightTile);
            }
        }
        else
        {
            if (_target.TopTile != null)
            {
                _target.TopTile.IsAbilitySelected = true;
                _selectedTiles.Add(_target.TopTile);
            }

            if (_target.BottomTile != null)
            {
                _target.BottomTile.IsAbilitySelected = true;
                _selectedTiles.Add(_target.BottomTile);
            }
        }
    }
    #endregion

    #region Simple Hit & Reveal + Probe selection
    private void SimpleHit_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles simple hit");

        if (GetIfSimpleHitXS())
        {
            Debug.Log("simple hit x2 selection");

            if (_selectedTiles != null)
            {
                DeselectAbilityTiles();
                _selectedTiles.Clear();
            }

            _target.IsAbilitySelected = true;
            _selectedTiles.Add(_target);

            // right
            if (_target.RightTile != null)
            {
                _target.RightTile.IsAbilitySelected = true;
                _selectedTiles.Add(_target.RightTile);
            }

            // bottom
            if (_target.BottomTile != null)
            {
                _target.BottomTile.IsAbilitySelected = true;
                _selectedTiles.Add(_target.BottomTile);
            }

            // diag bottom right
            if (_target.DiagBottomRightTile != null)
            {
                _target.DiagBottomRightTile.IsAbilitySelected = true;
                _selectedTiles.Add(_target.DiagBottomRightTile);
            }
        }
        else
        {
            SelectOnlyTargetTile();
        }
    }

    private void SimpleReveal_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles simple reveal");
        SelectOnlyTargetTile();
    }

    private void Probe_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles probe");
        SelectOnlyTargetTile();
    }

    private void SelectOnlyTargetTile()
    {
        if (_selectedTiles != null)
        {
            DeselectAbilityTiles();
            _selectedTiles.Clear();
        }

        _target.IsAbilitySelected = true;
        _selectedTiles.Add(_target);
    }
    #endregion

    #region Upgrade Shot
    private void UpgradeShot_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles upgrade shot");
        if (_selectedTiles != null)
        {
            DeselectAbilityTiles();
            _selectedTiles.Clear();
        }

        _currentUpgradeShotStep = GetCurrentPlayerUpgradeShotStep();

        switch (_currentUpgradeShotStep)
        {
            case (UpgradeShotStep.RevealOneTile):
                _target.IsAbilitySelected = true;
                _selectedTiles.Add(_target);
                break;
            case (UpgradeShotStep.DestroyOneTile):
                _target.IsAbilitySelected = true;
                _selectedTiles.Add(_target);
                break;
            case (UpgradeShotStep.DestroyThreeTilesInDiagonal):

                if (_target.DiagTopLeftTile != null)
                {
                    _target.DiagTopLeftTile.IsAbilitySelected = true;
                    _selectedTiles.Add(_target.DiagTopLeftTile);
                }

                if (_target.DiagBottomRightTile != null)
                {
                    _target.DiagBottomRightTile.IsAbilitySelected = true;
                    _selectedTiles.Add(_target.DiagBottomRightTile);
                }

                _target.IsAbilitySelected = true;
                _selectedTiles.Add(_target);
                break;
            case (UpgradeShotStep.DestroyFiveTilesInCross):

                if (_target.LeftTile != null)
                {
                    _target.LeftTile.IsAbilitySelected = true;
                    _selectedTiles.Add(_target.LeftTile);
                }

                if (_target.RightTile != null)
                {
                    _target.RightTile.IsAbilitySelected = true;
                    _selectedTiles.Add(_target.RightTile);
                }

                if (_target.TopTile != null)
                {
                    _target.TopTile.IsAbilitySelected = true;
                    _selectedTiles.Add(_target.TopTile);
                }

                if (_target.BottomTile != null)
                {
                    _target.BottomTile.IsAbilitySelected = true;
                    _selectedTiles.Add(_target.BottomTile);
                }

                _target.IsAbilitySelected = true;
                _selectedTiles.Add(_target);
                break;
        }
    }
    #endregion

    #endregion

    #region Use Abilities

    public List<GameObject> GetAbilityButtonsList()
    {
        return _abilitiesButtonsGameObjects;
    }

    public AbilityButton GetCurrentlySelectedAbilityButton()
    {
        return _selectedButton;
    }

    #region Simple Hit X2
    public void ActivateSimpleHitX2()
    {
        Debug.Log("activate simplte hit x2");
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            _simpleHitX2Player1 = true;
        }
        else 
        {
            _simpleHitX2Player2 = true;
        }

        UIManager.instance.CheckSimpleHitX2Img();
    }

    public void DesactivateSimpleHitX2IfActivated()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            if (_simpleHitX2Player1)
            {
                _simpleHitX2Player1 = false;
            }
        }
        else
        {
            if (_simpleHitX2Player2)
            {
                _simpleHitX2Player2 = false;
            }
        }

        UIManager.instance.CheckSimpleHitX2Img();
    }

    public bool GetIfSimpleHitXS()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            return _simpleHitX2Player1;
        }
        else
        {
            return _simpleHitX2Player2;
        }
    }
    #endregion

    public void UseSelectedAbility()
    {
        switch (_selectedButton.GetAbility().name)
        {
            case ("AlternateShot"):
                UseAlternateShot();
                break;
            case ("SimpleHit"):
                UseSimpleHit();
                break;
            case ("SimpleReveal"):
                UseSimpleReveal();
                break;
            case ("EMP"):
                UseEMP();
                break;
            case ("Scanner"):
                UseScanner();
                break;
            case ("TimeAccelerator"):
                UseTimeAccelerator();
                break;
            case ("Capacitor"):
                UseCapacitor();
                break;
            case ("UpgradeShot"):
                UseUpgradeShot();
                break;
            case ("Probe"):
                UseProbe();
                break;
        }

        if (_selectedButton.GetAbility().name != "Probe")
            DeselectAbilityButton(_selectedButton);
    }

    private void UseExample()
    {
        // ----- REWIND ----- //
        _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("Example");
        // ----- REWIND ----- //
        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        // code

        UpdateHiddenRooms(); // si destroy ou reveal 
        UIManager.instance.CheckAbilityButtonsColor();
    }

    #region Scanner
    private void UseScanner()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear(); ;
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("Scanner");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        #region Top
        // Top
        bool canGoTop = true;
        Tile currentTile = _target;
        while (canGoTop)
        {
            if (currentTile.TopTile != null)
            {
                if (currentTile.TopTile.IsOccupied)
                {
                    currentTile.TopTile.IsReavealed = true;
                }
                else
                {
                    currentTile.TopTile.IsMissed = true;
                }

                currentTile = currentTile.TopTile;
            }
            else
            {
                canGoTop = false;
            }
        }
        #endregion 

        #region Bottom
        // Bottom
        bool canGoBottom = true;
        currentTile = _target;
        while (canGoBottom)
        {
            if (currentTile.BottomTile != null)
            {
                if (currentTile.BottomTile.IsOccupied)
                {
                    currentTile.BottomTile.IsReavealed = true;
                }
                else
                {
                    currentTile.BottomTile.IsMissed = true;
                }

                currentTile = currentTile.BottomTile;
            }
            else
            {
                canGoBottom = false;
            }
        }
        #endregion

        // Center
        _target.IsReavealed = true;

        if (_target.IsOccupied)
        {
            UIManager.instance.ShowFicheRoom(GameManager.instance.TargetOnTile.Room.RoomData);
        }
        else
        {
            GameManager.instance.TargetOnTile.IsMissed = true;

            UIManager.instance.HideFicheRoom();
        }

        UpdateHiddenRooms();
        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region Capacitor
    private void UseCapacitor()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("Capacitor");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        ActivateSimpleHitX2();
        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region EMP
    private void UseEMP()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("EMP");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        DestroyRoom(_target);

        // Desactivate for one turn room's abilities around the target (+1 cooldown)
        List<Room> roomsToDesactivate = new List<Room>();

        #region Right, Left, Bottom & Top
        // Right
        if (_target.RightTile != null)
        {
            if (_target.RightTile.Room != null)
            {
                if (!roomsToDesactivate.Contains(_target.RightTile.Room))
                {
                    roomsToDesactivate.Add(_target.RightTile.Room);
                }
            }
        }

        // Left
        if (_target.LeftTile != null)
        {
            if (_target.LeftTile.Room != null)
            {
                if (!roomsToDesactivate.Contains(_target.LeftTile.Room))
                {
                    roomsToDesactivate.Add(_target.LeftTile.Room);
                }
            }
        }

        // Bottom
        if (_target.BottomTile != null)
        {
            if (_target.BottomTile.Room != null)
            {
                if (!roomsToDesactivate.Contains(_target.BottomTile.Room))
                {
                    roomsToDesactivate.Add(_target.BottomTile.Room);
                }
            }
        }

        // Top
        if (_target.TopTile != null)
        {
            if (_target.TopTile.Room != null)
            {
                if (!roomsToDesactivate.Contains(_target.TopTile.Room))
                {
                    roomsToDesactivate.Add(_target.TopTile.Room);
                }
            }
        }
        #endregion

        #region Diag Top Left & Right, Bottom Left & Right
        // Diag top left
        if (_target.DiagTopLeftTile != null)
        {
            if (_target.DiagTopLeftTile.Room != null)
            {
                if (!roomsToDesactivate.Contains(_target.DiagTopLeftTile.Room))
                {
                    roomsToDesactivate.Add(_target.DiagTopLeftTile.Room);
                }
            }
        }

        // Diag top right
        if (_target.DiagTopRightTile != null)
        {
            if (_target.DiagTopRightTile.Room != null)
            {
                if (!roomsToDesactivate.Contains(_target.DiagTopRightTile.Room))
                {
                    roomsToDesactivate.Add(_target.DiagTopRightTile.Room);
                }
            }
        }

        // Diag bottom left
        if (_target.DiagBottomLeftTile != null)
        {
            if (_target.DiagBottomLeftTile.Room != null)
            {
                if (!roomsToDesactivate.Contains(_target.DiagBottomLeftTile.Room))
                {
                    roomsToDesactivate.Add(_target.DiagBottomLeftTile.Room);
                }
            }
        }

        // Diag bottom right
        if (_target.DiagBottomRightTile != null)
        {
            if (_target.DiagBottomRightTile.Room != null)
            {
                if (!roomsToDesactivate.Contains(_target.DiagBottomRightTile.Room))
                {
                    roomsToDesactivate.Add(_target.DiagBottomRightTile.Room);
                }
            }
        }
        #endregion

        foreach (Room room in roomsToDesactivate)
        {
            if (room.RoomData.RoomAbility != null)
            {
                Debug.Log("cooldown " + GameManager.instance.GetCurrentCooldown(room.RoomData.RoomAbility));
                if (GameManager.instance.GetEnemyCurrentCooldown(room.RoomData.RoomAbility) == 0)
                {
                    GameManager.instance.AddEnemyAbilityOneCooldown(room.RoomData.RoomAbility);
                    Debug.Log("add 1 cooldown to " + room.RoomData.RoomAbility.name);
                }
            }
        }

        UpdateHiddenRooms();
        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region Simple Hit
    private void UseSimpleHit()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        DestroyRoom(_target);

        TargetController.instance.ChangeTargetColorToRed();

        if (GetIfSimpleHitXS())
        {
            Debug.Log("simple hit x2");
            AddActionToCurrentPlayerRound("SimpleHitX2");

            // Try destroy right
            if (_target.RightTile != null)
            {
                DestroyRoom(_target.RightTile);
            }

            // Try destroy bottom
            if (_target.BottomTile != null)
            {
                DestroyRoom(_target.BottomTile);
            }

            // Try destroy diag bottom right
            if (_target.DiagBottomRightTile != null)
            {
                DestroyRoom(_target.DiagBottomRightTile);
            }

            DesactivateSimpleHitX2IfActivated();
        }
        else
        {
            AddActionToCurrentPlayerRound("SimpleHit");
        }

        UpdateHiddenRooms(); 
        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region Simple Reveal
    private void UseSimpleReveal()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("SimpleReveal");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        RevealRoom(_target);

        UpdateHiddenRooms(); // si destroy ou reveal 
        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region Alternate Shot
    private void UseAlternateShot()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        Debug.Log(_target.name);
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("AlternateShot");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        _currentAlternateShotDirection = GetCurrentPlayerAlternateShotDirection();

        if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
        {
            if (_target.LeftTile != null)
            {
                DestroyRoom(_target.LeftTile);
            }

            if (_target.RightTile != null)
            {
                DestroyRoom(_target.RightTile);
            }
        }
        else
        {
            if (_target.TopTile != null)
            {
                DestroyRoom(_target.TopTile);
            }

            if (_target.BottomTile != null)
            {
                DestroyRoom(_target.BottomTile);
            }
        }

        DestroyRoom(_target);

        ChangeAlternateShotDirection();

        UpdateHiddenRooms(); // si destroy ou reveal 
        UIManager.instance.CheckAbilityButtonsColor();
    }

    private void AddActionToCurrentPlayerRound(string actionName)
    {
        Debug.Log("action " + actionName);
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            Debug.Log("add action to player 1");
            _lastRoundActionsPlayer1.Add(Tuple.Create(actionName, _currentActionTargetTiles));
        }
        else
        {
            Debug.Log("add action to player 2");
            _lastRoundActionsPlayer2.Add(Tuple.Create(actionName, _currentActionTargetTiles));
        }
    }

    public AlternateShotDirection GetCurrentPlayerAlternateShotDirection()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            return _currentAlternateShotDirectionPlayer1;
        }
        else
        {
            return _currentAlternateShotDirectionPlayer2;
        }
    }

    private void ChangeAlternateShotDirection()
    {
        _currentAlternateShotDirection = GetCurrentPlayerAlternateShotDirection();

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
            {
                _currentAlternateShotDirection = AlternateShotDirection.Vertical;
            }
            else
            {
                _currentAlternateShotDirection = AlternateShotDirection.Horizontal;
            }

            _currentAlternateShotDirectionPlayer1 = _currentAlternateShotDirection;
        }
        else
        {
            if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
            {
                _currentAlternateShotDirection = AlternateShotDirection.Vertical;
            }
            else
            {
                _currentAlternateShotDirection = AlternateShotDirection.Horizontal;
            }

            _currentAlternateShotDirectionPlayer2 = _currentAlternateShotDirection;
        }

        UIManager.instance.CheckAlternateShotDirectionImgRotation();
    }
    #endregion

    #region Time Accelerator
    private void UseTimeAccelerator()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("TimeAccelerator");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        // All player cooldown - 1
        GameManager.instance.CurrentPlayerLessCooldown(1);

        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region Upgrade Shot
    private void UseUpgradeShot()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("UpgradeShot");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        _currentUpgradeShotStep = GetCurrentPlayerUpgradeShotStep();

        switch (_currentUpgradeShotStep)
        {
            case (UpgradeShotStep.RevealOneTile):
                RevealRoom(_target);
                break;
            case (UpgradeShotStep.DestroyOneTile):
                DestroyRoom(_target);
                break;
            case (UpgradeShotStep.DestroyThreeTilesInDiagonal):

                if (_target.DiagTopLeftTile != null)
                {
                    DestroyRoom(_target.DiagTopLeftTile);
                }

                if (_target.DiagBottomRightTile != null)
                {
                    DestroyRoom(_target.DiagBottomRightTile);
                }

                DestroyRoom(_target);
                break;
            case (UpgradeShotStep.DestroyFiveTilesInCross):

                if (_target.LeftTile != null)
                {
                    DestroyRoom(_target.LeftTile);
                }

                if (_target.RightTile != null)
                {
                    DestroyRoom(_target.RightTile);
                }

                if (_target.TopTile != null)
                {
                    DestroyRoom(_target.TopTile);
                }

                if (_target.BottomTile != null)
                {
                    DestroyRoom(_target.BottomTile);
                }

                DestroyRoom(_target);
                break;
        }

        ChangeUpgradeShotStep();

        UpdateHiddenRooms(); 
        UIManager.instance.CheckAbilityButtonsColor();
    }

    private UpgradeShotStep GetCurrentPlayerUpgradeShotStep()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            return _currentUpgradeShotStepPlayer1;
        }
        else
        {
            return _currentUpgradeShotStepPlayer2;
        }
    }

    private void ChangeUpgradeShotStep()
    {
        _currentUpgradeShotStep = GetCurrentPlayerUpgradeShotStep();

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            if (_currentUpgradeShotStep == UpgradeShotStep.RevealOneTile)
            {
                _currentUpgradeShotStep = UpgradeShotStep.DestroyOneTile;
            }
            else if (_currentUpgradeShotStep == UpgradeShotStep.DestroyOneTile)
            {
                _currentUpgradeShotStep = UpgradeShotStep.DestroyThreeTilesInDiagonal;
            }
            else if (_currentUpgradeShotStep == UpgradeShotStep.DestroyThreeTilesInDiagonal)
            {
                _currentUpgradeShotStep = UpgradeShotStep.DestroyFiveTilesInCross;
            }
            // Else reste en destroy five tiles in cross

            _currentUpgradeShotStepPlayer1 = _currentUpgradeShotStep;
        }
        else
        {
            if (_currentUpgradeShotStep == UpgradeShotStep.RevealOneTile)
            {
                _currentUpgradeShotStep = UpgradeShotStep.DestroyOneTile;
            }
            else if (_currentUpgradeShotStep == UpgradeShotStep.DestroyOneTile)
            {
                _currentUpgradeShotStep = UpgradeShotStep.DestroyThreeTilesInDiagonal;
            }
            else if (_currentUpgradeShotStep == UpgradeShotStep.DestroyThreeTilesInDiagonal)
            {
                _currentUpgradeShotStep = UpgradeShotStep.DestroyFiveTilesInCross;
            }
            // Else reste en destroy five tiles in cross

            _currentUpgradeShotStepPlayer2 = _currentUpgradeShotStep;
        }

        //UIManager.instance.CheckAlternateShotDirectionImgRotation();
    }
    #endregion

    #region Probe
    private void UseProbe()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("Probe");
        // ----- REWIND ----- //

        _currentProbeCount++;
        UIManager.instance.ShowProbeCount(_currentProbeCount);

        RevealRoom(_target);

        if (_currentProbeCount == 3)
        {
            _selectedButton.SetCooldown();
            ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

            DesactivateSimpleHitX2IfActivated();
            UIManager.instance.CheckAbilityButtonsColor();
            DeselectAbilityButton(_selectedButton);
            UIManager.instance.HideProbeCount();
        }

        UpdateHiddenRooms(); 
    }

    public bool IsProbeStarted()
    {
        if (_selectedButton != null)
        {
            return _selectedButton.GetAbility().AbilityName == "Probe" && _currentProbeCount > 0;
        }
        return false;
    }

    public void ResetCurrentProbeCount()
    {
        _currentProbeCount = 0;
    }
    #endregion

    private void UpdateHiddenRooms()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            GameManager.instance.ShowOnlyDestroyedAndReavealedRooms(Player.Player2);
        }
        else
        {
            GameManager.instance.ShowOnlyDestroyedAndReavealedRooms(Player.Player1);
        }
    }

    private void UpdateRoomsRewind()
    {
        GameManager.instance.ShowAllRewindRooms(Player.Player1);
        GameManager.instance.ShowAllRewindRooms(Player.Player2);
    }

    private void DestroyRoom(Tile tile)
    {
        Debug.Log("destroy room " + tile.name);
        if (tile.IsOccupied && !tile.Room.IsRoomDestroyed)
        {
            Debug.Log("hit room " + tile.Room.name);
            tile.RoomTileSpriteRenderer.color = Color.black;
            tile.IsDestroyed = true;

            GameManager.instance.CheckIfTileRoomIsCompletelyDestroyed(tile);

            UIManager.instance.ShowFicheRoom(tile.Room.RoomData);
        }
        else
        {
            tile.IsMissed = true;
            Debug.Log("no room on hit " + tile.name);

            UIManager.instance.HideFicheRoom();
        }
    }

    private void RevealRoom(Tile tile)
    {
        Debug.Log("reveal room " + tile.name);
        if (tile.IsOccupied && !tile.Room.IsRoomDestroyed)
        {
            Debug.Log("hit room " + tile.Room.name);
            tile.IsReavealed = true;

            GameManager.instance.CheckIfTileRoomIsCompletelyDestroyed(tile);

            UIManager.instance.ShowFicheRoom(tile.Room.RoomData);
        }
        else
        {
            tile.IsMissed = true;
            Debug.Log("no room on hit " + tile.name);

            UIManager.instance.HideFicheRoom();
        }
    }
    #endregion
}
