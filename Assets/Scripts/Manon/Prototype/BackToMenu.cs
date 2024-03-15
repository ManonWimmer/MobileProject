using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public void Use()
    {
        SceneManager.LoadScene("MainMenu");

        if (audioManager.instance != null)
            audioManager.instance.ChangeMode();
    }
}
