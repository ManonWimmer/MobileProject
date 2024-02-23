using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour
{
    [SerializeField] private powerButton _button;

    public void Attack()
    {
        _button.GetEndTrun();
    }
}
