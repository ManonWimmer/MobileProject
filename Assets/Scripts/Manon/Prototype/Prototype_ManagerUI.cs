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
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ShowButtonValidateConstruction();
        HideButtonValidateCombat();
        HideEnergySlider();
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

    // validate combat
    public void HideButtonValidateCombat()
    {
        _buttonValidateCombat.SetActive(false);
    }

    public void ShowButtonValidateCombat()
    {
        _buttonValidateCombat.SetActive(true);
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
    }
}
