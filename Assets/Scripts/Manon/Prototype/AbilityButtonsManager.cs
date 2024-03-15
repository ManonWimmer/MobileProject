using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

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
    public AbilityButton SelectedButton { get => _selectedButton; set => _selectedButton = value; }

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

    private string _lastAbilityUsed;

    private Tile _tempTileRepaired;

    List<Tile> tempTargets = new List<Tile>();

    public bool IsInRewind;
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

    #region Rewind
    public void Rewind()
    {
        UpdateRoomsRewind();
        UIManager.instance.ShowRewindUI();
        StartCoroutine(RewindCoroutine());
    }

    public IEnumerator RewindCoroutine()
    {
        IsInRewind = true;

        if (audioManager.instance != null)
            audioManager.instance.PlaySoundRewindStart();

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            CameraController.instance.SwitchRewindPlayerShipCameraDirectly(Player.Player1);

            yield return new WaitForSeconds(1f);

            List<string> lastRoundActionNames = new List<string>();

            if (_lastRoundActionsPlayer2.Count > 0)
            {
                Debug.Log("last round actions player 2 count : " + +_lastRoundActionsPlayer2.Count);
                foreach (var action in _lastRoundActionsPlayer2)
                {
                    Debug.Log("Last round action " + action.Item1 + action.Item2[0].name);
                    lastRoundActionNames.Add(action.Item1);
                    EnemyActionsManager.instance.InitEnemyActions(lastRoundActionNames);

                    yield return StartCoroutine(RewindAction(action.Item1, action.Item2, Player.Player1));

                    
                }
            }
            CameraController.instance.SwitchPlayerShipCameraDirectly(Player.Player2);
            _lastRoundActionsPlayer2.Clear();
        }
        else
        {
            CameraController.instance.SwitchRewindPlayerShipCameraDirectly(Player.Player2);

            yield return new WaitForSeconds(1f);

            List<string> lastRoundActionNames = new List<string>();

            if (_lastRoundActionsPlayer1 != null)
            {
                Debug.Log("last round actions player 1 count : " + _lastRoundActionsPlayer1.Count);
                foreach (var action in _lastRoundActionsPlayer1)
                {
                    Debug.Log("Last round action " + action.Item1 + action.Item2[0].name);
                    lastRoundActionNames.Add(action.Item1);
                    EnemyActionsManager.instance.InitEnemyActions(lastRoundActionNames);

                    yield return StartCoroutine(RewindAction(action.Item1, action.Item2, Player.Player2));

                    
                }

            }
            CameraController.instance.SwitchPlayerShipCameraDirectly(Player.Player1);
            _lastRoundActionsPlayer1.Clear();
        }

        UIManager.instance.BackToCombatUI();
        IsInRewind = false;

        if (audioManager.instance != null)
            audioManager.instance.PlaySoundRewindEnd();

        yield return null;
    }

    private Tile GetRewindTile(Player player, Tile targetTile)
    {
        if (player == Player.Player1)
        {
            foreach (Tile rewindTile in GameManager.instance.TilesRewindPlayer1)
            {
                if (rewindTile.name == targetTile.name)
                {
                    Debug.Log("found rewind tile " + rewindTile.name);
                    return rewindTile;
                }
            }
        }
        else
        {
            foreach (Tile rewindTile in GameManager.instance.TilesRewindPlayer2)
            {
                if (rewindTile.name == targetTile.name)
                {
                    Debug.Log("found rewind tile " + rewindTile.name);
                    return rewindTile;
                    
                }
            }
        }

        return null;
    }

    private Tile GetShipTile(Player player, Tile targetTile)
    {
        if (player == Player.Player1)
        {
            foreach (Tile tile in GameManager.instance.TilesPlayer1)
            {
                if (tile.name == targetTile.name)
                {
                    Debug.Log("found ship tile " + tile.name);
                    return tile;
                }
            }
        }
        else
        {
            foreach (Tile tile in GameManager.instance.TilesPlayer2)
            {
                if (tile.name == targetTile.name)
                {
                    Debug.Log("found ship tile " + tile.name);
                    return tile;

                }
            }
        }

        return null;
    }

    private Tile GetRewindShipTile(Player player, Tile targetTile)
    {
        if (player == Player.Player1)
        {
            foreach (Tile tile in GameManager.instance.TilesRewindPlayer1)
            {
                Debug.Log(tile.name);
                if (tile.name == targetTile.name)
                {
                    Debug.Log("found ship tile " + tile.name);
                    return tile;
                }
            }
        }
        else
        {
            foreach (Tile tile in GameManager.instance.TilesRewindPlayer2)
            {
                Debug.Log(tile.name);
                if (tile.name == targetTile.name)
                {
                    Debug.Log("found ship tile " + tile.name);
                    return tile;

                }
            }
        }

        return null;
    }

    IEnumerator RewindAction(string actionName, List<Tile> targets, Player player)
    {
        Debug.Log("rewind action");
        // important -> target sur le ship normal à convertir en target sur le ship rewind
        List<Tile> targetsOnRewind = new List<Tile>();
 
        foreach(Tile target in targets)
        {
            targetsOnRewind.Add(GetRewindTile(player, target));
        }

        _target = targetsOnRewind[0];
        Debug.Log("target on rewind " + _target.name);

        UpdateRoomsRewind();

        switch (actionName)
        {
            case ("Alternate Shot"):
                AlternateShot_Action();
                yield return new WaitForSeconds(1.5f);
                break;
            case ("Simple Hit"):
                SimpleHit_Action();
                yield return new WaitForSeconds(1.5f);
                break;
            case ("Simple Hit X2"):
                SimpleHitX2_Action();
                yield return new WaitForSeconds(1.5f);
                break;
            case ("EMP"):
                EMP_Action();
                yield return new WaitForSeconds(1.5f);
                break;
            case ("Scanner"):
                Scanner_Action();
                yield return new WaitForSeconds(1.5f);
                break;
            case ("Time Accelerator"):
                //UseTimeAccelerator();
                yield return new WaitForSeconds(1.5f);
                break;
            case ("Capacitor"):
                //UseCapacitor();
                yield return new WaitForSeconds(1.5f);
                break;
            case ("Upgrade Shot"):
                UpgradeShot_Action();
                yield return new WaitForSeconds(1.5f);
                break;
            case ("Probe"):
                yield return StartCoroutine(ProbeRewind(targetsOnRewind));
                yield return new WaitForSeconds(1f);
                break;
            case ("Random Reveal"):
                yield return StartCoroutine(RandomRevealRewind(targetsOnRewind));
                yield return new WaitForSeconds(1f);
                break;
            case ("Time Decoy"):
            case ("Repair Decoy"):
            case ("Energy Decoy"):
                Decoy_Action();
                yield return new WaitForSeconds(1.5f);
                break;
            case ("EnergyDecoyDestroy"):
                EnergyDecoyDestroyNewRoom(targetsOnRewind);
                yield return new WaitForSeconds(1.5f);
                break;
            case ("TimeDecoyDestroy"):
                TimeDecoyDestroyNewRoom(targetsOnRewind);
                yield return new WaitForSeconds(1.5f);
                break;
            case ("RepairDecoyDestroy"):
                RepairDecoyDestroyNewRoom(targetsOnRewind);
                yield return new WaitForSeconds(1.5f);
                break;
        }

        
        
    }

    private IEnumerator ProbeRewind(List<Tile> targets)
    {
        foreach (Tile target in targets) // 3 Clicks pour reveal
        {
            _target = target;
            Probe_Action();
            yield return new WaitForSeconds(0.7f);
        }
    }

    private IEnumerator RandomRevealRewind(List<Tile> targets)
    {
        foreach (Tile target in targets) // 5 reveals
        {
            _target = target;
            RandomReveal_Action();
            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion

    #region Update & Reset
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
    #endregion

    #region Select / Deselect
    public void SelectAbilityButton(AbilityButton button)
    {
        Debug.Log("select ability button");
        _selectedButton = button;
        GameManager.instance.SetRoundTargetPos();

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

        UIManager.instance.CheckIfShowEndTurnButton();
        UIManager.instance.CheckAbilityButtonsColor();
        UIManager.instance.HideFicheAbility();
        UIManager.instance.HideFireButton();

        TargetController.instance.HideTarget();
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

        switch (_selectedButton.GetAbility().AbilityName)
        {
            case ("Alternate Shot"):
                AlternateShot_SelectAbilityTiles();
                break;
            case ("Simple Hit"):
                SimpleHit_SelectAbilityTiles();
                break;
            case ("EMP"):
                EMP_SelectAbilityTiles();
                break;
            case ("Scanner"):
                Scanner_SelectAbilityTiles();
                break;
            case ("Upgrade Shot"):
                UpgradeShot_SelectAbilityTiles();
                break;
            case ("Probe"):
                Probe_SelectAbilityTiles();
                break;
            case ("Random Reveal"):
            case ("Time Accelerator"):
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
            tile.IsAbilitySelectedDestroy = false;
            tile.IsAbilitySelectedReveal = false;
        }

        _selectedTiles.Clear();
    }

    private void SelectOnlyTargetTile()
    {
        if (_selectedTiles != null)
        {
            DeselectAbilityTiles();
            _selectedTiles.Clear();
        }

        _target.IsAbilitySelectedDestroy = true;
        _selectedTiles.Add(_target);
    }
    #endregion

    #region Get Ability Buttons or Selected Button
    public List<GameObject> GetAbilityButtonsList()
    {
        return _abilitiesButtonsGameObjects;
    }

    public AbilityButton GetCurrentlySelectedAbilityButton()
    {
        return _selectedButton;
    }
    #endregion

    #region Use Ability
    public void UseSelectedAbility()
    {
        if (CameraController.instance.IsMoving)
            return;

        _lastAbilityUsed = _selectedButton.GetAbility().AbilityName;

        switch (_selectedButton.GetAbility().AbilityName)
        {
            case ("Alternate Shot"):
                UseAlternateShot();
                DeselectAbilityButton(_selectedButton);
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("Simple Hit"):
                UseSimpleHit();
                DeselectAbilityButton(_selectedButton);
                break;
            case ("Random Reveal"):
                UseRandomReveal();
                DeselectAbilityButton(_selectedButton);
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("EMP"):
                UseEMP();
                DeselectAbilityButton(_selectedButton);
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("Scanner"):
                UseScanner();
                DeselectAbilityButton(_selectedButton);
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("Time Accelerator"):
                UseTimeAccelerator();
                DeselectAbilityButton(_selectedButton);
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("Capacitor"):
                UseCapacitor();
                DeselectAbilityButton(_selectedButton);
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("Upgrade Shot"):
                UseUpgradeShot();
                DeselectAbilityButton(_selectedButton);
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("Probe"):
                UseProbe();
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("Energy Decoy"):
                UseDecoy("Energy Decoy");
                DeselectAbilityButton(_selectedButton);
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("Time Decoy"):
                UseDecoy("Time Decoy");
                DeselectAbilityButton(_selectedButton);
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
            case ("Repair Decoy"):
                UseDecoy("Repair Decoy");
                DeselectAbilityButton(_selectedButton); 
                PlayerUsedOtherAbilityThanSimpleHit();
                break;
        }

        TargetController.instance.HideTarget();
    }
    #endregion

    #region Scanner
    // Selection
    private void Scanner_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles scanner");
        if (_selectedTiles != null)
        {
            DeselectAbilityTiles();
            _selectedTiles.Clear();
        }

        // Center
        _target.IsAbilitySelectedReveal = true;
        _selectedTiles.Add(_target);

        #region Top
        // Top
        bool canGoTop = true;
        Tile currentTile = _target;
        while (canGoTop)
        {
            Debug.Log("can go top");
            if (currentTile.TopTile != null)
            {
                currentTile.TopTile.IsAbilitySelectedReveal = true;
                _selectedTiles.Add(currentTile.TopTile);

                currentTile = currentTile.TopTile;
            }
            else
            {
                int row = currentTile.Row;
                int column = currentTile.Column;

                bool foundTop = false;

                if (GameManager.instance.PlayerTurn == Player.Player1)
                {
                    for (int i = 1; (row - i) > 0; i++)
                    {
                        if (GameManager.instance.DictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row - i, column)))
                        {
                            currentTile.TopTile = GameManager.instance.DictTilesRowColumnPlayer2[new Tuple<int, int>(row - i, column)];
                            Debug.Log("current tile " + currentTile.name);
                            currentTile.TopTile.IsAbilitySelectedReveal = true;
                            _selectedTiles.Add(currentTile.TopTile);
                            foundTop = true;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 1; (row - i) > 0; i++)
                    {
                        if (GameManager.instance.DictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row - i, column)))
                        {
                            currentTile.TopTile = GameManager.instance.DictTilesRowColumnPlayer1[new Tuple<int, int>(row - i, column)];
                            foundTop = true;
                            currentTile.TopTile.IsAbilitySelectedReveal = true;
                            _selectedTiles.Add(currentTile.TopTile);
                            Debug.Log("current tile " + currentTile.name);
                            break;
                        }
                    }
                }

                if (!foundTop)
                {
                    canGoTop = false;
                }
            }
        }
        #endregion

        #region Bottom
        // Bottom
        bool canGoBottom = true;
        currentTile = _target;
        while (canGoBottom)
        {
            Debug.Log("can go bottom");
            if (currentTile.BottomTile != null)
            {
                currentTile.BottomTile.IsAbilitySelectedReveal = true;
                _selectedTiles.Add(currentTile.BottomTile);

                currentTile = currentTile.BottomTile;
            }
            else
            {
                int row = currentTile.Row;
                int column = currentTile.Column;

                bool foundBottom = false;

                if (GameManager.instance.PlayerTurn == Player.Player1)
                {
                    for (int i = 1; (row + i) <= 9; i++)
                    {
                        if (GameManager.instance.DictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row + i, column)))
                        {
                            currentTile.BottomTile = GameManager.instance.DictTilesRowColumnPlayer2[new Tuple<int, int>(row + i, column)];
                            Debug.Log("current tile " + currentTile.name);
                            currentTile.BottomTile.IsAbilitySelectedReveal = true;
                            _selectedTiles.Add(currentTile.BottomTile);
                            if (currentTile.BottomTile.BottomTile == null)
                                foundBottom = true;
                            
                        }
                    }
                }
                else
                {
                    for (int i = 1; (row + i) <= 9; i++)
                    {
                        if (GameManager.instance.DictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row + i, column)))
                        {
                            currentTile.BottomTile = GameManager.instance.DictTilesRowColumnPlayer1[new Tuple<int, int>(row + i, column)];
                            Debug.Log("current tile " + currentTile.name);
                            currentTile.BottomTile.IsAbilitySelectedReveal = true;
                            _selectedTiles.Add(currentTile.BottomTile);
                            if (currentTile.BottomTile.BottomTile == null)
                                foundBottom = true;
                            
                        }
                    }
                }

                if (!foundBottom)
                {
                    canGoBottom = false;
                }
            }
        }
        #endregion
    }

    // Use
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

        Scanner_Action();

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

    private void Scanner_Action()
    {
        List<Tile> affectedTiles = new List<Tile>();

        // Center
        RevealRoom(_target);
        affectedTiles.Add(_target);

        Tile currentTile = _target;

        #region Top
        // Top
        bool canGoTop = true;
        
        while (canGoTop)
        {
            Debug.Log("can go top 2");
            if (currentTile.TopTile != null)
            {
                RevealRoom(currentTile.TopTile);
                affectedTiles.Add(currentTile.TopTile);

                currentTile = currentTile.TopTile;
            }
            else
            {
                int row = currentTile.Row;
                int column = currentTile.Column;

                bool foundTop = false;

                if (GameManager.instance.PlayerTurn == Player.Player1)
                {
                    if (IsInRewind)
                    {
                        for (int i = 1; (row - i) > 0; i++)
                        {
                            if (GameManager.instance.RewindDictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row - i, column)))
                            {
                                currentTile.TopTile = GameManager.instance.RewindDictTilesRowColumnPlayer1[new Tuple<int, int>(row - i, column)];
                                Debug.Log("current tile " + currentTile.name);
                                RevealRoom(currentTile.TopTile);
                                foundTop = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; (row - i) > 0; i++)
                        {
                            if (GameManager.instance.DictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row - i, column)))
                            {
                                currentTile.TopTile = GameManager.instance.DictTilesRowColumnPlayer2[new Tuple<int, int>(row - i, column)];
                                Debug.Log("current tile " + currentTile.name);
                                RevealRoom(currentTile.TopTile);
                                foundTop = true;
                                break;
                            }
                        }
                    }

                }
                else
                {
                    if (IsInRewind)
                    {
                        for (int i = 1; (row - i) > 0; i++)
                        {
                            if (GameManager.instance.RewindDictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row - i, column)))
                            {
                                currentTile.TopTile = GameManager.instance.RewindDictTilesRowColumnPlayer2[new Tuple<int, int>(row - i, column)];
                                Debug.Log("current tile " + currentTile.name);
                                RevealRoom(currentTile.TopTile);
                                foundTop = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; (row - i) > 0; i++)
                        {
                            if (GameManager.instance.DictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row - i, column)))
                            {
                                currentTile.TopTile = GameManager.instance.DictTilesRowColumnPlayer1[new Tuple<int, int>(row - i, column)];
                                Debug.Log("current tile " + currentTile.name);
                                RevealRoom(currentTile.TopTile);
                                foundTop = true;
                                break;
                            }
                        }
                    }
                }
                
                if (!foundTop)
                {
                    canGoTop = false;
                }
            }
        }
        #endregion 

        #region Bottom
        // Bottom
        bool canGoBottom = true;
        currentTile = _target;
        while (canGoBottom)
        {
            Debug.Log("can go bottom 2");
            if (currentTile.BottomTile != null)
            {
                RevealRoom(currentTile.BottomTile);
                affectedTiles.Add(currentTile.BottomTile);

                currentTile = currentTile.BottomTile;
            }
            else
            {
                int row = currentTile.Row;
                int column = currentTile.Column;

                bool foundBottom = false;

                if (GameManager.instance.PlayerTurn == Player.Player1)
                {
                    if (IsInRewind)
                    {
                        for (int i = 1; (row + i) <= 9; i++)
                        {
                            if (GameManager.instance.RewindDictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row + i, column)))
                            {
                                currentTile.BottomTile = GameManager.instance.RewindDictTilesRowColumnPlayer1[new Tuple<int, int>(row + i, column)];
                                Debug.Log("current tile " + currentTile.name);
                                RevealRoom(currentTile.BottomTile);
                                foundBottom = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; (row + i) <= 9; i++)
                        {
                            if (GameManager.instance.DictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row + i, column)))
                            {
                                currentTile.BottomTile = GameManager.instance.DictTilesRowColumnPlayer2[new Tuple<int, int>(row + i, column)];
                                Debug.Log("current tile " + currentTile.name);
                                RevealRoom(currentTile.BottomTile);
                                foundBottom = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (IsInRewind)
                    {
                        for (int i = 1; (row + i) <= 9; i++)
                        {
                            if (GameManager.instance.RewindDictTilesRowColumnPlayer2.ContainsKey(new Tuple<int, int>(row + i, column)))
                            {
                                currentTile.BottomTile = GameManager.instance.RewindDictTilesRowColumnPlayer2[new Tuple<int, int>(row + i, column)];
                                Debug.Log("current tile " + currentTile.name);
                                RevealRoom(currentTile.BottomTile);
                                foundBottom = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; (row + i) <= 9; i++)
                        {
                            if (GameManager.instance.DictTilesRowColumnPlayer1.ContainsKey(new Tuple<int, int>(row + i, column)))
                            {
                                currentTile.BottomTile = GameManager.instance.DictTilesRowColumnPlayer1[new Tuple<int, int>(row + i, column)];
                                Debug.Log("current tile " + currentTile.name);
                                RevealRoom(currentTile.BottomTile);
                                foundBottom = true;
                                break;
                            }
                        }
                    }
                }

                if (!foundBottom)
                {
                    canGoBottom = false;
                }
            }
        }
        #endregion

        VFXManager.instance.PlayScannerVFX(affectedTiles);
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
        VFXManager.instance.PlayCapacitorVFX();
        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region EMP
    // Selection
    private void EMP_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles emp");
        if (_selectedTiles != null)
        {
            DeselectAbilityTiles();
            _selectedTiles.Clear();
        }

        // Center
        _target.IsAbilitySelectedDestroy = true;
        _selectedTiles.Add(_target);

        #region Right, Left, Bottom & Top
        // Right
        if (_target.RightTile != null)
        {
            _target.RightTile.IsAbilitySelectedReveal = true;
            _selectedTiles.Add(_target.RightTile);
        }

        // Left
        if (_target.LeftTile != null)
        {
            _target.LeftTile.IsAbilitySelectedReveal = true;
            _selectedTiles.Add(_target.LeftTile);
        }

        // Bottom
        if (_target.BottomTile != null)
        {
            _target.BottomTile.IsAbilitySelectedReveal = true;
            _selectedTiles.Add(_target.BottomTile);
        }

        // Top
        if (_target.TopTile != null)
        {
            _target.TopTile.IsAbilitySelectedReveal = true;
            _selectedTiles.Add(_target.TopTile);
        }
        #endregion

        #region Diag Top Left & Right, Bottom Left & Right
        // Diag top left
        if (_target.DiagTopLeftTile != null)
        {
            _target.DiagTopLeftTile.IsAbilitySelectedReveal = true;
            _selectedTiles.Add(_target.DiagTopLeftTile);
        }

        // Diag top right
        if (_target.DiagTopRightTile != null)
        {
            _target.DiagTopRightTile.IsAbilitySelectedReveal = true;
            _selectedTiles.Add(_target.DiagTopRightTile);
        }

        // Diag bottom left
        if (_target.DiagBottomLeftTile != null)
        {
            _target.DiagBottomLeftTile.IsAbilitySelectedReveal = true;
            _selectedTiles.Add(_target.DiagBottomLeftTile);
        }

        // Diag bottom right
        if (_target.DiagBottomRightTile != null)
        {
            _target.DiagBottomRightTile.IsAbilitySelectedReveal = true;
            _selectedTiles.Add(_target.DiagBottomRightTile);
        }
        #endregion

        
    }

    // Use
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

        EMP_Action();

        UpdateHiddenRooms();
        UIManager.instance.CheckAbilityButtonsColor();
    }

    private void EMP_Action()
    {
        List<Tile> affectedTiles = new List<Tile>();

        DestroyRoom(_target);

        // Desactivate for one turn room's abilities around the target (+1 cooldown)
        List<Room> roomsToDesactivate = new List<Room>();

        #region Right, Left, Bottom & Top
        // Right
        if (_target.RightTile != null)
        {
            affectedTiles.Add(_target.RightTile);
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
            affectedTiles.Add(_target.LeftTile);
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
            affectedTiles.Add(_target.BottomTile);
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
            affectedTiles.Add(_target.TopTile);
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
            affectedTiles.Add(_target.DiagTopLeftTile);
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
            affectedTiles.Add(_target.DiagTopRightTile);
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
            affectedTiles.Add(_target.DiagBottomLeftTile);
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
            affectedTiles.Add(_target.DiagBottomRightTile);
            if (_target.DiagBottomRightTile.Room != null)
            {
                if (!roomsToDesactivate.Contains(_target.DiagBottomRightTile.Room))
                {
                    roomsToDesactivate.Add(_target.DiagBottomRightTile.Room);
                }
            }
        }
        #endregion

        VFXManager.instance.PlayEMPVFX(_target, affectedTiles);

        foreach (Room room in roomsToDesactivate)
        {
            if (room.RoomData.RoomAbility != null)
            {
                Debug.Log("cooldown " + GameManager.instance.GetCurrentCooldown(room.RoomData.RoomAbility));
                if (GameManager.instance.GetEnemyCurrentCooldown(room.RoomData.RoomAbility) == 0)
                {
                    GameManager.instance.AddEnemyAbilityOneCooldownNextTurn(room.RoomData.RoomAbility);
                    Debug.Log("add 2 cooldown to " + room.RoomData.RoomAbility.name);
                }
                else if (GameManager.instance.GetEnemyCurrentCooldown(room.RoomData.RoomAbility) == 1)
                {
                    GameManager.instance.AddEnemyAbilityOneCooldown(room.RoomData.RoomAbility);
                    Debug.Log("add 1 cooldown to " + room.RoomData.RoomAbility.name);
                }
                
            }
        }

    }
    #endregion

    #region Simple Hit
    // Selection
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

            _target.IsAbilitySelectedDestroy = true;
            _selectedTiles.Add(_target);

            // right
            if (_target.RightTile != null)
            {
                _target.RightTile.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target.RightTile);
            }

            // bottom
            if (_target.BottomTile != null)
            {
                _target.BottomTile.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target.BottomTile);
            }

            // diag bottom right
            if (_target.DiagBottomRightTile != null)
            {
                _target.DiagBottomRightTile.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target.DiagBottomRightTile);
            }
        }
        else
        {
            SelectOnlyTargetTile();
        }
    }

    // Use
    private void UseSimpleHit()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        if (GetIfSimpleHitXS())
        {
            _lastAbilityUsed = "Simple Hit X2";
            Debug.Log("simple hit x2");
            AddActionToCurrentPlayerRound("Simple Hit X2");

            SimpleHitX2_Action();

            DesactivateSimpleHitX2IfActivated();
        }
        else
        {
            AddActionToCurrentPlayerRound("Simple Hit");
            SimpleHit_Action();
        }

        UpdateHiddenRooms(); 
        UIManager.instance.CheckAbilityButtonsColor();
    }

    private void SimpleHit_Action()
    {
        Debug.Log("simple hit action");
        DestroyRoom(_target);

        VFXManager.instance.PlaySimpleHitVFX(_target);
    }

    private void SimpleHitX2_Action()
    {
        List<Tile> affectedTiles = new List<Tile>();

        Debug.Log("simple hit x 2 action");
        DestroyRoom(_target);
        affectedTiles.Add(_target);

        // Try destroy right
        if (_target.RightTile != null)
        {
            DestroyRoom(_target.RightTile);
            affectedTiles.Add(_target.RightTile);
        }

        // Try destroy bottom
        if (_target.BottomTile != null)
        {
            DestroyRoom(_target.BottomTile);
            affectedTiles.Add(_target.BottomTile);
        }

        // Try destroy diag bottom right
        if (_target.DiagBottomRightTile != null)
        {
            DestroyRoom(_target.DiagBottomRightTile);
            affectedTiles.Add(_target.DiagBottomRightTile);
        }

        VFXManager.instance.PlaySimpleHitX2VFX(affectedTiles);
    }

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

    #region Random Reveal
    // Selection
    private void RandomReveal_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles random reveal");
        
    }

    // Use
    private void UseRandomReveal()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        // Get Player Tiles
        List<Tile> playerTiles = new List<Tile>();
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            playerTiles = GameManager.instance.TilesPlayer2;
        }
        else
        {
            playerTiles = GameManager.instance.TilesPlayer1;
        }

        // Get random reveal tiles
        while(_currentActionTargetTiles.Count < 5)
        {
            Debug.Log("while 1");
            int randomIndex = Random.Range(0, playerTiles.Count);
            Tile randomTile = playerTiles[randomIndex];

            if (!_currentActionTargetTiles.Contains(randomTile) && !randomTile.IsDestroyed && !randomTile.IsReavealed && !randomTile.IsOccupied)
            {
                _currentActionTargetTiles.Add(randomTile);
                _target = randomTile;
                RandomReveal_Action();
            } 
        }

        AddActionToCurrentPlayerRound("Random Reveal");

        UpdateHiddenRooms(); // si destroy ou reveal 
        UIManager.instance.CheckAbilityButtonsColor();
    }

    private void RandomReveal_Action()
    {
        RevealRoom(_target);

        VFXManager.instance.PlayRandomProbeVFX(_target);
    }
    #endregion

    #region Alternate Shot
    // Selection
    private void AlternateShot_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles alternate shot");
        if (_selectedTiles != null)
        {
            DeselectAbilityTiles();
            _selectedTiles.Clear();
        }

        _currentAlternateShotDirection = GetCurrentPlayerAlternateShotDirection();

        _target.IsAbilitySelectedDestroy = true;
        _selectedTiles.Add(_target);

        if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
        {
            if (_target.LeftTile != null)
            {
                _target.LeftTile.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target.LeftTile);
            }

            if (_target.RightTile != null)
            {
                _target.RightTile.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target.RightTile);
            }
        }
        else
        {
            if (_target.TopTile != null)
            {
                _target.TopTile.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target.TopTile);
            }

            if (_target.BottomTile != null)
            {
                _target.BottomTile.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target.BottomTile);
            }
        }
    }

    // Use
    private void UseAlternateShot()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        Debug.Log(_target.name);


        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("Alternate Shot");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        _currentAlternateShotDirection = GetCurrentPlayerAlternateShotDirection();

        AlternateShot_Action();

        ChangeAlternateShotDirection();

        UpdateHiddenRooms(); // si destroy ou reveal 
        UIManager.instance.CheckAbilityButtonsColor();
    }

    private void AlternateShot_Action()
    {
        List<Tile> affectedTiles = new List<Tile>();

        DestroyRoom(_target);
        affectedTiles.Add(_target);

        if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
        {
            if (_target.LeftTile != null)
            {
                DestroyRoom(_target.LeftTile);
                affectedTiles.Add(_target.LeftTile);
            }

            if (_target.RightTile != null)
            {
                DestroyRoom(_target.RightTile);
                affectedTiles.Add(_target.RightTile);
            }
        }
        else
        {
            if (_target.TopTile != null)
            {
                DestroyRoom(_target.TopTile);
                affectedTiles.Add(_target.TopTile);
            }

            if (_target.BottomTile != null)
            {
                DestroyRoom(_target.BottomTile);
                affectedTiles.Add(_target.BottomTile);
            }
        }

        VFXManager.instance.PlayAlternateShotVFX(affectedTiles);
    }

    private void AddActionToCurrentPlayerRound(string actionName)
    {
        Debug.Log("add action " + actionName + "target 0 " + _currentActionTargetTiles[0]);

        // BUG : J'UTILISAIS _currentActionTargetTiles DANS LA CREATION DES TUPLES MAIS CA FAIT UNE REF DU COUP NORMAL QUE TOUS
        // AIENT LA MEME POS QUAND Y'EN A PLUSIEURS PTN FAUT UNE COPIE
        List<Tile> targetTilesCopy = new List<Tile>(_currentActionTargetTiles);

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            Debug.Log("add action to player 1 " + actionName);
            _lastRoundActionsPlayer1.Add(Tuple.Create(actionName, targetTilesCopy));
        }
        else
        {
            Debug.Log("add action to player 2 " + actionName);
            _lastRoundActionsPlayer2.Add(Tuple.Create(actionName, targetTilesCopy));
        }
    }

    private void AddActionToCurrentPlayerRoundWithTempTargets(string actionName)
    {
        Debug.Log("action " + actionName);
        List<Tile> targetTilesCopy = new List<Tile>(tempTargets);

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            Debug.Log("add action to player 1 " + actionName);
            _lastRoundActionsPlayer1.Add(Tuple.Create(actionName, targetTilesCopy));
        }
        else
        {
            Debug.Log("add action to player 2 " + actionName);
            _lastRoundActionsPlayer2.Add(Tuple.Create(actionName, targetTilesCopy));
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

    public AlternateShotDirection GetRewindPlayerAlternateShotDirection()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            if (_currentAlternateShotDirectionPlayer2 == AlternateShotDirection.Horizontal)
                return AlternateShotDirection.Vertical;
            else
                return AlternateShotDirection.Horizontal;
        }
        else
        {
            if (_currentAlternateShotDirectionPlayer1 == AlternateShotDirection.Horizontal)
                return AlternateShotDirection.Vertical;
            else
                return AlternateShotDirection.Horizontal;
        }
    }

    private void ChangeAlternateShotDirection()
    {
        AlternateShotDirection _tempAlternateShotDirection = GetCurrentPlayerAlternateShotDirection();

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            if (_tempAlternateShotDirection == AlternateShotDirection.Horizontal)
                _tempAlternateShotDirection = AlternateShotDirection.Vertical;
            else
                _tempAlternateShotDirection = AlternateShotDirection.Horizontal;

            _currentAlternateShotDirectionPlayer1 = _tempAlternateShotDirection;
        }
        else
        {
            if (_tempAlternateShotDirection == AlternateShotDirection.Horizontal)
                _tempAlternateShotDirection = AlternateShotDirection.Vertical;
            else
                _tempAlternateShotDirection = AlternateShotDirection.Horizontal;

            _currentAlternateShotDirectionPlayer2 = _tempAlternateShotDirection;
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
        AddActionToCurrentPlayerRound("Time Accelerator");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);
        VFXManager.instance.PlayTimeAcceleratorVFX();

        DesactivateSimpleHitX2IfActivated();

        // All player cooldown - 1
        GameManager.instance.CurrentPlayerLessCooldown(1);

        UIManager.instance.CheckAbilityButtonsColor();
    }
    #endregion

    #region Upgrade Shot
    // Selection
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
                _target.IsAbilitySelectedReveal = true;
                _selectedTiles.Add(_target);
                break;
            case (UpgradeShotStep.DestroyOneTile):
                _target.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target);
                break;
            case (UpgradeShotStep.DestroyThreeTilesInDiagonal):

                if (_target.DiagTopLeftTile != null)
                {
                    _target.DiagTopLeftTile.IsAbilitySelectedDestroy = true;
                    _selectedTiles.Add(_target.DiagTopLeftTile);
                }

                if (_target.DiagBottomRightTile != null)
                {
                    _target.DiagBottomRightTile.IsAbilitySelectedDestroy = true;
                    _selectedTiles.Add(_target.DiagBottomRightTile);
                }

                _target.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target);
                break;
            case (UpgradeShotStep.DestroyFiveTilesInCross):

                if (_target.LeftTile != null)
                {
                    _target.LeftTile.IsAbilitySelectedDestroy = true;
                    _selectedTiles.Add(_target.LeftTile);
                }

                if (_target.RightTile != null)
                {
                    _target.RightTile.IsAbilitySelectedDestroy = true;
                    _selectedTiles.Add(_target.RightTile);
                }

                if (_target.TopTile != null)
                {
                    _target.TopTile.IsAbilitySelectedDestroy = true;
                    _selectedTiles.Add(_target.TopTile);
                }

                if (_target.BottomTile != null)
                {
                    _target.BottomTile.IsAbilitySelectedDestroy = true;
                    _selectedTiles.Add(_target.BottomTile);
                }

                _target.IsAbilitySelectedDestroy = true;
                _selectedTiles.Add(_target);
                break;
        }
    }

    public UpgradeShotStep GetRewindPlayerUpgradeShotStep()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            switch (_currentUpgradeShotStepPlayer2)
            {
                case (UpgradeShotStep.RevealOneTile):
                    return UpgradeShotStep.DestroyFiveTilesInCross;
                case (UpgradeShotStep.DestroyOneTile):
                    return UpgradeShotStep.RevealOneTile;
                case (UpgradeShotStep.DestroyThreeTilesInDiagonal):
                    return UpgradeShotStep.DestroyOneTile;
                case (UpgradeShotStep.DestroyFiveTilesInCross):
                    return UpgradeShotStep.DestroyThreeTilesInDiagonal;
            }
        }
        else
        {
            switch (_currentUpgradeShotStepPlayer1)
            {
                case (UpgradeShotStep.RevealOneTile):
                    return UpgradeShotStep.DestroyFiveTilesInCross;
                case (UpgradeShotStep.DestroyOneTile):
                    return UpgradeShotStep.RevealOneTile;
                case (UpgradeShotStep.DestroyThreeTilesInDiagonal):
                    return UpgradeShotStep.DestroyOneTile;
                case (UpgradeShotStep.DestroyFiveTilesInCross):
                    return UpgradeShotStep.DestroyThreeTilesInDiagonal;
            }
        }

        return UpgradeShotStep.RevealOneTile;
    }

    // Use
    private void UseUpgradeShot()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        AddActionToCurrentPlayerRound("Upgrade Shot");
        // ----- REWIND ----- //

        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        DesactivateSimpleHitX2IfActivated();

        _currentUpgradeShotStep = GetCurrentPlayerUpgradeShotStep();

        UpgradeShot_Action();

        ChangeUpgradeShotStep();

        UpdateHiddenRooms(); 
        UIManager.instance.CheckAbilityButtonsColor();
    }

    private void UpgradeShot_Action()
    {
        List<Tile> affectedTiles = new List<Tile>();

        switch (_currentUpgradeShotStep)
        {
            case (UpgradeShotStep.RevealOneTile):
                RevealRoom(_target);
                affectedTiles.Add(_target);
                break;
            case (UpgradeShotStep.DestroyOneTile):
                DestroyRoom(_target);
                affectedTiles.Add(_target);
                break;
            case (UpgradeShotStep.DestroyThreeTilesInDiagonal):

                if (_target.DiagTopLeftTile != null)
                {
                    DestroyRoom(_target.DiagTopLeftTile);
                    affectedTiles.Add(_target.DiagTopLeftTile);
                }

                if (_target.DiagBottomRightTile != null)
                {
                    DestroyRoom(_target.DiagBottomRightTile);
                    affectedTiles.Add(_target.DiagBottomRightTile);
                }

                DestroyRoom(_target);
                affectedTiles.Add(_target);
                break;
            case (UpgradeShotStep.DestroyFiveTilesInCross):

                if (_target.LeftTile != null)
                {
                    DestroyRoom(_target.LeftTile);
                    affectedTiles.Add(_target.LeftTile);
                }

                if (_target.RightTile != null)
                {
                    DestroyRoom(_target.RightTile);
                    affectedTiles.Add(_target.RightTile);
                }

                if (_target.TopTile != null)
                {
                    DestroyRoom(_target.TopTile);
                    affectedTiles.Add(_target.TopTile);
                }

                if (_target.BottomTile != null)
                {
                    DestroyRoom(_target.BottomTile);
                    affectedTiles.Add(_target.BottomTile);
                }

                DestroyRoom(_target);
                affectedTiles.Add(_target);
                break;
        }

        VFXManager.instance.PlayUpgradeShotVFX(affectedTiles);
    }

    public UpgradeShotStep GetCurrentPlayerUpgradeShotStep()
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
        UpgradeShotStep _tempUpgradeShotStep = GetCurrentPlayerUpgradeShotStep();

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            if (_tempUpgradeShotStep == UpgradeShotStep.RevealOneTile)
            {
                _tempUpgradeShotStep = UpgradeShotStep.DestroyOneTile;
            }
            else if (_tempUpgradeShotStep == UpgradeShotStep.DestroyOneTile)
            {
                _tempUpgradeShotStep = UpgradeShotStep.DestroyThreeTilesInDiagonal;
            }
            else if (_tempUpgradeShotStep == UpgradeShotStep.DestroyThreeTilesInDiagonal)
            {
                _tempUpgradeShotStep = UpgradeShotStep.DestroyFiveTilesInCross;
            }
            // Else reste en destroy five tiles in cross

            _currentUpgradeShotStepPlayer1 = _tempUpgradeShotStep;
        }
        else
        {
            if (_tempUpgradeShotStep == UpgradeShotStep.RevealOneTile)
            {
                _tempUpgradeShotStep = UpgradeShotStep.DestroyOneTile;
            }
            else if (_tempUpgradeShotStep == UpgradeShotStep.DestroyOneTile)
            {
                _tempUpgradeShotStep = UpgradeShotStep.DestroyThreeTilesInDiagonal;
            }
            else if (_tempUpgradeShotStep == UpgradeShotStep.DestroyThreeTilesInDiagonal)
            {
                _tempUpgradeShotStep = UpgradeShotStep.DestroyFiveTilesInCross;
            }
            // Else reste en destroy five tiles in cross

            _currentUpgradeShotStepPlayer2 = _tempUpgradeShotStep;
        }

        if (_currentUpgradeShotStepPlayer1 == UpgradeShotStep.DestroyFiveTilesInCross || _currentUpgradeShotStepPlayer2 == UpgradeShotStep.DestroyFiveTilesInCross)
        {
            Debug.Log("Achievement IMMA FIRE MA LASER");
            if (GoogleSignInCheck.instance.getPlayerStatus() == true)
            {
                Social.ReportProgress("CgkIusy6mqADEAIQBA", 100.0f, (bool success) => {
                    if (success)
                    {
                        Debug.Log("Progress reported successfully!");
                    }
                    else
                    {
                        Debug.LogWarning("Error failed to report progress!");
                    }
                });
            }
        }

        UIManager.instance.CheckUpgradeShotLvlImg();
    }
    #endregion

    #region Probe
    // Select
    private void Probe_SelectAbilityTiles()
    {
        Debug.Log("select ability tiles probe");
        SelectOnlyTargetTile();
    }

    // Use
    private void UseProbe()
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0 && _currentProbeCount == 0)
            _currentActionTargetTiles.Clear();
        _currentActionTargetTiles.Add(_target);
        if (_currentProbeCount == 2) // ++ -> 3
            AddActionToCurrentPlayerRound("Probe");
        // ----- REWIND ----- //

        _currentProbeCount++;
        UIManager.instance.ShowProbeCount(_currentProbeCount);

        Probe_Action();

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
    
    private void Probe_Action()
    {
        RevealRoom(_target);

        VFXManager.instance.PlayProbeVFX(_target);
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

    #region Decoy
    private void CreateNewRandomDecoy(string decoyName)
    {
        // Create another decoy to random player ship position (and update to rewind)
        // Player ship
        // Get Player Tiles
        List<Tile> playerTiles = new List<Tile>();
        playerTiles = GetPlayerTiles();

        bool roomBuilt = false;
        while (!roomBuilt)
        {
            //Debug.Log("while 3" + _destroyTarget.name + _destroyTarget.Room.name);
            Tile randomTile = playerTiles[Random.Range(0, playerTiles.Count - 1)];
            if (GameManager.instance.CheckCanBuild(GetRoomFromTargetWithAbility(decoyName), randomTile) && !randomTile.IsDestroyed && !randomTile.IsReavealed && !randomTile.IsMissed && !randomTile.IsOccupied) // target on decoy room after destroy
            {
                // Player Ship
                Debug.Log("create room ship " + randomTile + " " + decoyName);
                //GameManager.instance.CreateNewBuilding(GetRoomFromTargetWithAbility(decoyName), randomTile, GameManager.instance.PlayerTurn);

                if (GameManager.instance.PlayerTurn == Player.Player1)
                    GameManager.instance.CreateNewBuilding(GetRoomFromTargetWithAbility(decoyName), randomTile,  Player.Player2);
                else
                    GameManager.instance.CreateNewBuilding(GetRoomFromTargetWithAbility(decoyName), randomTile, Player.Player1);

                RoomsAssetsManager.instance.SetTileRoomAsset(GetRoomFromTargetWithAbility(decoyName).RoomData.RoomAbility, randomTile.RoomTileSpriteRenderer, false, false);
                tempTargets.Add(randomTile);

                roomBuilt = true;

                //VFXManager.instance.PlayDecoyCreationVFX(randomTile);
            }
        }
    }

    private void UseDecoy(string decoyName)
    {
        // ----- REWIND ----- //
        if (_currentActionTargetTiles.Count > 0)
            _currentActionTargetTiles.Clear();
        // ----- REWIND ----- //

        DesactivateSimpleHitX2IfActivated();
        _selectedButton.SetCooldown();
        ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);

        // Get Player Tiles
        List<Tile> playerTiles = new List<Tile>();
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            playerTiles = GameManager.instance.TilesPlayer2;
        }
        else
        {
            playerTiles = GameManager.instance.TilesPlayer1;
        }

        // Get random reveal tiles
        while (_currentActionTargetTiles.Count <= 0)
        {
            Debug.Log("while 2");
            int randomIndex = Random.Range(0, playerTiles.Count);
            Tile randomTile = playerTiles[randomIndex];

            if (!_currentActionTargetTiles.Contains(randomTile) && !randomTile.IsDestroyed && !randomTile.IsReavealed && randomTile.IsOccupied)
            {
                _currentActionTargetTiles.Add(randomTile);
                _target = randomTile;
                Debug.Log("target : " + _target.name);
                Decoy_Action();
            }
        }

        AddActionToCurrentPlayerRound(decoyName);

        UpdateHiddenRooms();
        UIManager.instance.CheckAbilityButtonsColor();
    }

    private void Decoy_Action()
    {
        Debug.Log("energy decoy action");
        RevealRoom(_target);

        VFXManager.instance.PlayUseDecoyVFX(_target);
    }
    #endregion

    #region Energy Decoy
    private void EnergyDecoyDestroyNewRoom(List<Tile> targets)
    {        
        Debug.Log("new room energy decoy " + _target.name);
        foreach(Tile target in targets)
        {
            if (target.Room != null)
            {
                if (target.Room.RoomData.RoomName == "Energy Decoy")
                {
                    _target = target;
                }
            }
        }
        Tile lastTarget = GetShipTile(GameManager.instance.PlayerTurn, _target);

        // Create new decoy rewind 
        GameManager.instance.CreateNewBuildingRewind(lastTarget.Room, _target, GameManager.instance.PlayerTurn);
        RoomsAssetsManager.instance.SetTileRoomAsset(lastTarget.Room.RoomData.RoomAbility, _target.RoomTileSpriteRenderer, false, false);

        VFXManager.instance.PlayEnergyDecoyCreationVFX(_target);
    }
    
    private void UseEnergyDecoyAfterDestroy()
    {
        // ----- REWIND ----- //
        if (tempTargets.Count > 0)
            tempTargets.Clear();
        // ----- REWIND ----- //
        Debug.Log("----- use energy decoy after destroy");

        UseEnergyDecoyAfterDestroy_Action();

        AddActionToCurrentPlayerRoundWithTempTargets("EnergyDecoyDestroy");
    }

    private void UseEnergyDecoyAfterDestroy_Action()
    {
        Debug.Log("----- use energy decoy after destroy");

        // +1 Action point next round
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            GameManager.instance.EnergyDecoyTriggeredPlayer2 = true;
        }
        else
        {
            GameManager.instance.EnergyDecoyTriggeredPlayer1 = true;
        }

        CreateNewRandomDecoy("Energy Decoy");
    }
    #endregion

    #region Time Decoy
    private void TimeDecoyDestroyNewRoom(List<Tile> targets)
    {
        Debug.Log("new room time decoy " + _target.name);
        foreach (Tile target in targets)
        {
            if (target.Room != null)
            {
                if (target.Room.RoomData.RoomName == "Energy Decoy")
                {
                    _target = target;
                }
            }
        }

        Tile lastTarget = GetShipTile(GameManager.instance.PlayerTurn, _target);

        // Create new decoy rewind 
        GameManager.instance.CreateNewBuildingRewind(lastTarget.Room, _target, GameManager.instance.PlayerTurn);
        RoomsAssetsManager.instance.SetTileRoomAsset(lastTarget.Room.RoomData.RoomAbility, _target.RoomTileSpriteRenderer, false, false);

        VFXManager.instance.PlayTimeDecoyCreationVFX(_target);
    }

    private void UseTimeDecoyAfterDestroy()
    {
        // ----- REWIND ----- //
        if (tempTargets.Count > 0)
            tempTargets.Clear();
        // ----- REWIND ----- //
        Debug.Log("----- use time decoy after destroy");

        UseTimeDecoyAfterDestroy_Action();

        AddActionToCurrentPlayerRoundWithTempTargets("TimeDecoyDestroy");
    }


    private void UseTimeDecoyAfterDestroy_Action()
    {
        Debug.Log("----- use time decoy after destroy");

        GameManager.instance.AddCurrentPlayerAbilityOneCooldown(GameManager.instance.GetAbilityFromName(_lastAbilityUsed));

        CreateNewRandomDecoy("Time Decoy");
    }
    #endregion

    #region Repair Decoy
    private void RepairDecoyDestroyNewRoom(List<Tile> targets)
    {
        foreach (Tile target in targets)
        {
            if (target.Room != null)
            {
                if (target.Room.RoomData.RoomName == "Energy Decoy")
                {
                    _target = target;
                }
            }
        }

        // Repair room
        if (_tempTileRepaired != null)
        {
            Tile rewindTileToRepair = GetRewindShipTile(GameManager.instance.PlayerTurn, _tempTileRepaired);
            if (rewindTileToRepair.Room.IsRoomDestroyed)
                rewindTileToRepair.Room.IsRoomDestroyed = false;
            rewindTileToRepair.IsDestroyed = false;
            Debug.Log("rewind repair " + rewindTileToRepair.name + rewindTileToRepair.Room.name);
            RoomsAssetsManager.instance.SetTileRoomAsset(rewindTileToRepair.Room.RoomData.RoomAbility, rewindTileToRepair.RoomTileSpriteRenderer, false, false);

            VFXManager.instance.PlayRepairDecoyVFX(rewindTileToRepair);
        }


        Debug.Log("new room repair decoy " + _target.name);
        Tile lastTarget = GetShipTile(GameManager.instance.PlayerTurn, _target);

        // Create new decoy rewind 
        GameManager.instance.CreateNewBuildingRewind(lastTarget.Room, _target, GameManager.instance.PlayerTurn);
        RoomsAssetsManager.instance.SetTileRoomAsset(lastTarget.Room.RoomData.RoomAbility, _target.RoomTileSpriteRenderer, false, false);

        VFXManager.instance.PlayRepairDecoyCreationVFX(_target);
    }

    private void UseRepairDecoyAfterDestroy()
    {
        // ----- REWIND ----- //
        if (tempTargets.Count > 0)
            tempTargets.Clear();
        // ----- REWIND ----- //
        Debug.Log("----- use repair decoy after destroy");

        UseRepairDecoyAfterDestroy_Action();

        AddActionToCurrentPlayerRoundWithTempTargets("RepairDecoyDestroy");
    }


    private void UseRepairDecoyAfterDestroy_Action()
    {
        Debug.Log("----- use time decoy after destroy");

        // ----- REPAIR RANDOM ROOM DESTROYED ----- //
        List<Tile> playerTiles = new List<Tile>();
        playerTiles = GetPlayerTiles();
        List<Tile> occupiedTiles = new List<Tile>();

        // check foreach tile si possible d'en trouver une destroyed -> sinon on fait pas le random
        foreach(Tile tile in playerTiles)
        {
            if (tile.IsOccupied && tile.Room != null && tile.IsDestroyed)
                if (tile.Room.RoomData.RoomName != "Repair Decoy" && tile.Room.RoomData.RoomName != "Energy Decoy" && tile.Room.RoomData.RoomName != "Time Decoy")
                    occupiedTiles.Add(tile);
        }

        // prendre une tile random -> si room dessus destroy, plus destroy (+ check si pas room completely destroyed et mettre à false), update sprites ?
        if (occupiedTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, occupiedTiles.Count - 1);
            Tile tileToRepair = occupiedTiles[randomIndex];
            Room roomToRepair = tileToRepair.Room;
            Debug.Log("repair tile " + tileToRepair.name + ", room " + roomToRepair.name);

            if (roomToRepair.IsRoomDestroyed)
                roomToRepair.IsRoomDestroyed = false;
            // a update si on met les assets rooms completely destroyed

            tileToRepair.IsDestroyed = false;
            RoomsAssetsManager.instance.SetTileRoomAsset(roomToRepair.RoomData.RoomAbility, tileToRepair.RoomTileSpriteRenderer, false, false);
            _tempTileRepaired = tileToRepair;
        }
        else
        {
            _tempTileRepaired = null;
            Debug.Log("no room to repair");
        }
        // sinon pas de réparation à faire

        // ----- REPAIR RANDOM ROOM DESTROYED ----- //

        CreateNewRandomDecoy("Repair Decoy");
    }

    
    #endregion

    #region Update Rooms
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
    #endregion

    #region Destroy / Reveal Rooms
    private void DestroyRoom(Tile tile)
    {
        Debug.Log("destroy room " + tile.name);
        if (tile.IsOccupied)
        {
            if (!tile.IsDestroyed)
            {
                Debug.Log("hit room " + tile.Room.name);
                RoomsAssetsManager.instance.SetTileRoomAsset(tile.Room.RoomData.RoomAbility, tile.RoomTileSpriteRenderer, true, false);
                tile.IsDestroyed = true;

                GameManager.instance.CheckIfTileRoomIsCompletelyDestroyed(tile);
                UIManager.instance.ShowFicheRoom(tile.Room.RoomData);

                if (!IsInRewind)
                    CheckDecoysAfterDestoy(tile);
            }
        }
        else
        {
            tile.IsMissed = true;
            tile.IsMissedDestroyed = true;
            Debug.Log("no room on hit " + tile.name);

            UIManager.instance.HideFicheRoom();
        }

        UIManager.instance.UpdateEnemyLife();
    }

    private void RevealRoom(Tile tile)
    {
        Debug.Log("reveal room " + tile.name);
        if (tile.IsOccupied)
        {
            if (!tile.IsDestroyed)
            {
                Debug.Log("hit room " + tile.Room.name);
                RoomsAssetsManager.instance.SetTileRoomAsset(tile.Room.RoomData.RoomAbility, tile.RoomTileSpriteRenderer, false, true);
                tile.IsReavealed = true;

                GameManager.instance.CheckIfTileRoomIsCompletelyDestroyed(tile);

                UIManager.instance.ShowFicheRoom(tile.Room.RoomData);
            }
        }
        else
        {
            tile.IsMissed = true;
            tile.IsMissedReavealed = true;
            Debug.Log("no room on hit " + tile.name);

            UIManager.instance.HideFicheRoom();
        }
    }
    #endregion

    #region Check Decoys
    private void CheckDecoysAfterDestoy(Tile tileDestroyed)
    {
        Debug.Log("CHECK DECOYS AFTER DESTROY");

        if (tileDestroyed.Room != null)
        {
            Debug.Log(tileDestroyed.Room.RoomData.RoomName);
            switch (tileDestroyed.Room.RoomData.RoomName)
            {
                case ("Energy Decoy"):
                    UseEnergyDecoyAfterDestroy();
                    break;
                case ("Time Decoy"):
                    UseTimeDecoyAfterDestroy();
                    break;
                case ("Repair Decoy"):
                    UseRepairDecoyAfterDestroy();
                    break;
            }
        }
    }
    #endregion

    private Room GetRoomFromTargetWithAbility(string decoyName)
    {
        switch (_lastAbilityUsed)
        {
            case ("Simple Hit"):
            case ("EMP"):
                Debug.Log("last ability used simple hit or emp");
                return _target.Room;
            case ("Alternate Shot"):
                Debug.Log("last ability used alternate shot");
                if (_target != null)
                    if (_target.Room != null)
                        if (_target.Room.RoomData.RoomName == decoyName)
                            return _target.Room;
                if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
                {
                    if (_target.LeftTile != null)
                        if (_target.LeftTile.Room != null)
                            if (_target.LeftTile.Room.RoomData.RoomName == decoyName)
                                return _target.LeftTile.Room;

                    if (_target.RightTile != null)
                        if (_target.RightTile.Room != null)
                            if (_target.RightTile.Room.RoomData.RoomName == decoyName)
                                return _target.RightTile.Room;
                }
                else
                {
                    if (_target.TopTile != null)
                        if (_target.TopTile.Room != null)
                            if (_target.TopTile.Room.RoomData.RoomName == decoyName)
                                return _target.TopTile.Room;

                    if (_target.BottomTile != null)
                        if (_target.BottomTile.Room != null)
                            if (_target.BottomTile.Room.RoomData.RoomName == decoyName)
                                return _target.BottomTile.Room;
                }
                break;
            case ("Upgrade Shot"):
                Debug.Log("last ability used upgrade shot");
                switch (_currentUpgradeShotStep)
                {
                    case (UpgradeShotStep.DestroyOneTile):
                        if (_target != null)
                            if (_target.Room != null)
                                if (_target.Room.RoomData.RoomName == decoyName)
                                    return _target.Room;
                        break;
                    case (UpgradeShotStep.DestroyThreeTilesInDiagonal):
                        if (_target != null)
                            if (_target.Room != null)
                                if (_target.Room.RoomData.RoomName == decoyName)
                                    return _target.Room;

                        if (_target.DiagTopLeftTile != null)
                            if (_target.DiagTopLeftTile.Room != null)
                                if (_target.DiagTopLeftTile.Room.RoomData.RoomName == decoyName)
                                    return _target.DiagTopLeftTile.Room;

                        if (_target.DiagBottomRightTile != null)
                            if (_target.DiagBottomRightTile.Room != null)
                                if (_target.DiagBottomRightTile.Room.RoomData.RoomName == decoyName)
                                    return _target.DiagBottomRightTile.Room;
                        break;
                    case (UpgradeShotStep.DestroyFiveTilesInCross):
                        if (_target != null)
                            if (_target.Room != null)
                                if (_target.Room.RoomData.RoomName == decoyName)
                                    return _target.Room;

                        if (_target.LeftTile != null)
                            if (_target.LeftTile.Room != null)
                                if (_target.LeftTile.Room.RoomData.RoomName == decoyName)
                                    return _target.LeftTile.Room;

                        if (_target.RightTile != null)
                            if (_target.RightTile.Room != null)
                                if (_target.RightTile.Room.RoomData.RoomName == decoyName)
                                    return _target.RightTile.Room;

                        if (_target.TopTile != null)
                            if (_target.TopTile.Room != null)
                                if (_target.TopTile.Room.RoomData.RoomName == decoyName)
                                    return _target.TopTile.Room;

                        if (_target.BottomTile != null)
                            if (_target.BottomTile.Room != null)
                                if (_target.BottomTile.Room.RoomData.RoomName == decoyName)
                                    return _target.BottomTile.Room;
                        break;
                }
                break;
            case ("Simple Hit X2"):
                Debug.Log("last ability used simple hit x2");
                if (_target != null)
                    if (_target.Room != null)
                        if (_target.Room.RoomData.RoomName == decoyName)
                            return _target.Room;
                if (_target.LeftTile != null)
                    if (_target.LeftTile.Room != null)
                        if (_target.LeftTile.Room.RoomData.RoomName == decoyName)
                            return _target.LeftTile.Room;
                if (_target.BottomTile != null)
                    if (_target.BottomTile.Room != null)
                        if (_target.BottomTile.Room.RoomData.RoomName == decoyName)
                            return _target.BottomTile.Room;
                if (_target.DiagBottomRightTile != null)
                    if (_target.DiagBottomRightTile.Room != null)
                        if (_target.DiagBottomRightTile.Room.RoomData.RoomName == decoyName)
                            return _target.DiagBottomRightTile.Room;
                break;
        }

        return null;
    }

    private List<Tile> GetPlayerTiles()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
            return  GameManager.instance.TilesPlayer2;
        else
            return GameManager.instance.TilesPlayer1;
    }

    private void PlayerUsedOtherAbilityThanSimpleHit()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
            GameManager.instance.Player1OnlyUsingSimpleHit = false;
        else
            GameManager.instance.Player2OnlyUsingSimpleHit = false;
    }
}
