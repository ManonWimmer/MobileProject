using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype_CustomCursor : MonoBehaviour
{
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }
}
