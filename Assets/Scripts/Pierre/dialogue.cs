using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class dialogue : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private float _timeDilogue;

    [SerializeField] private List<string> _dialoguesNerd = new List<string>();
    private List<string> _dialoguesCow = new List<string>();
    private List<string> _dialoguesPizza = new List<string>();

    private int _lastIndex = 0;
    private string _text;
    private bool _isPlayed;

    public void DialogueText() => SetDilogueText(_text = RandomDialogue(_dialoguesNerd));
    public bool GetPlayedDilogue() => _isPlayed;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _dialogue = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _isPlayed = false;
    }

    private string RandomDialogue(List<string> listDialogue)
    {
        string dialogue;
        int index;

        do
        {
            index = Random.Range(0, listDialogue.Count);
        } while (index == _lastIndex);
        _lastIndex = index;

        dialogue = listDialogue[index];
        return dialogue;
    } 

    private void SetDilogueText(string text)
    {
        if (!_isPlayed)
        {
            _isPlayed = true;
            _animator.SetBool("Close", false);
            _animator.SetTrigger("Open");

            _dialogue.text = text;
            StartCoroutine(TimeDilogue());
        }
    }

    private IEnumerator TimeDilogue()
    {
        yield return new WaitForSeconds(_timeDilogue);
        _animator.SetBool("Close", true);
        _isPlayed = false;
    }
}
