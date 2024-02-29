using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum AlternateShotDirection
{
    Horizontal, 
    Vertical
}

public class AlternateShot : MonoBehaviour
{
    // ----- FIELDS ----- //
    private scriptablePower _ability;
    private AbilityButton _abilityButton;
    private Tile _target;
    // ----- FIELDS ----- //

    private void Start()
    {
        _abilityButton = GetComponent<AbilityButton>();
        _ability = _abilityButton.GetAbility();
    }

    public void TryAlternateShot()
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

                

                if (AbilityButtonsManager.instance.CurrentAlternateShotDirection == AlternateShotDirection.Horizontal)
                {
                    if (_target.LeftTile != null)
                    {
                        TryDestroyRoom(_target.LeftTile);
                    }

                    if (_target.RightTile != null)
                    {
                        TryDestroyRoom(_target.RightTile);
                    }
                }
                else
                {
                    if (_target.TopTile != null)
                    {
                        TryDestroyRoom(_target.TopTile);
                    }

                    if (_target.BottomTile != null)
                    {
                        TryDestroyRoom(_target.BottomTile);
                    }
                }

                TryDestroyRoom(_target);

                TargetController.instance.ChangeTargetColorToRed();
                ChangeAlternateShotDirection();
            }
        }
    }

    private void TryDestroyRoom(Tile tile)
    {
        if (tile.IsOccupied)
        {
            Debug.Log("hit room " + tile.Room.name);
            tile.RoomTileSpriteRenderer.color = Color.black;
            tile.IsDestroyed = true;

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

            UIManager.instance.ShowFicheRoom(tile.Room.RoomData);
        }
        else
        {
            tile.IsMissed = true;
            Debug.Log("no room on hit " + tile.name);

            UIManager.instance.HideFicheRoom();
        }

        GameManager.instance.CheckTileClickedInCombat(tile);
    }

    private void ChangeAlternateShotDirection()
    {
        if (AbilityButtonsManager.instance.CurrentAlternateShotDirection == AlternateShotDirection.Horizontal)
        {
            AbilityButtonsManager.instance.CurrentAlternateShotDirection = AlternateShotDirection.Vertical;
        }
        else
        {
            AbilityButtonsManager.instance.CurrentAlternateShotDirection = AlternateShotDirection.Horizontal;
        }
    }
}
