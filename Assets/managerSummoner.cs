using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class managerSummoner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _gameManagerPrefab;
    private void Awake()
    {
        GameObject music = GameObject.FindWithTag("Audio");
        GameObject sound = GameObject.FindWithTag("Sound");
        GameObject translate = GameObject.FindWithTag("Translate");
        if (music == null && sound == null && translate == null)
        {
            foreach (var prefab in _gameManagerPrefab)
            {
                GameObject gm = Instantiate(prefab);
                DontDestroyOnLoad(gm);
            }
        }
        Destroy(gameObject);
    }
}
