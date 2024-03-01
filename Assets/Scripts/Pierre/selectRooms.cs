using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class selectRooms : MonoBehaviour
{

    private List<selectRooms> _cards = new List<selectRooms>();

    private bool _selected;
    private Animator _animator;

    public bool GetSelected() => _selected;
    public void DisabledSelected() => UnSelected();

    private void Start()
    {
        _animator = GetComponent<Animator>();
        AddList();
    }

    #region ListButtons
    private void AddList()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Cards");

        foreach (GameObject card in cards)
        {
            if (card != gameObject)
            {
                selectRooms cardScript = card.GetComponent<selectRooms>();
                if (cardScript != null)
                {
                    _cards.Add(cardScript);
                }
            }
        }
    }

    private void CheckSelected()
    {
        foreach (selectRooms card in _cards)
        {
            if (card.GetSelected())
            {
                card.DisabledSelected();
            }
        }
    }
    #endregion

    private void UnSelected()
    {
        if (_selected)
        {
            _selected = false;
            _animator.SetBool("Unselected", true);
        }
    }

    public void Click()
    {
        CheckSelected();
        _animator.enabled = true;
        _animator.SetTrigger("Selected");
        _animator.SetBool("Unselected", false);
        _selected = true;
    }
}
