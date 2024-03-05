using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] Image _draftRoom01Indicator;
    [SerializeField] Image _draftRoom02Indicator;
    [SerializeField] Image _draftRoom03Indicator;

    [SerializeField] Image _spaceshipDraftRoomImg;
    [SerializeField] Image _patternRoom01DraftRoomImg;
    [SerializeField] Image _patternRoom02DraftRoomImg;
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
        _playerChoosing.text = "PLAYER : " + GameManager.instance.PlayerTurn.ToString().ToUpper();
    }

    public void UpdateSpaceshipDraftRoom()
    {
        Debug.Log("update spaceship draft room");
        _spaceshipDraftRoomImg.sprite = GameManager.instance.GetPlayerShip().ShipData.ShipImg;
        int roomDraft = DraftManager.instance.CurrentDraft;

        if (roomDraft == 1)
        {
            _patternRoom01DraftRoomImg.gameObject.SetActive(false);
            _patternRoom02DraftRoomImg.gameObject.SetActive(false);
        }
        else if (roomDraft == 2)
        {
            _patternRoom01DraftRoomImg.gameObject.SetActive(true);
            _patternRoom02DraftRoomImg.gameObject.SetActive(false);

            _patternRoom01DraftRoomImg.sprite = GameManager.instance.GetChoosenDraftRoom(0).RoomData.RoomPatternImg;
        }
        else
        {
            _patternRoom01DraftRoomImg.gameObject.SetActive(true);
            _patternRoom02DraftRoomImg.gameObject.SetActive(true);

            _patternRoom01DraftRoomImg.sprite = GameManager.instance.GetChoosenDraftRoom(0).RoomData.RoomPatternImg;
            _patternRoom02DraftRoomImg.sprite = GameManager.instance.GetChoosenDraftRoom(1).RoomData.RoomPatternImg;
        }
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

    public void CurrentDraftRoomO1Indicator()
    {
        _draftRoom01Indicator.color = new Color(0.34f, 0.54f, 0.76f, 1f);
    }

    public void CurrentDraftRoomO2Indicator()
    {
        _draftRoom02Indicator.color = new Color(0.34f, 0.54f, 0.76f, 1f);
    }

    public void CurrentDraftRoomO3Indicator()
    {
        _draftRoom03Indicator.color = new Color(0.34f, 0.54f, 0.76f, 1f);
    }
}
