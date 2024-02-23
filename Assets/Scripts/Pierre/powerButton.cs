using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class powerButton : MonoBehaviour
{
    [SerializeField] private scriptablePower _scriptable;
    private GameObject _description;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _powerNeed;

    private void Start()
    {
        _description = GameObject.FindGameObjectWithTag("Description");
        _description = _description.transform.Find("content").gameObject;

        _icon.sprite = _scriptable._icon;
        _powerNeed.text = _scriptable._powerNeed.ToString();
    }

    public void OpenDescription()
    {
        _description.SetActive(true);
        _description.GetComponent<popUp>().OpenDesc(_scriptable);
    }
}
