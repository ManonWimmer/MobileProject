using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static DraftManager instance;

    [SerializeField] GameObject _rooms;
    [SerializeField] GameObject _ships;

    [SerializeField] List<DraftRoom> _draftRooms = new List<DraftRoom>();
    [SerializeField] List<DraftShip> _draftShips = new List<DraftShip>();

    private int _selectedRoomIndex = 0;
    private int _selectedShipIndex = 0;
    private int _currentRoomDraft = 0;
    private bool shipDraft;
    public int CurrentDraft { get => _currentRoomDraft; set => _currentRoomDraft = value; }

    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    public void StartDraftRooms(int number)
    {
        _rooms.SetActive(true);
        _ships.SetActive(false);

        _currentRoomDraft = number;
        shipDraft = false;
        SelectRoom(0);
    }

    public void StartDraftShips()
    {
        _rooms.SetActive(false);
        _ships.SetActive(true);

        _currentRoomDraft = 0;
        shipDraft = true;
        SelectShip(0);
    }

    public void SelectRoom(int index)
    {
        _selectedRoomIndex = index;
        
        foreach(DraftRoom room in _draftRooms)
        {
            if (room == _draftRooms[_selectedRoomIndex])
            {
                room.SelectRoomUI();
            }
            else
            {
                room.DeselectRoomUI();
            }
        }
    }

    public void SelectShip(int index)
    {
        Debug.Log("select ship " + index);
        _selectedShipIndex = index;

        foreach (DraftShip ship in _draftShips)
        {
            if (ship == _draftShips[_selectedShipIndex])
            {
                ship.SelectShipUI();
            }
            else
            {
                ship.DeselectShipUI();
            }
        }
    }

    public void ValidateDraftSelection() // on click
    {
        if (!shipDraft)
        {
            Debug.Log("validate draft rooms");
            GameManager.instance.SelectDraftRoom(_draftRooms[_selectedRoomIndex].GetRoom());
        }
        else
        {
            Debug.Log("validate draft ships");
            GameManager.instance.SelectDraftShip(_draftShips[_selectedShipIndex].GetShip());
        }
    }
}
