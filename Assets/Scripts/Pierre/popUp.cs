using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class popUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _powerNeed;
    [SerializeField] private Image _icon;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenDesc(scriptablePower scriptable)
    {
        _description.text = scriptable.Description;
        _name.text = scriptable.AbilityName;
        _powerNeed.text = scriptable.ActionPointsNeeded.ToString();
        _icon.sprite = scriptable.Icon;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
