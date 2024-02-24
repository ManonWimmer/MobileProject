using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleReveal : MonoBehaviour
{
    public void TrySimpleReveal()
    {
        if (GameManager.instance.TargetOnTile != null)
        {
            if (!GameManager.instance.TargetOnTile.IsDestroyed && !GameManager.instance.TargetOnTile.IsMissed) // tile jamais hit
            {
                if (EnergySystem.instance.TryUseEnergy(GameManager.instance.PlayerTurn, 2)) // 2 temp -> energy cost de la compétence SO
                {
                    if (GameManager.instance.TargetOnTile.IsOccupied)
                    {
                        Debug.Log("reveal room " + GameManager.instance.TargetOnTile.Room.name);
                        //GameManager.instance.TargetOnTile.RoomTileSpriteRenderer.color = Color.magenta;
                        GameManager.instance.TargetOnTile.IsReavealed = true;

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
                        Debug.Log("no room on reveal");

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
