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

    [Header("Debug Text")]
    [SerializeField] TMP_Text _currentPlayerTxt;
    [SerializeField] TMP_Text _currentModeTxt;

    [Header("Validate Buttons")]
    [SerializeField] GameObject _buttonValidateConstruction;
    [SerializeField] GameObject _buttonValidateCombat;

    [Header("Change Player")]
    [SerializeField] GameObject _changePlayerCanvas;
    [SerializeField] TMP_Text _changePlayerCanvasTxt;
    [SerializeField] TMP_Text _constructionTimeRemainingTxt;
    public bool ChangingPlayer;

    [Header("Action Points")]
    [SerializeField] TMP_Text _actionPointsTxt;
    [SerializeField] TMP_Text _currentRoundTxt;

    [Header("Infos Ability")]
    [SerializeField] GameObject _infosAbility;
    [SerializeField] TMP_Text _infosNameAbility;
    [SerializeField] TMP_Text _infosDescriptionAbility;
    [SerializeField] TMP_Text _infosEnergy;

    [Header("Infos Room")]
    [SerializeField] GameObject _infosRoom;
    [SerializeField] Image _infosRoomIcon;
    [SerializeField] Image _infosRoomPattern;
    [SerializeField] TMP_Text _infosNameRoom;
    [SerializeField] TMP_Text _infosNameRoomAbility;
    [SerializeField] TMP_Text _infosDescriptionRoomAbility;

    [Header("Ability Buttons")]
    [SerializeField] GameObject _randomizeRoomsButton;
    private List<GameObject> _abilityButtons = new List<GameObject>();

    [Header("Victory")]
    [SerializeField] GameObject _victoryCanvas;
    [SerializeField] TMP_Text _victoryTxt;

    [Header("Switch Ship")]
    [SerializeField] GameObject _switchShipButton;

    [Header("Ability Bonus Images")]
    [SerializeField] Image _alternateShotDirectionImg;
    [SerializeField] Image _simpleHitX2Img;

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
    }

    public void CheckAlternateShotDirectionImgRotation()
    {
        AlternateShotDirection _currentAlternateShotDirection;
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            _currentAlternateShotDirection = AbilityButtonsManager.instance.CurrentAlternateShotDirectionPlayer1;
        }
        else
        {
            _currentAlternateShotDirection = AbilityButtonsManager.instance.CurrentAlternateShotDirectionPlayer2;
        }

        if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
        {
            Debug.Log("horizontal");
            _alternateShotDirectionImg.rectTransform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            Debug.Log("vertical");
            _alternateShotDirectionImg.rectTransform.eulerAngles = new Vector3(0, 0, 90);
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
    }

    public void ShowGameCanvas()
    {
        _gameCanvas.SetActive(true);
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
        _currentPlayerTxt.text = "Current Player : " + playerTurn.ToString();
    }

    public void UpdateCurrentModeTxt(Mode currentMode)
    {
        _currentModeTxt.text = "Current Mode : " + currentMode.ToString();
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

    public void ShowValidateCombat()
    {
        _buttonValidateCombat.SetActive(true);
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

        ShowValidateCombat();
    }

    public void CheckAbilityButtonsColor()
    {
        Debug.Log("check ability buttons color");
        foreach (GameObject abilityButton in _abilityButtons)
        {
            AbilityButton button = abilityButton.GetComponentInChildren<AbilityButton>();
            if (ActionPointsManager.instance.GetPlayerActionPoints(GameManager.instance.GetCurrentPlayer()) > 0 && GameManager.instance.IsTargetOnTile() && TargetController.instance.CanShootOnThisTile() && !button.IsOffline && GameManager.instance.GetCurrentCooldown(button.GetAbility()) == 0)
            {
                if (button.IsSelected)
                {
                    button.GetComponent<Image>().color = new Color(0.34f, 0.54f, 77f);
                }
                else
                {
                    button.GetComponent<Image>().color = Color.white;
                }
            }
            else if (button.IsOffline)
            {
                button.GetComponent<Image>().color = Color.red;
            }
            else
            {
                button.GetComponent<Image>().color = Color.gray;
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
        _actionPointsTxt.gameObject.SetActive(false);
        _currentModeTxt.gameObject.SetActive(false);
    }

    public void ShowOrUpdateActionPoints()
    {
        Debug.Log("show or update action points");
        _actionPointsTxt.gameObject.SetActive(true);
        _currentModeTxt.gameObject.SetActive(true);

        _actionPointsTxt.text = "Action points : " + ActionPointsManager.instance.GetPlayerActionPoints(GameManager.instance.GetCurrentPlayer());
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
        _infosNameAbility.text = "Name : " + abilityData.AbilityName;
        _infosDescriptionAbility.text = "Description : \n\n" + abilityData.Description;
        _infosEnergy.text = "Power needed : " + abilityData.ActionPointsNeeded.ToString();
    }

    public bool IsFicheAbilityWithSameAbility(scriptablePower abilityData)
    {
        return _infosNameAbility.text == "Name : " + abilityData.AbilityName;
    }

    public void ShowFicheRoom(RoomSO roomData)
    {
        _infosRoom.SetActive(true);

        _infosRoomIcon.sprite = roomData.RoomIcon;
        _infosRoomPattern.sprite = roomData.RoomPatternImg;
        _infosNameRoom.text = roomData.RoomName;

        if (roomData.RoomAbility != null)
        {
            _infosNameRoomAbility.gameObject.SetActive(true);
            _infosDescriptionRoomAbility.gameObject.SetActive(true);

            _infosNameRoomAbility.text = roomData.RoomAbility.AbilityName;
            _infosDescriptionRoomAbility.text = roomData.RoomAbility.Description;
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
