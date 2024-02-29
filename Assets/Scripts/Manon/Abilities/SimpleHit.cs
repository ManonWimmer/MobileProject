using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHit : MonoBehaviour
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

    public void TrySimpleHit()
    {
        Debug.Log("try simple hit");
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
            }
        } 
    } 
}
