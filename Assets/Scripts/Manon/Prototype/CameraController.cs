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

    [SerializeField] Transform _abilityButtons;
    [SerializeField] Vector3 _abilityButtonsShow;
    [SerializeField] float _abilityButtonsHideY;

    [SerializeField] Transform _endTurn;
    [SerializeField] Transform _fireButton;
    [SerializeField] Vector3 _endTurnAndFireShow;
    [SerializeField] float _endTurnAndFireHideY;

    public bool CombatOwnSpaceShip;

    private bool _isMoving;

    private Transform _currentPos;

    public bool IsMoving { get => _isMoving; set => _isMoving = value; }
    public Camera MainCamera { get => _mainCamera; set => _mainCamera = value; }

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
        _abilityButtonsShow = _abilityButtons.position;
        _endTurnAndFireShow = _endTurn.position;
    }

    public void SwitchPlayerShipCameraDirectly(Player player)
    {
        Debug.Log("switch player ship camera " + player.ToString());
        UIManager.instance.HideFicheAbility();
        UIManager.instance.HideFicheRoom();

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

        CombatOwnSpaceShip = false;
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
        if (!_isMoving && !AbilityButtonsManager.instance.IsInRewind)
        {
            CombatOwnSpaceShip = !CombatOwnSpaceShip;

            if (_currentPos == _cameraPosShipPlayer1)
            {
                StartCoroutine(LerpPosition(_cameraPosShipPlayer2));
            }
            else
            {
                StartCoroutine(LerpPosition(_cameraPosShipPlayer1));
            }

            if (CombatOwnSpaceShip)
            {
                UIManager.instance.GoToEnemyShipText();
                StartCoroutine(LerpAbilityButtonsPosition(true));
                StartCoroutine(LerpEndTurnAndFirePosition(true));
            }
            else
            {
                UIManager.instance.GoToPlayerShipText();
                StartCoroutine(LerpAbilityButtonsPosition(false));
                StartCoroutine(LerpEndTurnAndFirePosition(false));
            }

            UIManager.instance.UpdateSwitchShipArrow();
        }
    }

    // Rajouter pour quand changement de tour, les remettre à la position de show AU CAS OU
    public  void ResetEndTurnAndAbilityButtonsPos()
    {
        _abilityButtons.position = _abilityButtonsShow;
        _endTurn.position = _endTurnAndFireShow;
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

    IEnumerator LerpAbilityButtonsPosition(bool hide)
    {
        float timeElapsed = 0f;
        Vector3 startingPos = _abilityButtons.position;
        Vector3 targetPos = Vector3.zero;

        if (hide)
        {
            targetPos = new Vector3(_abilityButtonsShow.x, _abilityButtonsHideY, _abilityButtonsShow.z);
        }
        else
        {
            targetPos = _abilityButtonsShow;
        }

        while (timeElapsed < _lerpDuration)
        {
            _abilityButtons.transform.position = Vector3.Lerp(startingPos, targetPos, timeElapsed / _lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator LerpEndTurnAndFirePosition(bool hide)
    {
        float timeElapsed = 0f;
        Vector3 startingPos = _endTurn.position;
        Vector3 targetPos = Vector3.zero;

        if (hide)
        {
            targetPos = new Vector3(_endTurnAndFireShow.x, _endTurnAndFireHideY, _endTurnAndFireShow.z);
        }
        else
        {
            targetPos = _endTurnAndFireShow;
        }

        while (timeElapsed < _lerpDuration)
        {
            _endTurn.transform.position = Vector3.Lerp(startingPos, targetPos, timeElapsed / _lerpDuration);
            _fireButton.transform.position = Vector3.Lerp(startingPos, targetPos, timeElapsed / _lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}

