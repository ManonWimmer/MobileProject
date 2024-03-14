using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float EasterEggMaxSpawn; // IN SECONDS
    [SerializeField] float EasterEggMinSpawn;

    float _randomVal = 0;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (EasterEggMaxSpawn < EasterEggMinSpawn)
            EasterEggMinSpawn = EasterEggMaxSpawn;
        // determine a number
        Reset();
        StartCoroutine(EasterEggIterator());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator EasterEggIterator()
    {
        Debug.Log("Wait " + _randomVal + " seconds");
        yield return new WaitForSeconds(_randomVal);
        anim.SetTrigger("PlaySus");
        Reset();
        StartCoroutine(EasterEggIterator());
    }

    private void Reset()
    {
        _randomVal = UnityEngine.Random.Range(EasterEggMinSpawn, EasterEggMaxSpawn);
    }
}
