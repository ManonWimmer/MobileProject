using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_Building : MonoBehaviour
{
    // test nouvelle méthode pour pouvoir griser / changer sprite des sprites autour quand y'a un hit
    public SpriteRenderer CenterTileSR;
    public List<SpriteRenderer> LeftTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> RightTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> TopTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> BottomTilesSR = new List<SpriteRenderer>();

}
