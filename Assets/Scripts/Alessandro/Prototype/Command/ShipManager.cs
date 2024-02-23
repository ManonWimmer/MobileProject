using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public int ShipID = 0;

    // Receiver functions, peut être pas forcément une par capacité mais une par type d'attaque reçue
    public void GetHitLaser(CommandContext context)
    {
        Debug.Log(context.ReceiverID + " is getting hit by a laser sent by " +  context.SenderID + " at " + context.TileAimed.x + "x; " + context.TileAimed.y);
    }
}
