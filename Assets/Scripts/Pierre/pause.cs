using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour
{
    [SerializeField] private GameObject _pausePage;

    public void OpenPage()
    {
        _pausePage.SetActive(true);
    }

    public void ClosePage()
    {
        _pausePage.SetActive(false);
    }
}
