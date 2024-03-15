using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
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
            string newActionName = actionName;
            if (actionName == "Simple Hit X2")
                newActionName = "Simple Hit";
            scriptablePower ability = GameManager.instance.GetAbilityFromName(newActionName);
            if (ability != null)
            {
                Debug.Log("abilit : " + ability.AbilityName);
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
            _enemyActions[3].HideEnemyAction();
        }
        else if (i == 1)
        {
            _enemyActions[1].HideEnemyAction();
            _enemyActions[2].HideEnemyAction();
            _enemyActions[3].HideEnemyAction();
        }
        else if (i == 2)
        {
            _enemyActions[2].HideEnemyAction();
            _enemyActions[3].HideEnemyAction();
        }
        else if (i == 3)
        {
            _enemyActions[3].HideEnemyAction();
        }
    }

}
