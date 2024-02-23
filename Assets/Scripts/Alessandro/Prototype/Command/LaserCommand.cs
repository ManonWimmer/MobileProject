using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LaserCommand : ACommand
{
    public override IEnumerator Execute(CommandContext args)
    {
        // the active logic of the command
        // different steps, staircase mode
        // attacking step, hit step, applying damage step, feedback step
        yield return null;
        _sender.GetHitLaser(args);
    }


    // je vais peut être mettre cette preview dans ACommand en fonction commune car la majorité des
    // capacité devraient avoir le même type de visée, juste en remplacant pour les cas spécifiques
    public override IEnumerator Preview(CommandContext args)
    {
        // the aiming logic of the command
        // same as above
        // selecting phase, highlighting, confirming
        yield return null;
        // Once logic is done, execute the attacking part
        Execute(args);
    }
}
