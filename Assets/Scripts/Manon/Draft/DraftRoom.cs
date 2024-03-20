using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraftRoom : MonoBehaviour
{
    // ----- FIELDS ----- //
    [SerializeField] Image _infosRoomPattern;
    [SerializeField] Image _infosRoomSelectedBackground;
    [SerializeField] TMP_Text _infosNameRoom;
    [SerializeField] TMP_Text _infosNameRoomAbility;
    [SerializeField] TMP_Text _infosDescriptionRoomAbility;
    [SerializeField] TMP_Text _infosCooldownRoomAbility;
    [SerializeField] Animator _animator;

    [SerializeField] int _roomIndex;

    private RoomSO _roomData;
    private Room _room;
    // ----- FIELDS ----- //

    public void InitDraftRoom(Room room)
    {
        _room = room;
        _roomData = room.RoomData;

        _infosRoomPattern.sprite = _roomData.RoomPatternImg;
        _infosNameRoom.text = _roomData.RoomName.ToUpper();
        _infosNameRoomAbility.text = _roomData.RoomAbility.AbilityName.ToUpper();
        _infosDescriptionRoomAbility.text = _roomData.RoomAbility.Description.ToUpper();
        _infosCooldownRoomAbility.text = _roomData.RoomAbility.Cooldown.ToString();
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
        _animator.SetBool("Unselected", false);
        _animator.SetTrigger("Selected");
    }

    public void DeselectRoomUI()
    {
        _animator.SetBool("Unselected", true);
    }
}
