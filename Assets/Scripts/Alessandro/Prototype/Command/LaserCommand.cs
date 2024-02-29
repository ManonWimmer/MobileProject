using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LaserCommand : ACommand<AbilityType>
{
    public override IEnumerator Execute(CommandContext args)
    {
        Debug.Log("execute phase with these args: " + args.SenderID + " | " + args.ReceiverID + " | " + args.TileAimed.x + ","+ args.TileAimed.y);
        
        yield return Preview(args);
        // the active logic of the command
        // different steps, staircase mode
        // attacking step, hit step, applying damage step, feedback step
        _receiver.GetHitLaser(args);
        yield return null;
    }


    // je vais peut �tre mettre cette preview dans ACommand en fonction commune car la majorit� des
    // capacit� devraient avoir le m�me type de vis�e, juste en remplacant pour les cas sp�cifiques
    public override IEnumerator Preview(CommandContext args)
    {
        Debug.Log("preview phase");
        // the aiming logic of the command
        // same as above
        // selecting phase, highlighting, confirming
        yield return null;
    }
}
