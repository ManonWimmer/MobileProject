using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "scriptable/Power")]
public class scriptablePower : ScriptableObject
{
    public string _powerName;
    public string _description;

    public int _powerNeed;
}
