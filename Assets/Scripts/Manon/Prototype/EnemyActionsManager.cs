using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionsManager : MonoBehaviour
{
    // ----- FIELDS ----- // 
    public static EnemyActionsManager instance;

    [SerializeField] List<EnemyAction> _enemyActions = new List<EnemyAction>();
    // ----- FIELDS ----- // 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        HideAllEnemyActions();
    }

    public void HideAllEnemyActions()
    {
        foreach(EnemyAction enemyAction in _enemyActions)
        {
            enemyAction.HideEnemyAction();
        }
    }

    public void InitEnemyActions(List<string> actionNames)
    {
        Debug.Log("init enemy actions");

        int i = 0;
        foreach(string actionName in actionNames)
        {
            scriptablePower ability = GameManager.instance.GetAbilityFromName(actionName);
            if (ability != null)
            {
                _enemyActions[i].InitEnemyAction(ability);
            }
            else
            {
                Debug.Log("ERROR : ABILITY NULL");
                return;
            }
            i++;
        }

        if (i == 0)
        {
            _enemyActions[0].HideEnemyAction();
            _enemyActions[1].HideEnemyAction();
            _enemyActions[2].HideEnemyAction();
        }
        else if (i == 1)
        {
            _enemyActions[1].HideEnemyAction();
            _enemyActions[2].HideEnemyAction();
        }
        else if (i == 2)
        {
            _enemyActions[2].HideEnemyAction();
        }
    }

}
