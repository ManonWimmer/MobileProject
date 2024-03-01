using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityGeneric : MonoBehaviour
{
    // ----- FIELDs ----- //
    protected scriptablePower _ability;
    protected AbilityButton _abilityButton;
    // ID allows us to call all abilities from one place, in a way easier fashion
    private int abilityID = 0;

    public int AbilityID { get => abilityID; set => abilityID = value; }

    // ----- FIELDS ----- //

    private void Start()
    {
        _abilityButton = GetComponent<AbilityButton>();
        _ability = _abilityButton.GetAbility();
    }

    public bool TryUseAbility()
    {
        Debug.Log("try use abiilty");

        if (GameManager.instance.CanUseAbility(_ability)) 
        {
            _abilityButton.SetCooldown();
            AbilityEffect();
            return true;
        }
        return false;
    }

    public virtual void AbilityEffect() // this code is what should be overriden, depending on the effects of the abilities
    {
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
