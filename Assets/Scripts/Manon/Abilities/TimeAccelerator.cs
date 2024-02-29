using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class TimeAccelerator : MonoBehaviour
{
    // ----- FIELDs ----- //
    private scriptablePower _ability;
    private AbilityButton _abilityButton;
    // ----- FIELDS ----- //

    private void Start()
    {
        _abilityButton = GetComponent<AbilityButton>();
        _ability = _abilityButton.GetAbility();
    }

    public void TryTimeAccelerator()
    {
        Debug.Log("try time accelerator");

        //if (GameManager.instance.CanUseAbility(_ability)) on s'en fout d'ou est la target
        if (ActionPointsManager.instance.TryUseActionPoints(GameManager.instance.PlayerTurn) && !GameManager.instance.IsAbilityInCooldown(_ability))
        {
            ActionPointsManager.instance.UseActionPoint(GameManager.instance.PlayerTurn);
            _abilityButton.SetCooldown();

            /* Pas de destroy à la target
            if (GameManager.instance.TargetOnTile.IsOccupied)
            {
                Debug.Log("hit room " + GameManager.instance.TargetOnTile.Room.name);
                GameManager.instance.TargetOnTile.RoomTileSpriteRenderer.color = Color.black;
                GameManager.instance.TargetOnTile.IsDestroyed = true;

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
            */
            UIManager.instance.CheckAbilityButtonsColor();

            // All player cooldown - 1
            GameManager.instance.CurrentPlayerLessCooldown(1);
        }
    }
}
