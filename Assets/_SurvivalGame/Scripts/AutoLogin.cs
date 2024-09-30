using UnityEngine;
using XWizardGames.UnityGamingServicesAddons.AuthenticationAddon;

public class AutoLogin : MonoBehaviour
{
    [SerializeField]
    private AuthenticationController _authentication;

    [SerializeField]
    private bool _signInOnStart = true;


    private void Start()
    {
        if(_signInOnStart)
            _authentication.SignInAnonymously();

        //Mirror.NetworkManager.singleton.StartServer();
    }
}
