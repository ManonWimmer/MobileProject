using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class dialogue : MonoBehaviour
{
    public static dialogue instance;
    private Animator _animator;
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private float _timeDialogueHide;
    [SerializeField] float _textSpeed;
    [SerializeField] private bool _dontTimeDialogueHide;

    [Header("IMG Capt")]
    [SerializeField] private List<Sprite> _imgCapt = new List<Sprite>();

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

    private string _lines;
    private bool _isDialogueActive = false;

    private int _lastIndex = -1;
    private int _lastIndexVoice = -1;
    private bool _isPlayed;
    private int _indexText;
    private bool _capt = false;
    [SerializeField] private bool _winDialogue;

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioSource _audio;
    private audioManager _audioManager;
    private xmlReader _xmlReader;

    private Coroutine _coroutine;

    public bool GetDilogueIsPlayed() => _isPlayed;
    public bool DisablePlayed() => _isPlayed = false;
    public int GetLenghDialogue(string name) => GetListLengh(name);

    public void SartDialogueHit() => SetDialogueHit();
    public void SartDialogueAttack() => SetDialogueAttack();
    public void SartDialogueWin() => SetDialogueWin();
    public void StartDialogueTuto(int i) => StetDialogueTuto(i);

    public void CloseDialogue() => Disable();

    public void ChangeImgCapt() => UIManager.instance.ChangeImgCapt(FindListSprite());


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
            if (_audioManager != null) 
            { 
                Disable();
                string text = RandomDialogue(FindListDialogueHit());
                Enable(text);
                _audio.clip = _audioManager.GetPlayListDialogueHit()[_indexText];
                _audio.Play();
                OpenDialogue();
            }
            else
            {
                Debug.LogWarning("AudioManager not set.");
                _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();
                SetDialogueHit();
            }
        }

    }

    private void SetDialogueAttack()
    {
        if (!_isPlayed)
        {
            if (_audioManager != null)
            {
                Disable();
                string text = RandomDialogue(FindListDialogueAttack());
                Enable(text);
                _audio.clip = _audioManager.GetPlayListDialogueAttack()[_indexText];
                _audio.Play();
                OpenDialogue();
            }
            else
            {
                Debug.LogWarning("AudioManager not set.");
                _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();
                SetDialogueAttack();
            }
        }
    }

    private void SetDialogueWin()
    {
        if (!_isPlayed)
        {
            if (_audioManager != null)
            {
                Disable();
                string text = RandomDialogue(FindListDialogueWin());
                Enable(text);
                _audio.clip = _audioManager.GetPlayListDialogueWin()[_indexText];
                _audio.Play();
                OpenDialogue();
            }
            else
            {
                Debug.LogWarning("AudioManager not set.");
                _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();
                SetDialogueWin();
            }
        }
    }

    #endregion

    #region open & start & close dialogues
    private void OpenDialogue()
    {
        if (!_winDialogue)
        {
            _animator.SetBool("Close", false);
            _animator.SetTrigger("Open");
        }
        else if (_winDialogue)
        {
            _animator.SetBool("Close", false);
            _animator.SetTrigger("OpenWin");
        }
    }

    private void Enable(string text)
    {
        if (_xmlReader != null)
        {
            gameObject.name = text;
            _lines = _xmlReader.GetText(text);
            _dialogue.text = string.Empty;
            _isPlayed = true;

            //SeparateString(text);
            _coroutine = StartCoroutine(TypeLine());
        }
        else
        {
            Debug.LogWarning("XMLReader is not initialized!");
            _xmlReader = GameObject.FindGameObjectWithTag("Translate").GetComponent<xmlReader>();
            Enable(text);
        }
    }

    private void Disable()
    {
        _isPlayed = false;
        _animator.SetBool("Close", true);

        _dialogue.text = string.Empty;

        _audio.Stop();
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _isDialogueActive = false;
    }

    private IEnumerator TypeLine()
    {
        _isDialogueActive = true;
        foreach (char c in _lines)
        {
            _dialogue.text += c;
            yield return new WaitForSeconds(_textSpeed);
        }

        _isPlayed = false;
        if (!_dontTimeDialogueHide && _isDialogueActive)
        {
            _coroutine = StartCoroutine(TimeDialogue());
        }
    }

    private IEnumerator TimeDialogue()
    {
        yield return new WaitForSeconds(_timeDialogueHide);
        if (_isDialogueActive)
        {
            Disable();
        }
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
            _indexText = 0;

            do
            {
                _indexText = UnityEngine.Random.Range(0, listDialogue.Count);
            } while (_indexText == _lastIndex);

            _lastIndex = _indexText;
            dialogue = listDialogue[_indexText];
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

        if (_gameManager.GetPlayerShip(GameManager.instance.PlayerWin).ShipData.CaptainName == "CPT. COWBOY")
        {
            list = _dialoguesCowVictory;
            return list;
        }
        else if (_gameManager.GetPlayerShip(GameManager.instance.PlayerWin).ShipData.CaptainName == "CPT. NERD")
        {
            list = _dialoguesNerdVictory;
            return list;
        }
        else if (_gameManager.GetPlayerShip(GameManager.instance.PlayerWin).ShipData.CaptainName == "CPT. RAVIOLI")
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

    private Sprite FindListSprite()
    {
        Sprite list;
        if (!_capt)
        {
            if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. COWBOY")
            {
                _capt = true;
                list = _imgCapt[0];
                return list;
            }
            else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. NERD")
            {
                _capt = true;
                list = _imgCapt[1];
                return list;
            }
            else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. RAVIOLI")
            {
                _capt = true;
                list = _imgCapt[2];
                return list;
            }
            else
            {
                Debug.Log("Pas de list choisi");
                return list = null;
            }
        }
        else
        {
            if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. COWBOY")
            {
                _capt = false;
                list = _imgCapt[3];
                return list;
            }
            else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. NERD")
            {
                _capt = false;
                list = _imgCapt[4];
                return list;
            }
            else if (_gameManager.GetPlayerShip().ShipData.CaptainName == "CPT. RAVIOLI")
            {
                _capt = false;
                list = _imgCapt[5];
                return list;
            }
            else
            {
                Debug.Log("Pas de list choisi");
                return list = null;
            }
        }

    }
    #endregion

}
