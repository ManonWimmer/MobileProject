using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SimpleReveal : MonoBehaviour
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

    public void TrySimpleReveal()
    {
        Debug.Log("try simple reveal");
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
                _abilityButton.SetCooldown();

                AbilityButtonsManager.instance.DesactivateSimpleHitX2IfActivated();

                if (GameManager.instance.TargetOnTile.IsOccupied)
                {
                    Debug.Log("reveal room " + GameManager.instance.TargetOnTile.Room.name);
                    GameManager.instance.TargetOnTile.IsReavealed = true;

                    GameManager.instance.CheckIfTargetRoomIsCompletelyDestroyed();

                    // update hidden rooms
                    if (GameManager.instance.PlayerTurn == Player.Player1)
                    {
                        GameManager.instance.ShowOnlyDestroyedAndReavealedRooms(Player.Player2);
                    }
                    else
                    {
                        GameManager.instance.ShowOnlyDestroyedAndReavealedRooms(Player.Player1);
                    }

                    UIManager.instance.ShowFicheRoom(GameManager.instance.TargetOnTile.Room.RoomData);
                }
                else
                {
                    GameManager.instance.TargetOnTile.IsMissed = true;
                    Debug.Log("no room on hit");

                    UIManager.instance.HideFicheRoom();
                }

                TargetController.instance.ChangeTargetColorToRed();
            }
        }
    }
}
