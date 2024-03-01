using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor.Playables;
using UnityEngine;

public class EMP : MonoBehaviour
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

    public void TryEMP()
    {
        Debug.Log("try emp");
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

                // Simple hit on target (destroyed or missed)
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

                // Desactivate for one turn room's abilities around the target (+1 cooldown)
                List<Room> roomsToDesactivate = new List<Room>();

                #region Right, Left, Bottom & Top
                // Right
                if (GameManager.instance.TargetOnTile.RightTile != null)
                {
                    if (GameManager.instance.TargetOnTile.RightTile.Room != null)
                    {
                        if (!roomsToDesactivate.Contains(GameManager.instance.TargetOnTile.RightTile.Room))
                        {
                            roomsToDesactivate.Add(GameManager.instance.TargetOnTile.RightTile.Room);
                        }
                    }
                }

                // Left
                if (GameManager.instance.TargetOnTile.LeftTile != null)
                {
                    if (GameManager.instance.TargetOnTile.LeftTile.Room != null)
                    {
                        if (!roomsToDesactivate.Contains(GameManager.instance.TargetOnTile.LeftTile.Room))
                        {
                            roomsToDesactivate.Add(GameManager.instance.TargetOnTile.LeftTile.Room);
                        }
                    }
                }

                // Bottom
                if (GameManager.instance.TargetOnTile.BottomTile != null)
                {
                    if (GameManager.instance.TargetOnTile.BottomTile.Room != null)
                    {
                        if (!roomsToDesactivate.Contains(GameManager.instance.TargetOnTile.BottomTile.Room))
                        {
                            roomsToDesactivate.Add(GameManager.instance.TargetOnTile.BottomTile.Room);
                        }
                    }
                }

                // Top
                if (GameManager.instance.TargetOnTile.TopTile != null)
                {
                    if (GameManager.instance.TargetOnTile.TopTile.Room != null)
                    {
                        if (!roomsToDesactivate.Contains(GameManager.instance.TargetOnTile.TopTile.Room))
                        {
                            roomsToDesactivate.Add(GameManager.instance.TargetOnTile.TopTile.Room);
                        }
                    }
                }
                #endregion

                #region Diag Top Left & Right, Bottom Left & Right
                // Diag top left
                if (GameManager.instance.TargetOnTile.DiagTopLeftTile != null)
                {
                    if (GameManager.instance.TargetOnTile.DiagTopLeftTile.Room != null)
                    {
                        if (!roomsToDesactivate.Contains(GameManager.instance.TargetOnTile.DiagTopLeftTile.Room))
                        {
                            roomsToDesactivate.Add(GameManager.instance.TargetOnTile.DiagTopLeftTile.Room);
                        }
                    }
                }

                // Diag top right
                if (GameManager.instance.TargetOnTile.DiagTopRightTile != null)
                {
                    if (GameManager.instance.TargetOnTile.DiagTopRightTile.Room != null)
                    {
                        if (!roomsToDesactivate.Contains(GameManager.instance.TargetOnTile.DiagTopRightTile.Room))
                        {
                            roomsToDesactivate.Add(GameManager.instance.TargetOnTile.DiagTopRightTile.Room);
                        }
                    }
                }

                // Diag bottom left
                if (GameManager.instance.TargetOnTile.DiagBottomLeftTile != null)
                {
                    if (GameManager.instance.TargetOnTile.DiagBottomLeftTile.Room != null)
                    {
                        if (!roomsToDesactivate.Contains(GameManager.instance.TargetOnTile.DiagBottomLeftTile.Room))
                        {
                            roomsToDesactivate.Add(GameManager.instance.TargetOnTile.DiagBottomLeftTile.Room);
                        }
                    }
                }

                // Diag bottom right
                if (GameManager.instance.TargetOnTile.DiagBottomRightTile != null)
                {
                    if (GameManager.instance.TargetOnTile.DiagBottomRightTile.Room != null)
                    {
                        if (!roomsToDesactivate.Contains(GameManager.instance.TargetOnTile.DiagBottomRightTile.Room))
                        {
                            roomsToDesactivate.Add(GameManager.instance.TargetOnTile.DiagBottomRightTile.Room);
                        }
                    }
                }
                #endregion

                foreach (Room room in roomsToDesactivate)
                {
                    if (room.RoomData.RoomAbility != null)
                    {
                        Debug.Log("cooldown " + GameManager.instance.GetCurrentCooldown(room.RoomData.RoomAbility));
                        if (GameManager.instance.GetEnemyCurrentCooldown(room.RoomData.RoomAbility) == 0)
                        {
                            GameManager.instance.AddEnemyAbilityOneCooldown(room.RoomData.RoomAbility);
                            Debug.Log("add 1 cooldown to " + room.RoomData.RoomAbility.name);
                        }
                    }
                }
            }
        }
    }
}
