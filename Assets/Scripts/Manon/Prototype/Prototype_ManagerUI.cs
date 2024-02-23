using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Prototype_ManagerUI : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static Prototype_ManagerUI instance;
    [SerializeField] TMP_Text _currentPlayerTxt;
    [SerializeField] TMP_Text _currentModeTxt;
    [SerializeField] GameObject _buttonValidateConstruction;
    [SerializeField] GameObject _buttonValidateCombat;

    [SerializeField] GameObject _changePlayerCanvas;
    [SerializeField] TMP_Text _changePlayerCanvasTxt;
    [SerializeField] TMP_Text _constructionTimeRemainingTxt;
    public bool ChangingPlayer;

    [SerializeField] Slider _energySlider;
    [SerializeField] TMP_Text _energyTxt;

    [SerializeField] Image _testHitButton;
    [SerializeField] int _testHitEnergy = 2;

    [SerializeField] GameObject _infosRoomOrAbility;
    [SerializeField] TMP_Text _infosTitle;
    [SerializeField] TMP_Text _infosName;
    [SerializeField] TMP_Text _infosDescription;
    [SerializeField] TMP_Text _infosEnergy;

    [SerializeField] List<GameObject> _abilityButtons = new List<GameObject>();

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
        HideButtonsCombat();
        HideEnergySlider();
        HideTestHitButton();
        HideFiche();
        HideValidateCombat();
    }

    public void UpdateCurrentPlayerTxt(Player playerTurn)
    {
        _currentPlayerTxt.text = "Current Player : " + playerTurn.ToString();
    }

    public void UpdateCurrentModeTxt(Mode currentMode)
    {
        _currentModeTxt.text = "Current Mode : " + currentMode.ToString();
    }

    // Validate construction
    public void HideButtonValidateConstruction()
    {
        _buttonValidateConstruction.SetActive(false);
    }

    public void ShowButtonValidateConstruction()
    {
        _buttonValidateConstruction.SetActive(true);
    }

    public void HideValidateCombat()
    {
        _buttonValidateCombat.SetActive(false);
    }

    public void ShowValidateCombat()
    {
        _buttonValidateCombat.SetActive(true);
    }

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

    // Change Player Canvas
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

        Prototype_GameManager.instance.CheckIfStartConstructionTimer();
    }

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

    // Energy Slider
    public void HideEnergySlider()
    {
        _energySlider.gameObject.SetActive(false);
        _energyTxt.enabled = false;
    }

    public void ShowEnergySlider()
    {
        _energySlider.gameObject.SetActive(true);
        _energyTxt.enabled = true;
        _energySlider.maxValue = Prototype_EnergySystem.instance.GetMaxEnergy();
        _energySlider.value = Prototype_EnergySystem.instance.GetStartEnergy();
    }

    public void UpdateEnergySlider(Player player)
    {
        _energySlider.value = Prototype_EnergySystem.instance.GetPlayerEnergy(player);
        _energyTxt.text = "Energy \n" + _energySlider.value + "/" + _energySlider.maxValue;

        CheckTestHitColor();
    }

    public void CheckTestHitColor()
    {
        Debug.Log("check test hit color");
        if (_energySlider.value >= _testHitEnergy && Prototype_GameManager.instance.IsTargetOnTile() && Prototype_Target.instance.CanShootOnThisTile())
        {
            _testHitButton.color = Color.white;
        }
        else
        {
            _testHitButton.color = Color.gray;
        } 
    }

    public void ShowTestHitButton()
    {
        _testHitButton.gameObject.SetActive(true);
    }

    public void HideTestHitButton()
    {
        _testHitButton.gameObject.SetActive(false);
    }

    public void ShowFicheRoom()
    {
        _infosRoomOrAbility.SetActive(true);
        _infosEnergy.enabled = false;
    }

    public void ShowFicheAbility(scriptablePower ability)
    {
        _infosRoomOrAbility.SetActive(true);
        _infosEnergy.enabled = true;

        _infosTitle.text = "Ability";
        _infosName.text = "Name : " + ability._powerName;
        _infosDescription.text = "Description : \n\n" +ability._description;
        _infosEnergy.text = "Power needed : " + ability._powerNeed.ToString();
    }

    public void HideFiche()
    {
        _infosRoomOrAbility.SetActive(false);
    }
}
