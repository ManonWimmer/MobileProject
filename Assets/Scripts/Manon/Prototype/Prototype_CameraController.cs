using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Prototype_CameraController : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static Prototype_CameraController instance;
    //[SerializeField] Transform _gridPlayer1;
    //[SerializeField] Transform _gridPlayer2;
    [SerializeField] Camera _cameraShipPlayer1;
    [SerializeField] Camera _cameraShipPlayer2;
    [SerializeField] float _lerpDuration;

    private Camera _currentActiveCamera;

    private Transform _currentTarget;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _cameraShipPlayer1.enabled = true;
        _cameraShipPlayer2.enabled = false;

        _currentActiveCamera = _cameraShipPlayer1;
    }

    /*
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
    */

    public void SwitchPlayerShipCamera(Player player)
    {
        Debug.Log("switch player ship camera " + player.ToString());
        if (player == Player.Player1)
        {
            _cameraShipPlayer1.enabled = true;
            _cameraShipPlayer2.enabled = false;
        }
        else
        {
            _cameraShipPlayer1.enabled = false;
            _cameraShipPlayer2.enabled = true;
        }


        //StartCoroutine(LerpPosition());
        //transform.position = new Vector3(_currentTarget.position.x, _currentTarget.position.y, -10);
    }
}

