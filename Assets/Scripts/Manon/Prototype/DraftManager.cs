using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static DraftManager instance;
    [SerializeField] List<DraftRoom> _draftRooms = new List<DraftRoom>();
    private int _selectedRoomIndex = 0;
    private int _currentDraft = 0;

    public int CurrentDraft { get => _currentDraft; set => _currentDraft = value; }

    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    public void StartDraft(int number)
    {
        _currentDraft = number;
        SelectRoom(0);
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

    public void ValidateDraftRoomSelection() // on click
    {
        GameManager.instance.SelectDraftRoom(_draftRooms[_selectedRoomIndex].GetRoom());
    }
}
