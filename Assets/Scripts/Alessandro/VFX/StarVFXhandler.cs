using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class StarVFXhandler : MonoBehaviour
{
    [SerializeField] int _howManyStarsTotal = 20;
    [SerializeField] GameObject _star;
    [SerializeField] int _howManyStarsCycle = 5;
    [SerializeField] float delayBetweenCycles = 0.5f;

    [SerializeField] Transform _topLeftPosAnchor;
    [SerializeField] Transform _botRightPosAnchor;
    [SerializeField] Transform _StarSpawnPoint;
    [SerializeField] UnityEvent OnStartStars;
    [SerializeField] UnityEvent OnCancelStars;

    private List<StarInstance> StarArray = new List<StarInstance>();
    private IEnumerator coroutine;
    GameObject tempVal;

    private void Start()
    {
        if (_howManyStarsCycle > _howManyStarsTotal)
            _howManyStarsCycle = _howManyStarsTotal;

        for (int i = 0; i < _howManyStarsTotal; i++)
        {
            // make an array of stars at the start to load them and save memory space later
            tempVal = Instantiate(_star, _StarSpawnPoint.transform.position, Quaternion.identity, _StarSpawnPoint);
            StarArray.Add(tempVal.GetComponent<StarInstance>());
            StarArray[i].StarSpawnPoint = new Vector2(0, 0);
        }
        //StartVFX();
    }
    // their position is set outside the screen boundaries (might not have to set them here, could do it from the star itself)
    // function that selects _howManyStarsCycle number of stars that arent active
    // give them random positions
    // when they're not active anymore, they'll set themselves down
    private IEnumerator VFXCycleRandomSpawn()
    {
        while (true)
        {
            for (int i = 0; i < _howManyStarsCycle ; i++)
            {
                if(!StarArray[i].IsActive)
                {
                    // define random pos for object
                    Vector2 randomPos = new Vector2(UnityEngine.Random.Range(_topLeftPosAnchor.position.x, _botRightPosAnchor.position.x), 
                        UnityEngine.Random.Range(_botRightPosAnchor.position.y, _topLeftPosAnchor.position.y));
                    StarArray[i].Activate(randomPos);
                }
            }
            yield return new WaitForSeconds(delayBetweenCycles);
        }
    }

    private IEnumerator VFXCycleMovement()
    {
        for (int i = 0; i < _howManyStarsCycle; i++)
        {
            if (!StarArray[i].IsActive)
            {
                // define random pos for object
                Vector2 randomPos = new Vector2(UnityEngine.Random.Range(_topLeftPosAnchor.position.x, _botRightPosAnchor.position.x),
                    UnityEngine.Random.Range(_botRightPosAnchor.position.y, _topLeftPosAnchor.position.y));
                StarArray[i].Activate(randomPos);
            }
        }
        yield return null;
    }
    public void StartVFX()
    {
        // bouton ammène ici
        // lance la loop après setup
        coroutine = VFXCycleMovement();
        //coroutine = VFXCycleRandomSpawn();
        StartCoroutine(coroutine);
        OnStartStars.Invoke();
    }
    public void StopVFX()
    {
        // call for end of loop
        for (int i = 0; i < _howManyStarsTotal; i++)
        {
            if (StarArray[i].IsActive)
            {
                StarArray[i].Reset();
            }
            // and resets
            OnCancelStars.Invoke();
            StopAllCoroutines();
        }
    }
    public void ToggleSFX(bool toggle)
    {
        if(toggle)
        {
            StartVFX();
        } else {
            StopVFX();
        }
    }
}