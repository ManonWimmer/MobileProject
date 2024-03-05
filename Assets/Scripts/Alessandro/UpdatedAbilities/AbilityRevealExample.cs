using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityRevealExample : AAbility
{
    private void Awake()
    {
        AbilityID = 1;
    }

    override public void AbilityEffect()
    {
        Debug.Log("try simple reveal");

        if (GameManager.instance.CanUseAbility(_ability))
        {
            _abilityButton.SetCooldown();

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
