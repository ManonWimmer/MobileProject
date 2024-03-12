using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class dialogue : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private float _timeDialogue;
    [SerializeField] private xmlReader _xmlReader;

    [Header("Dialogue Hit")]
    [SerializeField] private List<string> _dialoguesNerdHit = new List<string>();
    [SerializeField] private List<string> _dialoguesCowHit = new List<string>();
    [SerializeField] private List<string> _dialoguesPizzaHit = new List<string>();

    [Header("Dialogue Attack")]
    [SerializeField] private List<string> _dialoguesNerdAttack = new List<string>();
    [SerializeField] private List<string> _dialoguesCowAttack = new List<string>();
    [SerializeField] private List<string> _dialoguesPizzaAttack = new List<string>();

    [Header("Dialogue Victory")]
    [SerializeField] private List<string> _dialoguesNerdVictory = new List<string>();
    [SerializeField] private List<string> _dialoguesCowVictory = new List<string>();
    [SerializeField] private List<string> _dialoguesPizzaVictory = new List<string>();

    [SerializeField] float _textSpeed;
    private string _lines;
    private int _index;
    private bool _isWriting = false;

    private int _lastIndex = 0;
    private bool _isPlayed;

    public void StartDialogueText() => SetDialogueText();
    public bool GetDilogueIsPlayed() => _isPlayed;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _dialogue = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _isPlayed = false;
    }

    private void SetDialogueText()
    {
        if (!_isPlayed)
        {
            string text = RandomDialogue(_dialoguesNerdHit);
            Enable(text);

            _animator.SetBool("Close", false);
            _animator.SetTrigger("Open");
        }
    }

    private void Enable(string text)
    {
        gameObject.name = text;
        _lines = _xmlReader.GetText(text);
        _dialogue.text = string.Empty;
        _isPlayed = true;

        //SeparateString(text);
        StartDialogue();
    }

    private void Disable()
    {
        _index = 0;
        _dialogue.text = string.Empty;
        _isPlayed = false;
    }

    private void StartDialogue()
    {
        _index = 0;
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        _isWriting = true;

        foreach (char c in _lines)
        {
            _dialogue.text += c;
            //sounds
            yield return new WaitForSeconds(_textSpeed);
        }

        StartCoroutine(TimeDialogue());

        /*_isWriting = false;

        while (_isWriting)
        {
            yield return null;
        }
        NextLine(); */
    }

    private IEnumerator TimeDialogue()
    {
        yield return new WaitForSeconds(_timeDialogue);

        Disable();
        _animator.SetBool("Close", true);
    }


    //SEPERATE LINES 

    /*private void NextLine()
    {
        if (_index < _lines.Length - 1)
        {
            _index++;
            _dialogue.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            StartCoroutine(TimeDialogue());
        }
    }

    private void SeparateString(string text)
    {
        _lines = text.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
    } */

    private string RandomDialogue(List<string> listDialogue)
    {
        string dialogue;
        int index;
        int lastIndex = 0;

        do
        {
            index = UnityEngine.Random.Range(0, listDialogue.Count);
        } while (index == lastIndex);

        lastIndex = index;
        dialogue = listDialogue[index];
        return dialogue;
    } 

}
