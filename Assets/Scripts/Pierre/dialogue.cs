using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using TMPro;
using UnityEngine;

public class dialogue : MonoBehaviour
{
    public static dialogue instance;
    private Animator _animator;
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private float _timeDialogue;
    [SerializeField] private bool _dontTimeDialogue;

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

    [Header("Tuto")]
    [SerializeField] private List<string> _dialoguesTuto = new List<string>();

    [SerializeField] float _textSpeed;
    private string _lines;
    private int _index;
    private bool _isWriting = false;

    private int _lastIndex = -1;
    private int _lastIndexVoice = -1;
    private bool _isPlayed;

    private xmlReader _xmlReader;
    [SerializeField] private GameManager _gameManager;

    [SerializeField] private AudioSource _audio;
    private audioManager _audioManager;

    public bool GetDilogueIsPlayed() => _isPlayed;
    public bool DisablePlayed() => _isPlayed = false;
    public int GetLenghDialogue(string name) => GetListLengh(name);

    public void SartDialogueHit() => SetDialogueHit();
    public void SartDialogueAttack() => SetDialogueAttack();
    public void SartDialogueWin() => SetDialogueWin();
    public void StartDialogueTuto(int i) => StetDialogueTuto(i);

    public void CloseDialogue() => Disable();


    private void Awake()
    {
        instance = this;

        _dialogue = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
        _isPlayed = false;
    }

    private void Start()
    {
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();
        _xmlReader = GameObject.FindGameObjectWithTag("Translate").GetComponent<xmlReader>();
    }

    private int GetListLengh(string name)
    {
        List<string> list = new List<string>();
        int i;

        if (name == "_dialoguesNerdHit")
        {
            list = _dialoguesNerdHit;
        }
        else if (name == "_dialoguesCowHit")
        {
            list = _dialoguesCowHit;
        }
        else if (name == "_dialoguesPizzaHit")
        {
            list = _dialoguesPizzaHit;
        }
        else if (name == "_dialoguesNerdAttack")
        {
            list = _dialoguesNerdAttack;
        }
        else if (name == "_dialoguesCowAttack")
        {
            list = _dialoguesCowAttack;
        }
        else if (name == "_dialoguesPizzaAttack")
        {
            list = _dialoguesPizzaAttack;
        }
        else if (name == "_dialoguesNerdVictory")
        {
            list = _dialoguesNerdVictory;
        }
        else if (name == "_dialoguesCowVictory")
        {
            list = _dialoguesCowVictory;
        }
        else if (name == "_dialoguesPizzaVictory")
        {
            list = _dialoguesPizzaVictory;
        }
        else if (name == "_dialoguesTuto")
        {
            list = _dialoguesTuto;
        }

        i = list.Count;
        return i;
    }

    #region Tuto dialogue
    private void StetDialogueTuto(int index)
    {
        if (index == 0)
        {
            Enable(_dialoguesTuto[index]);
            OpenDialogue();
        }
        else if (index == 1)
        {
            Enable(_dialoguesTuto[index]);
            OpenDialogue();
        }
        else if (index == 2)
        {
            Enable(_dialoguesTuto[index]);
            OpenDialogue();
        }
        else if (index == 3)
        {
            Enable(_dialoguesTuto[index]);
            OpenDialogue();
        }
        else if (index == 4)
        {
            Enable(_dialoguesTuto[index]);
            OpenDialogue();
        }
        else if (index == 5)
        {
            Enable(_dialoguesTuto[index]);
            OpenDialogue();
        }
        else if (index == 6)
        {
            Enable(_dialoguesTuto[index]);
            OpenDialogue();
        }
        else if (index == 7)
        {
            Enable(_dialoguesTuto[index]);
            OpenDialogue();
        }
    }
    #endregion

    #region Start dialogues
    private void SetDialogueHit()
    {
        if (!_isPlayed)
        {
            _audio.clip = RandomVoice(_audioManager.GetPlayListDialogueHit());
            _audio.Play();
            string text = RandomDialogue(FindListDialogueHit());
            Enable(text);
            OpenDialogue();
        }
        else
        {
            Disable();
            SetDialogueHit();
        }
    }

    private void SetDialogueAttack()
    {
        if (!_isPlayed)
        {
            _audio.clip = RandomVoice(_audioManager.GetPlayListDialogueAttack());
            _audio.Play();
            string text = RandomDialogue(FindListDialogueAttack());
            Enable(text);
            OpenDialogue();
        }
        else
        {
            Disable();
            SetDialogueAttack();
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
        else
        {
            Disable();
            SetDialogueWin();
        }
    }

    #endregion

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

    #region open & start & close dialogues
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
        _isPlayed = false;
        _animator.SetBool("Close", true);

        _index = 0;
        _dialogue.text = string.Empty;

        _audio.Stop();
        StopAllCoroutines();
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

        _isPlayed = false;
        if (!_dontTimeDialogue) 
        {
            StartCoroutine(TimeDialogue());
        }

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
    }
    #endregion


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
        if (listDialogue.Count > 1)
        {
            string dialogue;
            int index;

            do
            {
                index = UnityEngine.Random.Range(0, listDialogue.Count);
            } while (index == _lastIndex);

            _lastIndex = index;
            dialogue = listDialogue[index];
            return dialogue;
        }
        else
        {
            return listDialogue[0];
        }
    }

    private AudioClip RandomVoice(AudioClip[] listClip)
    {
        if (listClip.Length > 1)
        {
            AudioClip clip;
            int index;

            do
            {
                index = UnityEngine.Random.Range(0, listClip.Length);
            } while (index == _lastIndexVoice);

            _lastIndexVoice = index;
            clip = listClip[index];
            return clip;
        }
        else
        {
            return listClip[0];
        }
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
