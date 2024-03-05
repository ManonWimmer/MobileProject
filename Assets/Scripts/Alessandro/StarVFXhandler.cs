using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StarVFXhandler : MonoBehaviour
{
    [SerializeField] int _howManyStarsTotal = 20;
    [SerializeField] GameObject _star;
    [SerializeField] int _howManyStarsCycle = 5;
    [SerializeField] float delayBetweenCycles = 0.5f;

    [SerializeField] Transform _topLeftPosAnchor;
    [SerializeField] Transform _botRightPosAnchor;
    [SerializeField] Transform _StarSpawnPoint;

    private List<StarInstance> StarArray = new List<StarInstance>();
    private IEnumerator coroutine;
    GameObject tempVal;
    StarInstance bruh;

    private void Start()
    {
        if (_howManyStarsCycle > _howManyStarsTotal)
            _howManyStarsCycle = _howManyStarsTotal;

        for (int i = 0; i < _howManyStarsTotal; i++)
        {
            // make an array of stars at the start to load them and save memory space later
            tempVal = Instantiate(_star, _StarSpawnPoint.position, _StarSpawnPoint.rotation, _StarSpawnPoint);
            bruh = tempVal.GetComponent<StarInstance>();
            StarArray.Add(bruh);
            StarArray[i].StarSpawnPoint = _StarSpawnPoint.transform.position;
        }

        StartVFX();
    }

    // their position is set outside the screen boundaries (might not have to set them here, could do it from the star itself)
    // function that selects _howManyStarsCycle number of stars that arent active
    // give them random positions
    // when they're not active anymore, they'll set themselves down


    private IEnumerator VFXCycle()
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

/*    private void Start()
    {
        coroutine = VFXCycle();
        StartVFX();
    }*/

    public void StartVFX()
    {
        // bouton ammène ici
        // lance la loop après setup
        coroutine = VFXCycle();
        StartCoroutine(coroutine);
    }


    public void StopVFX()
    {
        // call for end of loop
        // and resets
        StopAllCoroutines();
    }
}
