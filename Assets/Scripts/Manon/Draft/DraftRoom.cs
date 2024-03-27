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
    [SerializeField] TextMeshProUGUI _infosNameRoom;
    [SerializeField] TextMeshProUGUI _infosNameRoomAbility;
    [SerializeField] TextMeshProUGUI _infosDescriptionRoomAbility;
    [SerializeField] TMP_Text _infosCooldownRoomAbility;
    [SerializeField] Animator _animator;

    [SerializeField] int _roomIndex;

    private RoomSO _roomData;
    private Room _room;
    private int _currentLanguage = -1;

    // ----- FIELDS ----- //

    private void Update()
    {
        if (_currentLanguage != xmlReader.instance.GetLanguage() && _currentLanguage != -1)
        {
            InitDraftRoom(_room);
        }
    }

    public void InitDraftRoom(Room room)
    {
        _room = room;
        _roomData = room.RoomData;

        _infosRoomPattern.sprite = _roomData.RoomPatternImg;
        _infosNameRoom.text = _roomData.RoomName;
        _infosNameRoomAbility.text = _roomData.RoomAbility.AbilityName;
        _infosDescriptionRoomAbility.text = xmlReader.instance.GetText(_roomData.RoomAbility.Description);
        _infosCooldownRoomAbility.text = _roomData.RoomAbility.Cooldown.ToString();

        _currentLanguage = xmlReader.instance.GetLanguage();
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
