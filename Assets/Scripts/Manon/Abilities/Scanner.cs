using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    // ----- FIELDs ----- //
    private scriptablePower _ability;
    private AbilityButton _abilityButton;

    private Tile _target;
    // ----- FIELDS ----- //

    private void Start()
    {
        _abilityButton = GetComponent<AbilityButton>();
        _ability = _abilityButton.GetAbility();
    }

    public void TryScanner()
    {
        Debug.Log("try scanner");
        _target = GameManager.instance.TargetOnTile;

        if (_target == null)
        {
            return;
        }

        if (!_abilityButton.IsSelected)
        {
            AbilityButtonsManager.instance.SelectAbilityButton(_abilityButton);
            return;
        }
        else
        {
            if (GameManager.instance.CanUseAbility(_ability))
            {
                _abilityButton.SetCooldown();;

                // Top
                bool canGoTop = true;
                Tile currentTile = _target;
                while (canGoTop)
                {
                    if (currentTile.TopTile != null)
                    {
                        if (currentTile.TopTile.IsOccupied)
                        {
                            currentTile.TopTile.IsReavealed = true;
                            UpdateHiddenRooms();
                        }
                        else
                        {
                            currentTile.TopTile.IsMissed = true;
                        }

                        currentTile = currentTile.TopTile;
                    }
                    else
                    {
                        canGoTop = false;
                    }
                }


                // Bottom
                bool canGoBottom = true;
                currentTile = _target;
                while (canGoBottom)
                {
                    if (currentTile.BottomTile != null)
                    {
                        if (currentTile.BottomTile.IsOccupied)
                        {
                            currentTile.BottomTile.IsReavealed = true;
                            UpdateHiddenRooms();
                        }
                        else
                        {
                            currentTile.BottomTile.IsMissed = true;
                        }

                        currentTile = currentTile.BottomTile;
                    }
                    else
                    {
                        canGoBottom = false;
                    }
                }

                // Center
                _target.IsReavealed = true;

                if (_target.IsOccupied)
                {
                    UpdateHiddenRooms();

                    UIManager.instance.ShowFicheRoom(GameManager.instance.TargetOnTile.Room.RoomData);
                }
                else
                {
                    GameManager.instance.TargetOnTile.IsMissed = true;

                    UIManager.instance.HideFicheRoom();
                }

                TargetController.instance.ChangeTargetColorToWhite();
                UIManager.instance.CheckAbilityButtonsColor();
            }
        }
    }

    private void UpdateHiddenRooms()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            GameManager.instance.ShowOnlyDestroyedAndReavealedRooms(Player.Player2);
        }
        else
        {
            GameManager.instance.ShowOnlyDestroyedAndReavealedRooms(Player.Player1);
        }
    }
}
