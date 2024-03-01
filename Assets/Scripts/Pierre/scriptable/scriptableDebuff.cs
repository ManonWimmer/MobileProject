using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "scriptable/Debuff")]
public class scriptableDebuff : ScriptableObject
{
    public Sprite Icon;
    public string DebuffName;
    public string Description;
}
