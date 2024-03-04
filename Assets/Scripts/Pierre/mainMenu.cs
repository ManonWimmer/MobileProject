using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenu : MonoBehaviour
{
    private GameObject _menu;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _credits;

    private void Start()
    {
        _menu = GetComponent<GameObject>();
    }

    public void StartDuel()
    {

    }

    public void StartTuto()
    {

    }

    public void OpenSettings()
    {
        _settings.SetActive(true);
    }

    public void CloseSettings()
    {
        _settings.SetActive(false);
    }

    public void OpenCredits()
    {
        _credits.SetActive(true);
    }

    public void Leave()
    {
        Application.Quit();
    }
}
