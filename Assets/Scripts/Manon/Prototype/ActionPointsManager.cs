using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ActionPointsManager : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static ActionPointsManager instance;

    private int _player1ActionPoints;
    private int _player2ActionPoints;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    public void InitRoundActionPoints(int round) 
    {
        int actionPoints = (1 + round / 3);

        _player1ActionPoints = actionPoints;
        _player2ActionPoints = actionPoints;

        UIManager.instance.ShowOrUpdateActionPoints();
    }

    public int GetPlayerActionPoints(Player player)
    {
        if (player == Player.Player1)
        {
            return _player1ActionPoints;
        }
        else
        {
            return _player2ActionPoints;
        }
    }

    public bool TryUseActionPoints(Player player)
    {
        Debug.Log("Try use action points");
        if (player == Player.Player1)
        {
            if (_player1ActionPoints > 0)
            {
                UIManager.instance.ShowOrUpdateActionPoints();
                return true;
            }
        }
        else
        {
            if (_player2ActionPoints > 0)
            {
                _player2ActionPoints -= 1;
                UIManager.instance.ShowOrUpdateActionPoints();
                return true;
            }

        }
        return false;
    }

    public void UseActionPoint(Player player)
    {
        if (player == Player.Player1)
        {
            _player1ActionPoints -= 1;
        }
        else
        {
            _player2ActionPoints -= 1;

        }

        UIManager.instance.ShowOrUpdateActionPoints();
    }
}
