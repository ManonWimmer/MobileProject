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

    [SerializeField] int _roomIndex;

    private Image _image;

    private RoomSO _roomData;
    private Room _room;
    // ----- FIELDS ----- //

    private void Start()
    {
        _image = GetComponent<Image>();
    }

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
        DraftManager.instance.SelectRoom(_roomIndex);
    }

    public Room GetRoom()
    {
        return _room;
    }

    public void SelectRoomUI()
    {
        GetComponent<Image>().color = new Color(0.34f, 0.54f, 0.77f, 1f);
    }

    public void DeselectRoomUI()
    {
        GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f);
    }
}
