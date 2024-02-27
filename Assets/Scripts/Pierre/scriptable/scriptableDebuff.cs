using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "scriptable/Debuff")]
public class scriptableDebuff : ScriptableObject
{
    public Sprite _icon;
    public string _name;
    public string _description;
}
