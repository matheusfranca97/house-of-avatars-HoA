using JetBrains.Annotations;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupSceneLoader : MonoBehaviour
{
    private const string STARTUP_SCENE = "Startup";
    private const string ESSENTIALS_SCENE = "Essentials";
    private const string MAINMENU_SCENE = "Main Menu";
    private const string SERVER_SCENE = "Server Scene";

    private IEnumerator Start()
    {
#if UNITY_SERVER
        AsyncOperation operation = SceneManager.LoadSceneAsync(SERVER_SCENE, LoadSceneMode.Additive);
        while (!operation.isDone)
            yield return null;
        
        Scene serverScene = SceneManager.GetSceneByName(SERVER_SCENE);
        SceneManager.SetActiveScene(serverScene);

        //FusionBootstrap bootStrap = serverScene.GetComponents<FusionBootstrap>(true).First();
        //if (bootStrap == null)
        //{
        //    Debug.LogError("Server Bootstrap is Null!");
        //    Application.Quit();
        //    yield return null;
        //}
        //bootStrap.StartServer();
#else
        AsyncOperation operation = SceneManager.LoadSceneAsync(ESSENTIALS_SCENE, LoadSceneMode.Additive);
        while (!operation.isDone)
            yield return null;

        operation = SceneManager.LoadSceneAsync(MAINMENU_SCENE, LoadSceneMode.Additive);
        while (!operation.isDone)
            yield return null;
#endif
        operation = SceneManager.UnloadSceneAsync(STARTUP_SCENE);
        while (!operation.isDone)
            yield return null;

    }
}