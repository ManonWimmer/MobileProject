using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoomSO : ScriptableObject
{
    public string RoomName;
    public Sprite RoomIcon;
    public Sprite RoomPatternImg;
    public scriptablePower RoomAbility;
    public bool IsVital;
}
