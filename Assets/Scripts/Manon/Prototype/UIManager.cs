using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static UIManager instance;

    [Header("Canvas")]
    [SerializeField] GameObject _gameCanvas;

    [Header("Top & Bottom Game UI")]
    [SerializeField] GameObject _enemyTop;
    [SerializeField] GameObject _playerBottom;

    [Header("Debug Text")]
    [SerializeField] TMP_Text _currentPlayerTxt;
    [SerializeField] TMP_Text _currentModeTxt;

    [Header("Validate Buttons")]
    [SerializeField] GameObject _buttonValidateConstruction;
    [SerializeField] GameObject _buttonValidateCombat;
    [SerializeField] GameObject _fireButtonCombat;

    [Header("Change Player")]
    [SerializeField] GameObject _changePlayerCanvas;
    [SerializeField] TMP_Text _changePlayerCanvasTxt;
    [SerializeField] TMP_Text _constructionTimeRemainingTxt;
    public bool ChangingPlayer;

    [Header("Action Points")]
    [SerializeField] GameObject _actionPoints;
    [SerializeField] TMP_Text _actionPointsTxt;
    [SerializeField] TMP_Text _currentRoundTxt;

    [Header("Infos Ability")]
    [SerializeField] GameObject _infosAbility;
    [SerializeField] Image _infosAbilityIcon;
    [SerializeField] TMP_Text _infosNameAbility;
    [SerializeField] TMP_Text _infosDescriptionAbility;
    [SerializeField] TMP_Text _infosCooldown;

    [Header("Infos Room")]
    [SerializeField] GameObject _infosRoom;
    [SerializeField] Image _infosRoomPattern;
    [SerializeField] TMP_Text _infosNameRoom;
    [SerializeField] TMP_Text _infosNameRoomAbility;
    [SerializeField] TMP_Text _infosDescriptionRoomAbility;
    [SerializeField] TMP_Text _infosCooldownRoomAbility;

    [Header("Ability Buttons")]
    [SerializeField] GameObject _randomizeRoomsButton;
    private List<GameObject> _abilityButtons = new List<GameObject>();

    [Header("Victory")]
    [SerializeField] GameObject _victoryCanvas;
    [SerializeField] TMP_Text _victoryTxt;

    [Header("Switch Ship")]
    [SerializeField] GameObject _switchShipButton;
    [SerializeField] Image _switchShipArrow;
    [SerializeField] TMP_Text _goToShipText;

    [Header("Ability Bonus")]
    [SerializeField] Image _alternateShotIcon;
    [SerializeField] Image _simpleHitX2Img;
    [SerializeField] TMP_Text _probeCount;
    [SerializeField] Image _upgradeShotIcon;
    [SerializeField] Sprite _upgradeShotLvl1;
    [SerializeField] Sprite _upgradeShotLvl2;
    [SerializeField] Sprite _upgradeShotLvl3;
    [SerializeField] Sprite _upgradeShotLvl4;

    [Header("Life")]
    [SerializeField] TMP_Text _enemyLife;
    [SerializeField] TMP_Text _playerLife;

    [Header("Current Player Informations")]
    [SerializeField] GameObject _currentPlayerCorner;
    [SerializeField] Image _currentCaptainImg;
    [SerializeField] TMP_Text _currentCaptainName;
    [SerializeField] List<RawImage> _playerLifeImagesFrom1To6;

    [Header("Rewind")]
    [SerializeField] TMP_Text _rewindTxt;

    private SpriteRenderer _spriteRenderer;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _abilityButtons = AbilityButtonsManager.instance.GetAbilityButtonsList();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        ShowButtonValidateConstruction();
        ShowRandomizeRoomsButton();
        HideButtonsCombat();
        HideActionPoints();
        HideFicheAbility();
        HideFicheRoom();
        HideValidateCombat();
        HideVictoryCanvas();
        HideSwitchShipButton();
        HideFireButton();
        HideProbeCount();
        HidePlayerCorner();
        HideRewindTxt();
    }

    public void HideEndTurnButton()
    {
        _buttonValidateCombat.SetActive(false); 
    }

    public void CheckIfShowEndTurnButton()
    {
        if (ActionPointsManager.instance.GetPlayerActionPoints(GameManager.instance.PlayerTurn) == 0)
            _buttonValidateCombat.SetActive(true);
        else
            _buttonValidateCombat.SetActive(false);
    }

    public void GoToPlayerShipText()
    {
        _goToShipText.text = "SEE YOUR\nSPACESHIP";
    }

    public void GoToEnemyShipText()
    {
        _goToShipText.text = "SEE ENEMY\nSPACESHIP";
    }

    public void UpdateSwitchShipArrow()
    {
        if ((GameManager.instance.PlayerTurn == Player.Player1 && CameraController.instance.CombatOwnSpaceShip) || (GameManager.instance.PlayerTurn == Player.Player2 && !CameraController.instance.CombatOwnSpaceShip))
        {
            _switchShipArrow.transform.eulerAngles = new Vector3(0, 0, -90);
        }
        else
        {
            _switchShipArrow.transform.eulerAngles = new Vector3(0, 0, 90);
        }
    }

    public void HideRewindTxt()
    {
        _rewindTxt.gameObject.SetActive(false);
    }

    public void ShowRewindTxt()
    {
        _rewindTxt.gameObject.SetActive(true);
    }

    public void ShowRewindUI()
    {
        ShowRewindTxt();
        HideActionPoints();
        _playerBottom.SetActive(false);
        HideValidateCombat();
        TargetController.instance.HideTarget();
        HideFicheAbility();
        HideFicheRoom();
    }

    public void BackToCombatUI()
    {
        HideRewindTxt();
        ShowOrUpdateActionPoints();
        _playerBottom.SetActive(true);
        CheckIfShowEndTurnButton();
        //HideFicheAbility();
        HideFicheRoom();
    }

    public void HidePlayerCorner()
    {
        _currentPlayerCorner.SetActive(false);
    }

    public void UpdateCurrentPlayer()
    {
        _currentPlayerCorner.SetActive(true);

        _currentCaptainImg.sprite = GameManager.instance.GetPlayerShip().ShipData.CaptainImg;
        _currentCaptainName.text = GameManager.instance.GetPlayerShip().ShipData.CaptainName.ToUpper();

        UpdatePlayerLife();
    }

    public void HideProbeCount()
    {
        _probeCount.gameObject.SetActive(false);
    }

    public void ShowProbeCount(int count)
    {
        _probeCount.gameObject.SetActive(true);
        _probeCount.text = count.ToString() + "/3";
    }

    public void CheckAlternateShotDirectionImgRotation()
    {
        AlternateShotDirection _currentAlternateShotDirection = AbilityButtonsManager.instance.GetCurrentPlayerAlternateShotDirection();

        if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
        {
            Debug.Log("horizontal");
            _alternateShotIcon.rectTransform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            Debug.Log("vertical");
            _alternateShotIcon.rectTransform.eulerAngles = new Vector3(0, 0, 90);
        }
    }

    public void CheckUpgradeShotLvlImg()
    {
        UpgradeShotStep _currentUpgradeShotStep = AbilityButtonsManager.instance.GetCurrentPlayerUpgradeShotStep();

        switch (_currentUpgradeShotStep)
        {
            case (UpgradeShotStep.RevealOneTile):
                Debug.Log("upgrade shot lvl 1");
                _upgradeShotIcon.sprite = _upgradeShotLvl1;
                break;
            case (UpgradeShotStep.DestroyOneTile):
                Debug.Log("upgrade shot lvl 2");
                _upgradeShotIcon.sprite = _upgradeShotLvl2;
                break;
            case (UpgradeShotStep.DestroyThreeTilesInDiagonal):
                Debug.Log("upgrade shot lvl 3");
                _upgradeShotIcon.sprite = _upgradeShotLvl3;
                break;
            case (UpgradeShotStep.DestroyFiveTilesInCross):
                Debug.Log("upgrade shot lvl 4");
                _upgradeShotIcon.sprite = _upgradeShotLvl4;
                break;
        }
    }

    public void CheckSimpleHitX2Img()
    {
        Debug.Log("check simple hit x2 img");
        bool showImg;
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            showImg = AbilityButtonsManager.instance.SimpleHitX2Player1;
        }
        else
        {
            showImg = AbilityButtonsManager.instance.SimpleHitX2Player2;
        }

        _simpleHitX2Img.enabled = showImg;
    }

    #region Switch Ship 
    public void HideSwitchShipButton()
    {
        _switchShipButton.SetActive(false);
    }

    public void ShowShitchShipButton()
    {
        _switchShipButton.SetActive(true);
    }
    #endregion

    public void UpdateEnemyLife()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            _enemyLife.text = "ENEMY LIFE : " + GameManager.instance.GetPlayerLife(Player.Player2) + "/6";
        }
        else
        {
            _enemyLife.text = "ENEMY LIFE : " + GameManager.instance.GetPlayerLife(Player.Player1) + "/6";
        }
    }

    public void UpdatePlayerLife()
    {
        int playerLife = GameManager.instance.GetPlayerLife(GameManager.instance.PlayerTurn);
        _playerLife.text = playerLife + "/6";

        int i = 1;
        bool stopEnabling = false;

        foreach(RawImage lifeImg in _playerLifeImagesFrom1To6)
        {
            if (!stopEnabling)
                lifeImg.enabled = true;
            else
                lifeImg.enabled = false;

            if (playerLife == i)
            {
                stopEnabling = true;
            }

            i++;
        }
    }

    #region Victory / Game Canvas
    public void HideVictoryCanvas()
    {
        _victoryCanvas.SetActive(false);
    }

    public void ShowVictoryCanvas(Player winner)
    {
        _victoryCanvas.SetActive(true);
        _victoryTxt.text = "Winner : " + winner.ToString();
    }

    public void HideGameCanvas()
    {
        _gameCanvas.SetActive(false);
        _playerBottom.SetActive(false);
        _enemyTop.SetActive(false);
    }

    public void ShowGameCanvas()
    {
        _gameCanvas.SetActive(true);
    }

    public void StartGameCanvas()
    {
        _playerBottom.SetActive(true);
        _enemyTop.SetActive(true);
    }
    #endregion

    #region Randomize Rooms Button
    public void HideRandomizeRoomsButton()
    {
        _randomizeRoomsButton.SetActive(false);
    }

    public void ShowRandomizeRoomsButton()
    {
        _randomizeRoomsButton.SetActive(true);
    }
    #endregion

    #region CurrentPlayer
    public void UpdateCurrentPlayerTxt(Player playerTurn)
    {
        _currentPlayerTxt.text = playerTurn.ToString().ToUpper();
    }

    public void UpdateCurrentModeTxt(Mode currentMode)
    {
        _currentModeTxt.text = currentMode.ToString().ToUpper();
    }
    #endregion

    #region ValidateConstruction
    public void HideButtonValidateConstruction()
    {
        _buttonValidateConstruction.SetActive(false);
    }

    public void ShowButtonValidateConstruction()
    {
        _buttonValidateConstruction.SetActive(true);
    }
    #endregion

    #region ValideCombat
    public void HideValidateCombat()
    {
        _buttonValidateCombat.SetActive(false);
    }

    public void HideFireButton()
    {
        _fireButtonCombat.SetActive(false); 
    }

    public void ShowValidateCombat()
    {
        _buttonValidateCombat.SetActive(true);
        _fireButtonCombat.SetActive(false);
    }

    public void ShowFireButton()
    {
        _buttonValidateCombat.SetActive(false);
        _fireButtonCombat.SetActive(true);
    }
    #endregion

    #region Ability Buttons
    public void HideButtonsCombat()
    {
        Debug.Log("hide buttons combat");
        foreach (GameObject abilityButton in _abilityButtons)
        {
            abilityButton.SetActive(false);  
        }
    }

    public void ShowButtonsCombat()
    {
        Debug.Log("show buttons combat");

        foreach (GameObject abilityButton in _abilityButtons)
        {
            abilityButton.SetActive(true);
        }

        CheckIfShowEndTurnButton();
    }

    public void CheckAbilityButtonsColor()
    {
        Debug.Log("check ability buttons color");
        foreach (GameObject abilityButton in _abilityButtons)
        {

            AbilityButton button = abilityButton.GetComponentInChildren<AbilityButton>();
            //Debug.Log(button.name);
            if (ActionPointsManager.instance.GetPlayerActionPoints(GameManager.instance.GetCurrentPlayer()) > 0 && !button.IsOffline && GameManager.instance.GetCurrentCooldown(button.GetAbility()) == 0)
            {
                if (button.IsSelected)
                {
                    button.SelectedButtonUI();
                }
                else
                {
                    button.OnlineAndCanBeUsedButtonUI();
                }
            }
            else if (button.IsOffline)
            {
                button.OfflineButtonUI();
            }
            else
            {
                button.OnlineAndCantBeUsedButtonUI();
            }
        }
    }

    #endregion

    #region Change Player
    public void ShowChangerPlayerCanvas(Player playerTurn)
    {
        _changePlayerCanvas.SetActive(true);
        _changePlayerCanvasTxt.text = "Player's turn : " + playerTurn.ToString();
        ChangingPlayer = true;
    }

    public void HideChangePlayerCanvas()
    {
        _changePlayerCanvas.SetActive(false);
        ChangingPlayer = false;

        if (GameManager.instance.GetCurrentMode() == Mode.Combat && !(GameManager.instance.PlayerTurn == Player.Player1 && GameManager.instance.GetCurrentRound() == 1))
        {
            AbilityButtonsManager.instance.Rewind();
        }
        
        GameManager.instance.CheckIfStartConstructionTimer();
    }

    #endregion

    #region Construction Timer
    public void UpdateConstructionTimerTxt(float remainingTime)
    {
        remainingTime += 1;
        if (remainingTime < 0)
        {
            remainingTime = 0;
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        _constructionTimeRemainingTxt.text = string.Format("({0:00}:{1:00} remaining...)", minutes, seconds);
    }
    #endregion

    #region Action Points
    public void HideActionPoints()
    {
        _actionPoints.SetActive(false);
    }

    public void ShowOrUpdateActionPoints()
    {
        Debug.Log("show or update action points");
        _actionPoints.SetActive(true);

        _actionPointsTxt.text = ActionPointsManager.instance.GetPlayerActionPoints(GameManager.instance.GetCurrentPlayer()).ToString();
        _currentRoundTxt.text = "Current round : " + GameManager.instance.GetCurrentRound().ToString();
    }
    #endregion

    #region Fiche Ability & Room
    public void ShowFicheAbility(scriptablePower abilityData)
    {
        _infosAbility.SetActive(true);

        UpdateFicheAbility(abilityData);
    }

    public void HideFicheAbility()
    {
        _infosAbility.SetActive(false);
    }

    public bool IsFicheAbilityOpened()
    {
        return _infosAbility.activeSelf;
    }

    public void UpdateFicheAbility(scriptablePower abilityData)
    {
        _infosNameAbility.text = abilityData.AbilityName.ToUpper();
        _infosDescriptionAbility.text = "Description : \n\n" + abilityData.Description.ToUpper();
        _infosCooldown.text = abilityData.Cooldown.ToString();
        _infosAbilityIcon.sprite = abilityData.Icon;
    }

    public bool IsFicheAbilityWithSameAbility(scriptablePower abilityData)
    {
        return _infosNameAbility.text == abilityData.AbilityName.ToUpper();
    }

    public void ShowFicheRoom(RoomSO roomData)
    {
        _infosRoom.SetActive(true);

        _infosRoomPattern.sprite = roomData.RoomPatternImg;
        _infosNameRoom.text = roomData.RoomName.ToUpper();

        UpdateFicheRoom(roomData);
    }

    public void UpdateFicheRoom(RoomSO roomData)
    {
        if (roomData.RoomAbility != null)
        {
            _infosNameRoomAbility.gameObject.SetActive(true);
            _infosDescriptionRoomAbility.gameObject.SetActive(true);
            _infosCooldownRoomAbility.gameObject.SetActive(true);

            _infosNameRoomAbility.text = roomData.RoomAbility.AbilityName.ToUpper();
            _infosDescriptionRoomAbility.text = roomData.RoomAbility.Description.ToUpper();
            _infosCooldownRoomAbility.text = roomData.RoomAbility.Cooldown.ToString();
        }
        else
        {
            _infosNameRoomAbility.gameObject.SetActive(false);
            _infosDescriptionRoomAbility.gameObject.SetActive(false);
        }
    }

    public void HideFicheRoom()
    {
        _infosRoom.SetActive(false);
    }
    #endregion
}
