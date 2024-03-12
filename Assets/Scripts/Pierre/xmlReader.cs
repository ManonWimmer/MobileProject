using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

[System.Serializable]

public class LanguageData
{
    public string name;
    public Dictionary<string, string> texts = new Dictionary<string, string>();
}

public class xmlReader : MonoBehaviour
{

    [SerializeField] private TextAsset _dictionary;
    [SerializeField] private TMP_Dropdown _dropdown1;
    [SerializeField] private TMP_Dropdown _dropdown2;
    [SerializeField] private List<TextMeshProUGUI> _textFields = new List<TextMeshProUGUI>();

    private List<LanguageData> _languages = new List<LanguageData>();
    private int _currentLanguageIndex = 0;

    public void ChangeText(TextMeshProUGUI textMesh, string newName) => UpdateTextTranslation(textMesh, newName);
    public string GetText(string name) => _languages[_currentLanguageIndex].texts[name];
    public int GetLanguage() => _currentLanguageIndex;

    private void Awake()
    {
        LoadLanguages();
        UpdateTexts();
    }

    private void LoadLanguages()
    {
        _languages.Clear();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(_dictionary.text);
        XmlNodeList languageList = xmlDoc.GetElementsByTagName("language");

        foreach (XmlNode languageNode in languageList)
        {
            LanguageData language = new LanguageData();
            language.name = languageNode.SelectSingleNode("Name").InnerText;

            foreach (XmlNode textFieldNode in languageNode.ChildNodes)
            {
                if (textFieldNode.Name != "Name" && textFieldNode.NodeType == XmlNodeType.Element)
                {
                    string fieldName = textFieldNode.Name;
                    string fieldValue = textFieldNode.InnerText;
                    language.texts.Add(fieldName, fieldValue);
                }
            }

            _languages.Add(language);
        }
    }

    private void UpdateTexts()
    {
        foreach (var textField in _textFields)
        {
            string fieldName = textField.name;
            textField.text = _languages[_currentLanguageIndex].texts[fieldName];
        }
    }

    public void OnLanguageChange()
    {
        if (_dropdown1.gameObject.activeInHierarchy && _dropdown1.IsActive())
        {
            _dropdown2.value = _dropdown1.value;
            _currentLanguageIndex = _dropdown1.value;
            UpdateTexts();
        }
        else if (_dropdown2.gameObject.activeInHierarchy && _dropdown2.IsActive())
        {
            _dropdown1.value = _dropdown2.value;
            _currentLanguageIndex = _dropdown2.value;
            UpdateTexts();
        }

        Debug.Log("Langue sélectionnée : " + name + _currentLanguageIndex);
    }

    private void UpdateTextTranslation(TextMeshProUGUI textField, string newValue)
    {
        string fieldName = textField.name;
        textField.name = newValue;
        UpdateTexts();
    }
}
