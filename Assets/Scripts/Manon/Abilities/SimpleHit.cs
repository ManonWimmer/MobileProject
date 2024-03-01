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
                    UIManager.instance.ShowFicheRoom(GameManager.instance.TargetOnTile.Room.RoomData);
                }  
                else
                {
                    GameManager.instance.TargetOnTile.IsMissed = true;
                    Debug.Log("no room on hit");

                    UIManager.instance.HideFicheRoom();
                }

                TargetController.instance.ChangeTargetColorToRed();

                if (AbilityButtonsManager.instance.GetIfSimpleHitXS())
                {
                    Debug.Log("simple hit x2");

                    // Try destroy right
                    if (_target.RightTile != null)
                    {
                        if (_target.RightTile.IsOccupied)
                        {
                            _target.RightTile.RoomTileSpriteRenderer.color = Color.black;
                            _target.RightTile.IsDestroyed = true;
                        }
                        else
                        {
                            _target.RightTile.IsMissed = true;
                        }
                    }

                    // Try destroy bottom
                    if (_target.BottomTile != null)
                    {
                        if (_target.BottomTile.IsOccupied)
                        {
                            _target.BottomTile.RoomTileSpriteRenderer.color = Color.black;
                            _target.BottomTile.IsDestroyed = true;
                        }
                        else
                        {
                            _target.BottomTile.IsMissed = true;
                        }
                    }

                    // Try destroy diag bottom right
                    if (_target.DiagBottomRightTile != null)
                    {
                        if (_target.DiagBottomRightTile.IsOccupied)
                        {
                            _target.DiagBottomRightTile.RoomTileSpriteRenderer.color = Color.black;
                            _target.DiagBottomRightTile.IsDestroyed = true;
                        }
                        else
                        {
                            _target.DiagBottomRightTile.IsMissed = true;
                        }
                    }

                    AbilityButtonsManager.instance.DesactivateSimpleHitX2IfActivated();
                }

                // update hidden rooms
                if (GameManager.instance.PlayerTurn == Player.Player1)
                {
                    GameManager.instance.ShowOnlyDestroyedAndReavealedRooms(Player.Player2);
                }
                else
                {
                    GameManager.instance.ShowOnlyDestroyedAndReavealedRooms(Player.Player1);
                }

                GameManager.instance.CheckTileClickedInCombat(_target);
                TargetController.instance.ChangeTargetColorToRed();
                UIManager.instance.CheckAbilityButtonsColor();
            }
        } 
    } 
}
