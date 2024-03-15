using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.Events;
using TMPro;

public class GoogleSignInCheck : MonoBehaviour{
    private bool isPlayerConnected = false;
    public static GoogleSignInCheck instance;
    [SerializeField] bool shouldGoogleBeActive = true;

    [SerializeField] UnityEvent OnPlayerConnectingSuccess;
    [SerializeField] UnityEvent OnPlayerConnectingFail;
    [SerializeField] TextMeshProUGUI errorText;
    int loginHardCap = 5;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        // juste un check pour debug au cas ou si ça marche pas très bien
        if (shouldGoogleBeActive)
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
    }

/*    internal void ProcessAuthenticationa(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            isPlayerConnected = true;
            OnPlayerConnectingSuccess.Invoke();
            // Continue with Play Games Services
        }
        else
        {
            OnPlayerConnectingFail.Invoke();
            // juste histoire de pas rentrer en boucle inf de login
            Debug.Log("Player is not connected");
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            if(loginHardCap > 0)
                PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
        }
    }*/

    internal void ProcessAuthentication(SignInStatus status)
    {
        loginHardCap -= 1;
        switch (status)
        {
            case SignInStatus.Success:
                isPlayerConnected = true;
                OnPlayerConnectingSuccess.Invoke();
                errorText.text = ("good :D");
                break;
            case SignInStatus.Canceled:
                OnPlayerConnectingFail.Invoke();
                Debug.Log("Authentication canceled by user.");
                errorText.text = ("Authentication canceled by user.");
                break;
            case SignInStatus.InternalError:
                OnPlayerConnectingFail.Invoke();
                Debug.LogError("Internal error during authentication.");
                errorText.text = ("Internal error during authentication.");

                break;
            default:
                OnPlayerConnectingFail.Invoke();
                Debug.LogWarning("Unknown error during authentication.");
                errorText.text = ("Unknown error during authentication.");

                break;
        }
        if (loginHardCap > 0 && status != SignInStatus.Success)
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }

    public bool getPlayerStatus()
    {
        return isPlayerConnected;
    }
}
