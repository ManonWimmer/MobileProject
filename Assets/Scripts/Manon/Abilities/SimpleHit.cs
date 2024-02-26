using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHit : MonoBehaviour
{
    public void TrySimpleHit()
    {
        if (GameManager.instance.TargetOnTile != null)
        {
            if (!GameManager.instance.TargetOnTile.IsDestroyed && !GameManager.instance.TargetOnTile.IsMissed) // tile jamais hit
            {
                if (ActionPointsManager.instance.TryUseActionPoints(GameManager.instance.PlayerTurn)) 
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
                else
                {
                    Debug.Log("pas assez d'energie! (2 demandées)");
                }
            }
            else
            {
                // already hit that tile
                TargetController.instance.ChangeTargetColorToRed();
            }
        }

        // update button color
    }
}
