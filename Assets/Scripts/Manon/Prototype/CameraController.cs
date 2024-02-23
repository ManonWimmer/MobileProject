using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    // ----- FIELDS ----- //
    public static CameraController instance;
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
    }
}

