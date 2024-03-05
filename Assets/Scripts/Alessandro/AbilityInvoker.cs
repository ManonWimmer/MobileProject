using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AbilityType
{
    LaserShoot = 0,
}


public class AbilityInvoker : MonoBehaviour
{
    //[SerializeField] scriptablePower[] abilitiesArray = null;
    //[SerializeField] ShipManager senderTESTManager;
    //[SerializeField] ShipManager receiverTESTManager;

    [SerializeField] AAbility[] abilities;

    // in order, ability used, the aimed location it was shot at, the sender, the receiver and the order
    //List<(AbilityType abilityUsed, Vector2 locationShot, ShipManager sender, ShipManager receiver, float delay)> savedMoves;
    List<(AAbility abilityUsed, Tile tilenShot, Player sender, Player receiver, int order)> savedMoves;

    private int order = 0;

    private void Awake()
    {
        //if (abilitiesArray == null || abilities == null)
        if (abilities == null)
        {
                throw new System.ArgumentNullException();
        }
    }
    public void ExecuteCommand(int abilityID)
    {
        var foundItem = Array.Find(abilities, element => element.AbilityID == abilityID);
        if(foundItem)
        {
            // Execute ability
            bool isSucceed = foundItem.TryUseAbility();
            if (isSucceed)
            {
                // Add to the rewind
                if(GameManager.instance.PlayerTurn == Player.Player1)
                {
                    (AAbility abilityUsed, Tile tilenShot, Player sender, Player receiver, int order) values = (foundItem, GameManager.instance.TargetOnTile, Player.Player1, Player.Player2, order);
                    savedMoves.Add(values);
                } else
                {
                    (AAbility abilityUsed, Tile tilenShot, Player sender, Player receiver, int order) values = (foundItem, GameManager.instance.TargetOnTile, Player.Player2, Player.Player1, order);
                    savedMoves.Add(values);
                }
                order++;
            }
        }
    }
    IEnumerator RewindTime()
    {
        // save the state of the game (?)
        foreach (var item in savedMoves)
        {
            // force the tile to be aimed at
            // change who the player is
            ExecuteCommand(item.abilityUsed.AbilityID);
            yield return new WaitForSeconds(0.5f);
        }
        savedMoves.Clear();
        // restore the state of the game
    }

    /*    private void launchAbility(ACommand<AbilityType> command, CommandContext args)
        {
            Debug.Log("launch ability");
            StartCoroutine(command.Execute(args));
        }*/

    //public void ExecuteCommand(AbilityType abilityType, ShipManager senderManager, ShipManager receiverManager, Vector2 tileAimed)
    //{
    //    Debug.Log("Ability Invoker Setup");
    //ACommand<AbilityType> command = _GenerateCommmand(abilityType);
    // le truc pour passer le scriptable object est moche je sais
    //command.Init(senderManager, receiverManager, abilitiesArray[(int)abilityType]);
    //CommandContext args = new CommandContext();
    // set values
    //args.TileAimed = tileAimed;
    //args.SenderID = senderManager.ShipID;
    //args.ReceiverID = receiverManager.ShipID;
    // execute command
    //launchAbility(command, args);
    //    Debug.Log("Ability Invoker Setup ends");
    //(AbilityType, Vector2, ShipManager, ShipManager, float) bruh = (abilityType, tileAimed, senderManager, receiverManager, 0.5f);
    //savedMoves.Add(bruh);
    //}

    /*    private ACommand<AbilityType> _GenerateCommmand(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.LaserShoot: return new LaserCommand();
            }

            return null;
        }
    */

    //IEnumerator RewindTime()
    //{
    //    foreach (var item in savedMoves)
    //    {
    //        ExecuteCommand(item.abilityUsed, item.sender, item.receiver, item.locationShot);
    //        yield return new WaitForSeconds(item.delay);
    //    }
    //    savedMoves.Clear();
    //}

    //public void clearRewind()
    //{
    //    savedMoves.Clear();
    //}

    /*    public void TestAbility() // DEBUG
        {

            Debug.Log("test execute command starts");

            if (checkTile())
            {
                ExecuteCommand(AbilityType.LaserShoot, senderTESTManager, receiverTESTManager, new Vector2(0, 0));
            }
            Debug.Log("test execute command completed");
        }*/
}