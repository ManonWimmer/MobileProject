using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tuto : MonoBehaviour
{
    [SerializeField] private dialogue _dialogue;
    [SerializeField] private int _index;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _imageTuto;
    [SerializeField] private mainMenu _mainMenu;

    public void StartDialogueTuto() => StartDialogueOpenTuto();
    public void NextTuto() => PassNextTuto();

    private void StartDialogueOpenTuto()
    {
        _index = 0;
        _dialogue.DisablePlayed();
        NextTuto();
    }
    private void PassNextTuto()
    {
        if (!_dialogue.GetDilogueIsPlayed() && _index < _dialogue.GetLenghDialogue("_dialoguesTuto"))
        {
            _image.sprite = _imageTuto[_index];
            _dialogue.StartDialogueTuto(_index);
            _index ++;
        }
        else if (_index >= _dialogue.GetLenghDialogue("_dialoguesTuto"))
        {
            _mainMenu.Close(gameObject);
        }
    }
}
