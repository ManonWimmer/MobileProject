using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private AlternateShotDirection _currentAlternateShotDirection;
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

                AbilityButtonsManager.instance.DesactivateSimpleHitX2IfActivated();

                GetCurrentPlayerAlternateShotDirection();

                if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
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

    private void GetCurrentPlayerAlternateShotDirection()
    {
        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            _currentAlternateShotDirection = AbilityButtonsManager.instance.CurrentAlternateShotDirectionPlayer1;
        }
        else
        {
            _currentAlternateShotDirection = AbilityButtonsManager.instance.CurrentAlternateShotDirectionPlayer2;
        }
    }

    private void TryDestroyRoom(Tile tile)
    {
        Debug.Log("destroy room " + tile.name);
        if (tile.IsOccupied && !tile.Room.IsRoomDestroyed)
        {
            Debug.Log("hit room " + tile.Room.name);
            tile.RoomTileSpriteRenderer.color = Color.black;
            tile.IsDestroyed = true;

            GameManager.instance.CheckIfTileRoomIsCompletelyDestroyed(tile);

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
        GetCurrentPlayerAlternateShotDirection();

        if (GameManager.instance.PlayerTurn == Player.Player1)
        {
            if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
            {
                _currentAlternateShotDirection = AlternateShotDirection.Vertical;
            }
            else
            {
                _currentAlternateShotDirection = AlternateShotDirection.Horizontal;
            }

            AbilityButtonsManager.instance.CurrentAlternateShotDirectionPlayer1 = _currentAlternateShotDirection;
        }
        else
        {
            if (_currentAlternateShotDirection == AlternateShotDirection.Horizontal)
            {
                _currentAlternateShotDirection = AlternateShotDirection.Vertical;
            }
            else
            {
                _currentAlternateShotDirection = AlternateShotDirection.Horizontal;
            }

            AbilityButtonsManager.instance.CurrentAlternateShotDirectionPlayer2 = _currentAlternateShotDirection;
        }

        UIManager.instance.CheckAlternateShotDirectionImgRotation();
    }
}
