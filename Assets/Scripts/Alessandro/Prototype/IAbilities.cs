using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IAbilities
{
    string Name { get; set; }
    string Description { get; set; }

    Image Icon { get; set; }

    enum Type
    {
        ACTIVE,
        PASSIVE
    }

    Type AbilityType { get; set; }
    int Cost { get; set; }

    //scriptablePower Data { get; set; }

    virtual void AbilityCastCall(int gamePhase)
    {
        switch (gamePhase)
        {
            case 0:
                // if we are in ship building mode, just show the description
                break;
            case 1:
                // if we are in battle mode, depending on the input either show the description or cast the ability
                // check for player energy (if not done before)

                break;
            default:
                // not in a castable area, return bug
                Debug.LogError("Tried casting " + Name + " in Phase " + gamePhase + " of the game, this shouldnt be possible");
                break;
        }
    }

    virtual void ConsumeEnergy()
    {
        // call function/event to reduce player energy based on cost
        throw new NotImplementedException();
    }

    abstract void AbilityUse();
}
