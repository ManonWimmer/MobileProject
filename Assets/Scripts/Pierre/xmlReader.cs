using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class xmlReader : MonoBehaviour
{

    [SerializeField] private TextAsset _dictionary;

    [SerializeField] private string _languageName;
    [SerializeField] private int _currentLanguage;

    private string _bonjour;
    private string _aurevoir;

    [SerializeField] private TextMeshProUGUI _textBonjour;
    [SerializeField] private TextMeshProUGUI _textAurevoir;
    [SerializeField] private TMP_Dropdown _dropdown;

    private List<Dictionary<string, string>> _languages = new List<Dictionary<string, string>>();
    private Dictionary<string, string> _obj;

    private void Awake()
    {
        Reader();
    }

    // Update is called once per frame
    void Update()
    {
        _languages[_currentLanguage].TryGetValue("Name", out _languageName);
        _languages[_currentLanguage].TryGetValue("Bonjour", out _bonjour);
        _languages[_currentLanguage].TryGetValue("Aurevoir", out _aurevoir);

        _textBonjour.text = _bonjour;
        _textAurevoir.text = _aurevoir;

    }

    private void Reader()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_dictionary.text);
        XmlNodeList languageList = xmlDoc.GetElementsByTagName("language");

        foreach (XmlNode languageValue in languageList)
        {
            XmlNodeList languageContent = languageValue.ChildNodes;
            _obj = new Dictionary<string, string>();

            foreach (XmlNode value in languageContent)
            {
                if (value.Name == "Name")
                {
                    _obj.Add(value.Name, value.InnerText);
                }

                if (value.Name == "Bonjour")
                {
                    _obj.Add(value.Name, value.InnerText);
                }
                if (value.Name == "Aurevoir")
                {
                    _obj.Add(value.Name, value.InnerText);
                }
            }
            _languages.Add(_obj);
        }
    }

    public void ValueChangeCheck()
    {
        _currentLanguage = _dropdown.value;
    }
}
