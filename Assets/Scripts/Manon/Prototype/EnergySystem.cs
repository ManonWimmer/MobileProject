using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static EnergySystem instance;
    [SerializeField] int _startEnergy = 10;
    [SerializeField] int _maxEnergy = 10;
    [SerializeField] int _tempGainEnergyPerTurn = 3;

    private int _player1Energy;
    private int _player2Energy;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _player1Energy = _startEnergy;
        _player2Energy = _startEnergy;
    }

    public int GetMaxEnergy()
    {
        return _maxEnergy;
    }

    public int GetStartEnergy()
    {
        return _startEnergy;
    }

    public int GetPlayerEnergy(Player player)
    {
        if (player == Player.Player1)
        {
            return _player1Energy;
        }
        else
        {
            return _player2Energy;
        }
    }

    public void GetRoundEnergy(Player player)
    {
        Debug.Log("Get round energy");

        if (player == Player.Player1)
        {
            _player1Energy += _tempGainEnergyPerTurn; // plus tard -> nbr générateurs ?
            _player1Energy = Mathf.Clamp(_player1Energy, 0, _maxEnergy);

            UIManager.instance.UpdateEnergySlider(player);
        }
        else
        {
            _player2Energy += _tempGainEnergyPerTurn; // plus tard -> nbr générateurs ?
            _player2Energy = Mathf.Clamp(_player2Energy, 0, _maxEnergy);

            UIManager.instance.UpdateEnergySlider(player);
        }
    }

    public bool TryUseEnergy(Player player, int energyAmount)
    {
        Debug.Log("Try use energy");

        if (player == Player.Player1)
        {
            if (_player1Energy - energyAmount >= 0)
            {
                _player1Energy -= energyAmount;
                UIManager.instance.UpdateEnergySlider(player);
                return true;
            }
        }
        else
        {
            if (_player2Energy - energyAmount >= 0)
            {
                _player2Energy -= energyAmount;
                UIManager.instance.UpdateEnergySlider(player);
                return true;
            }
        }

        return false;
    }
}
