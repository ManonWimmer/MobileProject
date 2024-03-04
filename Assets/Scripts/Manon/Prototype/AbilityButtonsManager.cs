using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    public void UpdateAllAbilityButtonsCooldown()
    {
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

    #region Simple Hit & Reveal
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
        }

        DeselectAbilityButton(_selectedButton);
    }

    private void UseExample()
    {
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
        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        ActivateSimpleHitX2();
        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region EMP
    private void UseEMP()
    {
        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        _selectedButton.SetCooldown();

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
        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        DestroyRoom(_target);

        TargetController.instance.ChangeTargetColorToRed();

        if (GetIfSimpleHitXS())
        {
            Debug.Log("simple hit x2");

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

        UpdateHiddenRooms(); 
        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region Simple Reveal
    private void UseSimpleReveal()
    {
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
            else
            {
                _currentUpgradeShotStep = UpgradeShotStep.RevealOneTile;
            }

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
            else
            {
                _currentUpgradeShotStep = UpgradeShotStep.RevealOneTile;
            }

            _currentUpgradeShotStepPlayer2 = _currentUpgradeShotStep;
        }

        //UIManager.instance.CheckAlternateShotDirectionImgRotation();
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
