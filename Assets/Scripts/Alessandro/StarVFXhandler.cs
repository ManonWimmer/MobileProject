using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StarVFXhandler : MonoBehaviour
{
    bool _isActive = false;
    [SerializeField] int _howManyStarsTotal = 20;
    [SerializeField] GameObject _star;
    [SerializeField] int _howManyStarsCycle = 5;
    [SerializeField] float delayBetweenCycles = 0.5f;

    [SerializeField] Transform _topLeftPosAnchor;
    [SerializeField] Transform _botRightPosAnchor;
    [SerializeField] Transform _StarSpawnPoint;

    private GameObject[] StarArray = null;
    private IEnumerator coroutine;

    private void Awake()
    {
        for (int i = 0; i < _howManyStarsTotal; i++)
        {
            StarArray[i] = Instantiate(_star, _StarSpawnPoint, _StarSpawnPoint);
        }
    }

    // make an array of stars at the start to load them and save memory space later
    // their position is set outside the screen boundaries (might not have to set them here, could do it from the star itself)
    // function that selects _howManyStarsCycle number of stars that arent active
    // give them random positions
    // activate them (and they do their own silly things)
    // when they're not active anymore, they'll set themselves down


    private IEnumerator VFXCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenCycles);
            for (int i = 0; i < _howManyStarsCycle; i++)
            {
                // define random pos for object
                Vector2 randomPos = new Vector2(UnityEngine.Random.Range(_topLeftPosAnchor.position.x, _botRightPosAnchor.position.x), UnityEngine.Random.Range(_botRightPosAnchor.position.y, _topLeftPosAnchor.position.y));
                
                //Instantiate(_star, randomPos, Quaternion.identity);            
            }
        }
        
    }

    private void Start()
    {
        coroutine = VFXCycle();
        StartVFX();
    }

    public void StartVFX()
    {
        // bouton ammène ici
        // lance la loop après setup
        StartCoroutine(coroutine);
    }


    public void StopVFX()
    {
        // call for end of loop
        // and resets
        StopAllCoroutines();
    }
}
