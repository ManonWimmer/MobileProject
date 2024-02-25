using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftManagerUI : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static DraftManagerUI instance;

    [SerializeField] GameObject _draftGameObjet;

    [SerializeField] DraftRoom _draftRoom0;
    [SerializeField] DraftRoom _draftRoom1;
    [SerializeField] DraftRoom _draftRoom2;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    public void ShowDraftUI()
    {
        _draftGameObjet.SetActive(true);
        UIManager.instance.HideGameCanvas();
    }

    public void HideDraftUI()
    {
        _draftGameObjet.SetActive(false);
        UIManager.instance.ShowGameCanvas();
    }

    public void InitDraftRoom(int index, Room room)
    {
        if (index == 0)
        {
            _draftRoom0.InitDraftRoom(room);
        }
        else if (index == 1)
        {
            _draftRoom1.InitDraftRoom(room);
        }
        else if (index == 2)
        {
            _draftRoom2.InitDraftRoom(room);
        }
        else
        {
            Debug.Log("ERREUR D'INDEX");
        }
    }
}
