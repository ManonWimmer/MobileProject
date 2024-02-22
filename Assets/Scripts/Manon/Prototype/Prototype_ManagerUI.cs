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
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ShowButtonValidateConstruction();
        HideButtonValidateCombat();
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
}
