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

    [Header("Energy")]
    [SerializeField] Slider _energySlider;
    [SerializeField] TMP_Text _energyTxt;

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
    [SerializeField] List<GameObject> _abilityButtons = new List<GameObject>();

    [Header("Victory")]
    [SerializeField] GameObject _victoryCanvas;
    [SerializeField] TMP_Text _victoryTxt;

    private SpriteRenderer _spriteRenderer;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        ShowButtonValidateConstruction();
        ShowRandomizeRoomsButton();
        HideButtonsCombat();
        HideEnergySlider();
        HideFicheAbility();
        HideFicheRoom();
        HideValidateCombat();
        HideVictoryCanvas();
    }

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

    public void HideRandomizeRoomsButton()
    {
        _randomizeRoomsButton.SetActive(false);
    }

    public void ShowRandomizeRoomsButton()
    {
        _randomizeRoomsButton.SetActive(true);
    }

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
        foreach (GameObject abilityButton in _abilityButtons)
        {
            abilityButton.SetActive(false);  
        }
    }

    public void ShowButtonsCombat()
    {
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
            if (_energySlider.value >= button.GetAbility()._powerNeed && GameManager.instance.IsTargetOnTile() && TargetController.instance.CanShootOnThisTile() && !button.IsOffline)
            {
                button.GetComponent<Image>().color = Color.white;
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

    #region Energy
    public void HideEnergySlider()
    {
        _energySlider.gameObject.SetActive(false);
        _energyTxt.enabled = false;
    }

    public void ShowEnergySlider()
    {
        _energySlider.gameObject.SetActive(true);
        _energyTxt.enabled = true;
        _energySlider.maxValue = EnergySystem.instance.GetMaxEnergy();
        _energySlider.value = EnergySystem.instance.GetStartEnergy();
    }

    public void UpdateEnergySlider(Player player)
    {
        _energySlider.value = EnergySystem.instance.GetPlayerEnergy(player);
        _energyTxt.text = "Energy \n" + _energySlider.value + "/" + _energySlider.maxValue;

        CheckAbilityButtonsColor();
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
        _infosNameAbility.text = "Name : " + abilityData._powerName;
        _infosDescriptionAbility.text = "Description : \n\n" + abilityData._description;
        _infosEnergy.text = "Power needed : " + abilityData._powerNeed.ToString();
    }

    public bool IsFicheAbilityWithSameAbility(scriptablePower abilityData)
    {
        return _infosNameAbility.text == "Name : " + abilityData._powerName;
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

            _infosNameRoomAbility.text = roomData.RoomAbility._powerName;
            _infosDescriptionRoomAbility.text = roomData.RoomAbility._description;
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
