using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Search;

public class DraftRoom : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] Image _infosRoomIcon;
    [SerializeField] Image _infosRoomPattern;
    [SerializeField] TMP_Text _infosNameRoom;
    [SerializeField] TMP_Text _infosNameRoomAbility;
    [SerializeField] TMP_Text _infosDescriptionRoomAbility;

    private RoomSO _roomData;
    private Room _room;
    // ----- FIELDS ----- //

    public void InitDraftRoom(Room room)
    {
        _room = room;
        _roomData = room.RoomData;

        _infosRoomIcon.sprite = _roomData.RoomIcon;
        _infosRoomPattern.sprite = _roomData.RoomPatternImg;
        _infosNameRoom.text = _roomData.RoomName;
        _infosNameRoomAbility.text = _roomData.RoomAbility.AbilityName;
        _infosDescriptionRoomAbility.text = _roomData.RoomAbility.Description;
    }

    public void SelectDraftRoom() // on click
    {
        GameManager.instance.SelectDraftRoom(_room);
    }
}
