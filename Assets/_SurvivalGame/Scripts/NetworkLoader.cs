using PolymindGames;
using System.Collections;
using UnityEngine;
using XWizardGames.STP_MP;

public class NetworkLoader : MonoBehaviour
{
    public void StartSingleplayer()
    {
        StartCoroutine(CStartSingleplayer());
    }

    public void StartServer()
    {
        StartCoroutine(CStartServer());
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator CStartSingleplayer()
    {
        LevelManager.LoadScene("ForestMP");

        yield return new WaitUntil(() => LevelManager.IsLoading == false);

        var session = FindObjectOfType<SessionValidator>();

        if(session != null)
            session.StartSingleplayer();
        else
            Application.Quit();
    }

    private IEnumerator CStartServer()
    {
        LevelManager.LoadScene("ForestMP");

        yield return new WaitUntil(() => LevelManager.IsLoading == false);

        var session = FindObjectOfType<SessionValidator>();

        if(session != null)
            session.StartServer();
        else
            Application.Quit();
    }
}