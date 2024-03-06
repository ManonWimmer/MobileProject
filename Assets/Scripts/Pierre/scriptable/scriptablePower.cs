using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewItem", menuName = "scriptable/Power")]
public class scriptablePower : ScriptableObject
{
    public string AbilityName;
    public string Description;
    public Sprite Icon;

    public int ActionPointsNeeded;
    public int Cooldown;

    public GameObject AbilityButton;

    public bool CanBeDefused;
}
