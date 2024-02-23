using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class popUp : MonoBehaviour
{
    private GameObject _descriptionPage;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _powerNeed;
    [SerializeField] private Image _icon;

    private void Start()
    {
        _descriptionPage = GetComponent<GameObject>();
        gameObject.SetActive(false);
    }

    public void OpenDesc(scriptablePower scriptable)
    {
        _description.text = scriptable._description;
        _name.text = scriptable._powerName;
        _powerNeed.text = scriptable._powerNeed.ToString();
        _icon.sprite = scriptable._icon;
    }
}
