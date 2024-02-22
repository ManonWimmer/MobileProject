using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerButton : MonoBehaviour
{
    [SerializeField] private scriptablePower _scriptable;
    [SerializeField] private GameObject _description;

    private void Start()
    {
        _description = GameObject.FindGameObjectWithTag("Description");
        _description = _description.transform.Find("content").gameObject;
        //set de l'icone
        //set de l'energie
    }

    public void OpenDescription()
    {
        _description.SetActive(true);
        _description.GetComponent<popUp>().OpenDesc(_scriptable);
    }
}
