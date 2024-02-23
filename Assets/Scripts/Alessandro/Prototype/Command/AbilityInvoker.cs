using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    LaserShoot = 0,
}



public class AbilityInvoker : MonoBehaviour
{
    [SerializeField] scriptablePower[] abilitiesArray = null;

    private void Awake()
    {
        if(abilitiesArray == null)
        {
            throw new System.ArgumentNullException();
        }
    }
    public void ExecuteCommand(AbilityType abilityType, ShipManager senderManager, ShipManager receiverManager, Vector2 tileAimed)
    {
        ACommand command = _GenerateCommmand(abilityType);
        // le truc pour passer le scriptable object est moche je sais
        command.Init(senderManager, receiverManager, abilitiesArray[(int)abilityType]);
        CommandContext args = new CommandContext();
        // set values
        args.TileAimed = tileAimed;
        args.SenderID = senderManager.ShipID;
        args.ReceiverID = receiverManager.ShipID;
        // execute command
        command.Execute(args);
    }

    private ACommand _GenerateCommmand(AbilityType abilityType)
    {
        switch(abilityType)
        {
            case AbilityType.LaserShoot: return new LaserCommand();
        }

        return null;
    }
}
