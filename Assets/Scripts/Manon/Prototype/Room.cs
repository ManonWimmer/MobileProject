using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // ----- FIELDS ----- //
    public SpriteRenderer CenterTileSR;
    public List<SpriteRenderer> LeftTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> RightTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> TopTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> BottomTilesSR = new List<SpriteRenderer>();

    public List<SpriteRenderer> DiagBottomLeftTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> DiagBottomRightTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> DiagTopLeftTilesSR = new List<SpriteRenderer>();
    public List<SpriteRenderer> DiagTopRightTilesSR = new List<SpriteRenderer>();

    public RoomSO RoomData;

    public bool IsRoomDestroyed;
    // ----- FIELDS ----- //
}
