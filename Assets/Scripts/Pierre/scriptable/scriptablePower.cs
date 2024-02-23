using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewItem", menuName = "scriptable/Power")]
public class scriptablePower : ScriptableObject
{
    public string _powerName;
    public string _description;
    public Sprite _icon;

    public int _powerNeed;
}
