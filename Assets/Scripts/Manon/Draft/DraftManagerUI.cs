using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DraftManagerUI : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static DraftManagerUI instance;

    [SerializeField] GameObject _draftGameObjet;
    [SerializeField] TMP_Text _playerChoosing;

    [SerializeField] DraftRoom _draftRoom0;
    [SerializeField] DraftRoom _draftRoom1;
    [SerializeField] DraftRoom _draftRoom2;

    [SerializeField] DraftShip _draftShip0;
    [SerializeField] DraftShip _draftShip1;
    [SerializeField] DraftShip _draftShip2;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    public void ShowDraftUI()
    {
        _draftGameObjet.SetActive(true);
        UIManager.instance.HideGameCanvas();
    }

    public void UpdatePlayerChoosing()
    {
        _playerChoosing.text = "Player choice : " + GameManager.instance.PlayerTurn;
    }

    public void HideDraftUI()
    {
        _draftGameObjet.SetActive(false);
        UIManager.instance.ShowGameCanvas();
    }

    public void InitDraftRoom(int index, Room room)
    {
        if (index == 0)
        {
            _draftRoom0.InitDraftRoom(room);
        }
        else if (index == 1)
        {
            _draftRoom1.InitDraftRoom(room);
        }
        else if (index == 2)
        {
            _draftRoom2.InitDraftRoom(room);
        }
        else
        {
            Debug.Log("ERREUR D'INDEX");
        }
    }

    public void InitDraftShip(int index, Ship ship)
    {
        if (index == 0)
        {
            _draftShip0.InitDraftShip(ship);
        }
        else if (index == 1)
        {
            _draftShip1.InitDraftShip(ship);
        }
        else if (index == 2)
        {
            _draftShip2.InitDraftShip(ship);
        }
        else
        {
            Debug.Log("ERREUR D'INDEX");
        }
    }
}
