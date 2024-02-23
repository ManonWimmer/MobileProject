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


    // je vais peut �tre mettre cette preview dans ACommand en fonction commune car la majorit� des
    // capacit� devraient avoir le m�me type de vis�e, juste en remplacant pour les cas sp�cifiques
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
