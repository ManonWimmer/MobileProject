using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static CameraController instance;
    [SerializeField] Transform _cameraPosShipPlayer1;
    [SerializeField] Transform _cameraRewindPosShipPlayer1;
    [SerializeField] Transform _cameraPosShipPlayer2;
    [SerializeField] Transform _cameraRewindPosShipPlayer2;

    [SerializeField] Camera _mainCamera;
    [SerializeField] float _lerpDuration = 1f;

    private bool _isMoving;

    private Transform _currentPos;
    // ----- FIELDS ----- //

    private void Awake()
    {
        instance = this;
    }

    public float GetDistanceShipToRewind()
    {
        return Vector3.Distance(_cameraPosShipPlayer1.position, _cameraRewindPosShipPlayer1.position);
    }

    void Start()
    {
        _mainCamera.transform.position = _cameraPosShipPlayer1.position;
        _currentPos = _cameraPosShipPlayer1;
    }

    public void SwitchPlayerShipCameraDirectly(Player player)
    {
        Debug.Log("switch player ship camera " + player.ToString());
        if (player == Player.Player1)
        {
            _mainCamera.transform.position = _cameraPosShipPlayer1.position;
            _currentPos = _cameraPosShipPlayer1;
        }
        else
        {
            _mainCamera.transform.position = _cameraPosShipPlayer2.position;
            _currentPos = _cameraPosShipPlayer2;
        }
    }

    public void SwitchRewindPlayerShipCameraDirectly(Player player)
    {
        Debug.Log("switch player rewind ship camera " + player.ToString());
        if (player == Player.Player1)
        {
            _mainCamera.transform.position = _cameraRewindPosShipPlayer1.position;
            _currentPos = _cameraRewindPosShipPlayer1;
        }
        else
        {
            _mainCamera.transform.position = _cameraRewindPosShipPlayer2.position;
            _currentPos = _cameraRewindPosShipPlayer2;
        }
    }

    public void SwitchPlayerShipCameraWithLerp()
    {
        Debug.Log("a");
        if (!_isMoving)
        {
            if (_currentPos == _cameraPosShipPlayer1)
            {
                StartCoroutine(LerpPosition(_cameraPosShipPlayer2));
            }
            else
            {
                StartCoroutine(LerpPosition(_cameraPosShipPlayer1));
            }
        }
    }


    IEnumerator LerpPosition(Transform transTarget)
    {
        Debug.Log("lerp camera position " + transTarget.name);
        _isMoving = true;
        float timeElapsed = 0f;
        Vector3 startingPos = _mainCamera.transform.position;
        Vector3 targetPos = new Vector3(transTarget.position.x, transTarget.position.y, -10);

        while (timeElapsed < _lerpDuration)
        {
            _mainCamera.transform.position = Vector3.Lerp(startingPos, targetPos, timeElapsed / _lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _isMoving = false;
        _currentPos = transTarget;
    }
}

