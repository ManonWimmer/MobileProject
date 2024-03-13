using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using TMPro;
using UnityEngine;

public class dialogue : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private float _timeDialogue;

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

    private xmlReader _xmlReader;
    private GameManager _gameManager;

    [SerializeField] private AudioSource _audio;
    [SerializeField] private audioManager _audioManager;

    public void StartDialogueText() => SetDialogueText();
    public bool GetDilogueIsPlayed() => _isPlayed;

    public void SartDialogueHit() => SetDialogueHit();
    public void SartDialogueAttack() => SetDialogueAttack();
    public void SartDialogueWin() => SetDialogueWin();


    private void Start()
    {
        //_gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();
        _audio = GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>();
        _xmlReader = GameObject.FindGameObjectWithTag("Translate").GetComponent<xmlReader>();

        _dialogue = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
        _isPlayed = false;
    }

    #region Start dialogues
    private void SetDialogueHit()
    {
        if (!_isPlayed)
        {
            _audio.clip = RandomVoice(_audioManager.GetPlayListDialogueWin());
            _audio.Play();
            string text = RandomDialogue(FindListDialogueHit());
            Enable(text);
            OpenDialogue();
        }
    }

    private void SetDialogueAttack()
    {
        if (!_isPlayed)
        {
            _audio.clip = RandomVoice(_audioManager.GetPlayListDialogueWin());
            _audio.Play();
            string text = RandomDialogue(FindListDialogueAttack());
            Enable(text);
            OpenDialogue();
        }
    }

    private void SetDialogueWin()
    {
        if (!_isPlayed)
        {
            _audio.clip = RandomVoice(_audioManager.GetPlayListDialogueWin());
            _audio.Play();
            string text = RandomDialogue(FindListDialogueWin());
            Enable(text);
            OpenDialogue();
        }
    }

    #endregion

    private void SetDialogueText()
    {
        if (!_isPlayed)
        {
            /*if (GetShip == "CPT. COWBOY")
            {
                string text = RandomDialogue(_dialoguesCowHit);
                Enable(text);
                OpenDialogue();
            }
            else if (GetShip == "CPT. NERD")
            {
                string text = RandomDialogue(_dialoguesNerdHit);
                Enable(text);
                OpenDialogue();
            }
            else if (GetShip == "CPT. RAVIOLI")
            {
                string text = RandomDialogue(_dialoguesPizzaHit);
                Enable(text);
                OpenDialogue();
            }*/

            _audio.clip = _audioManager._playlistWinNerd[0];
            _audio.Play();

            string text = RandomDialogue(_dialoguesCowHit);
            Enable(text);
            OpenDialogue();
        }
    }

    /* private List<string> SelectList(int i)
    {
        List<string> list = new List<string>();

        switch (i)
        {
            case 1:
                if ()
                {
                    list = _dialoguesCowHit;
                }
                else if ()
                {
                    list = _dialoguesCowAttack;
                }
                else if ()
                {
                    list = _dialoguesCowVictory;
                }
                break;
            case 2:
                if ()
                {
                    list = _dialoguesNerdHit;
                }
                else if ()
                {
                    list = _dialoguesNerdAttack;
                }
                else if ()
                {
                    list = _dialoguesNerdVictory;
                }
                break;
            case 3:
                if ()
                {
                    list = _dialoguesPizzaHit;
                }
                else if ()
                {
                    list = _dialoguesPizzaAttack;
                }
                else if ()
                {
                    list = _dialoguesPizzaVictory;
                }
                break;
        }

        return list;
    } */

    private void OpenDialogue()
    {
        _animator.SetBool("Close", false);
        _animator.SetTrigger("Open");
    }

    private void Enable(string text)
    {
        gameObject.name = text;
        _lines = _xmlReader.GetText(text);
        _dialogue.text = string.Empty;
        _isPlayed = true;

        //SeparateString(text);
        _index = 0;
        StartCoroutine(TypeLine());
    }

    private void Disable()
    {
        _index = 0;
        _dialogue.text = string.Empty;
        _isPlayed = false;
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

    #region Random text dialogue
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

    private AudioClip RandomVoice(AudioClip[] listClip)
    {
        AudioClip clip;
        int index;
        int lastIndex = 0;

        do
        {
            index = UnityEngine.Random.Range(0, listClip.Length);
        } while (index == lastIndex);

        lastIndex = index;
        clip = listClip[index];
        return clip;
    }
    #endregion

    #region Find lists
    private List<string> FindListDialogueWin()
    {
        List<string> list;

        if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. COWBOY")
        {
            list = _dialoguesCowVictory;
            return list;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. NERD")
        {
            list = _dialoguesNerdVictory;
            return list;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. RAVIOLI")
        {
            list = _dialoguesPizzaVictory;
            return list;
        }
        else
        {
            Debug.Log("Pas de list choisi");
            return list = null;
        }
    }

    private List<string> FindListDialogueHit()
    {
        List<string> list;

        if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. COWBOY")
        {
            list = _dialoguesCowHit;
            return list;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. NERD")
        {
            list = _dialoguesNerdHit;
            return list;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. RAVIOLI")
        {
            list = _dialoguesPizzaHit;
            return list;
        }
        else
        {
            Debug.Log("Pas de list choisi");
            return list = null;
        }
    }

    private List<string> FindListDialogueAttack()
    {
        List<string> list;

        if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. COWBOY")
        {
            list = _dialoguesCowAttack;
            return list;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. NERD")
        {
            list = _dialoguesNerdAttack;
            return list;
        }
        else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. RAVIOLI")
        {
            list = _dialoguesPizzaAttack;
            return list;
        }
        else
        {
            Debug.Log("Pas de list choisi");
            return list = null;
        }
    }
    #endregion

}
