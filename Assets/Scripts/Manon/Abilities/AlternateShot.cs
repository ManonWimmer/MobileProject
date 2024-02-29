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

    private AlternateShotDirection _currentAlternateShotDirection = AlternateShotDirection.Horizontal;
    // ----- FIELDS ----- //

    private void Start()
    {
        _abilityButton = GetComponent<AbilityButton>();
        _ability = _abilityButton.GetAbility();
    }

    public void TryAlternateShot()
    {
        Debug.Log("try simple hit");

        if (GameManager.instance.CanUseAbility(_ability))
        {
            _abilityButton.SetCooldown();

            Tile target = GameManager.instance.TargetOnTile;

            if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
            {
                if (target.LeftTile != null)
                {
                    TryDestroyRoom(target.LeftTile);
                }

                if (target.RightTile != null)
                {
                    TryDestroyRoom(target.RightTile);
                }
            }
            else
            {
                if (target.TopTile != null)
                {
                    TryDestroyRoom(target.TopTile);
                }

                if (target.BottomTile != null)
                {
                    TryDestroyRoom(target.BottomTile);
                }
            }

            TryDestroyRoom(target);

            TargetController.instance.ChangeTargetColorToRed();
            ChangeAlternateShotDirection();
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
        if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
        {
            _currentAlternateShotDirection = AlternateShotDirection.Vertical;
        }
        else
        {
            _currentAlternateShotDirection = AlternateShotDirection.Horizontal;
        }
    }
}
