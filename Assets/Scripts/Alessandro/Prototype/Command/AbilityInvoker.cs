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
    [SerializeField] ShipManager senderTESTManager;
    [SerializeField] ShipManager receiverTESTManager;

    private void Awake()
    {
        if(abilitiesArray == null)
        {
            throw new System.ArgumentNullException();
        }
    }
    public void ExecuteCommand(AbilityType abilityType, ShipManager senderManager, ShipManager receiverManager, Vector2 tileAimed)
    {
        Debug.Log("Ability Invoker Setup");
        ACommand<AbilityType> command = _GenerateCommmand(abilityType);
        // le truc pour passer le scriptable object est moche je sais
        command.Init(senderManager, receiverManager, abilitiesArray[(int)abilityType]);
        CommandContext args = new CommandContext();
        // set values
        args.TileAimed = tileAimed;
        args.SenderID = senderManager.ShipID;
        args.ReceiverID = receiverManager.ShipID;
        // execute command
        launchAbility(command, args);
        Debug.Log("Ability Invoker Setup ends");
    }

    private void launchAbility(ACommand<AbilityType> command, CommandContext args)
    {
        Debug.Log("launch ability");
        StartCoroutine(command.Execute(args));
    }

    private ACommand<AbilityType> _GenerateCommmand(AbilityType abilityType)
    {
        switch(abilityType)
        {
            case AbilityType.LaserShoot: return new LaserCommand();
        }

        return null;
    }

    public void TestAbility()
    {
        Debug.Log("test execute command starts");
        ExecuteCommand(AbilityType.LaserShoot, senderTESTManager, receiverTESTManager, new Vector2(0, 0));
        Debug.Log("test execute command completed");

    }
}
