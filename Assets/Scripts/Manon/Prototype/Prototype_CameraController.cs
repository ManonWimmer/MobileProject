using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Prototype_CameraController : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static Prototype_CameraController instance;
    [SerializeField] Transform _gridPlayer1;
    [SerializeField] Transform _gridPlayer2;
    [SerializeField] float _lerpDuration;

    private Transform _currentTarget;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    IEnumerator LerpPosition()
    {
        Debug.Log("lerp position");
        float timeElapsed = 0f;
        Vector3 startingPos = transform.position;
        Vector3 targetPos = new Vector3(_currentTarget.position.x, _currentTarget.position.y, -10);

        while (timeElapsed < _lerpDuration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPos, timeElapsed / _lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void SwitchPlayerShipCamera(Player player)
    {
        Debug.Log("switch player ship camera " + player.ToString());
        if (player == Player.Player1)
        {
            _currentTarget = _gridPlayer1;
        }
        else
        {
            _currentTarget = _gridPlayer2;
        }

        StartCoroutine(LerpPosition());
    }




}

