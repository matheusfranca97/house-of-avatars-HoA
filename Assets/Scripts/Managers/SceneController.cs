
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance { private set; get; }

    public SceneType currentScene { get; set; }
    private const string MAINMENU_SCENE = "Main Menu";
    private const string GAME_ESSENTIALS_SCENE = "Game Essentials";

    public Coroutine loadSceneRoutine;

    public static bool completeLoad = true;
    public static bool inGame = false;

    private void Awake()
    {
        instance = this;
        int activeScenes = SceneManager.sceneCount;
        for(int i = 0; i < activeScenes; i++)
        {
            Scene activeScene = SceneManager.GetSceneAt(i);
            SceneType[] sceneTypes = Enum.GetValues(typeof(SceneType)) as SceneType[];
            if (!sceneTypes.TryFind(j => j.GetIdentifier() == activeScene.name, out SceneType item))
                continue;

            currentScene = item;
            break;
        }
    }

    public void ForceSetCurrentScene(SceneType sceneType)
    {
        currentScene = sceneType;
    }
    
    public void LoadMainMenu_FromGameScene(StartupScreenType startupScreenType)
    {
        if (loadSceneRoutine != null)
            return;

        loadSceneRoutine = StartCoroutine(Routine_LoadMainMenu_FromGame(startupScreenType));
    }

    public void LoadGameScene_FromMainMenu(SceneType sceneType)
    {
        if (loadSceneRoutine != null)
            return;

        loadSceneRoutine = StartCoroutine(Routine_LoadGameScene_FromMainMenu(sceneType));
    }

    public void SwitchGameScene(SceneType sceneType, SpawnLocationType spawnLocation)
    {
        if (loadSceneRoutine != null)
            return;

        loadSceneRoutine = StartCoroutine(Routine_LoadGameScene(sceneType, spawnLocation));
    }

    private IEnumerator Routine_LoadMainMenu_FromGame(StartupScreenType startupScreenType)
    {
        IEnumerator subRoutine = LoadingScreen.instance.Routine_ShowLoadingScreen();
        while (subRoutine.MoveNext())
            yield return null;

        AsyncOperation operation;
        if (currentScene != SceneType.None && SceneManager.GetSceneByName(currentScene.GetIdentifier()).isLoaded)
        {
            operation = SceneManager.UnloadSceneAsync(currentScene.GetIdentifier());
            while (!operation.isDone)
                yield return null;
        }

        if (SceneManager.GetSceneByName(GAME_ESSENTIALS_SCENE).isLoaded)
        {
            operation = SceneManager.UnloadSceneAsync(GAME_ESSENTIALS_SCENE);
            while (!operation.isDone)
                yield return null;
        }

        if (!SceneManager.GetSceneByName(MAINMENU_SCENE).isLoaded)
        {
            operation = SceneManager.LoadSceneAsync(MAINMENU_SCENE, LoadSceneMode.Additive);
            while (!operation.isDone)
                yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(MAINMENU_SCENE));
        ChatLog.ResetChat();

        while (StartupUI.instance == null)
            yield return null;

        StartupUI.instance.selectedScreenType.value = startupScreenType;

        subRoutine = LoadingScreen.instance.Routine_HideLoadingScreen();
        while (subRoutine.MoveNext())
            yield return null;

        inGame = false;
        currentScene = SceneType.None;
        PlayerSettingsManager.instance.uiFocused.value = false;
        PlayerSettingsManager.instance.mouseMode.value = MouseMode.UI;
        loadSceneRoutine = null;
    }

    private IEnumerator Routine_LoadGameScene_FromMainMenu(SceneType sceneType)
    {
        completeLoad = false;

        IEnumerator subRoutine = LoadingScreen.instance.Routine_ShowLoadingScreen();
        while (subRoutine.MoveNext())
            yield return null;

        AsyncOperation operation = SceneManager.UnloadSceneAsync(MAINMENU_SCENE);
        while (!operation.isDone)
            yield return null;

        operation = SceneManager.LoadSceneAsync(GAME_ESSENTIALS_SCENE, LoadSceneMode.Additive);
        while (!operation.isDone)
            yield return null;

        //operation = SceneManager.LoadSceneAsync(sceneType.GetIdentifier(), LoadSceneMode.Additive);
        //while (!operation.isDone)
        //    yield return null;

        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneType.GetIdentifier()));
        //NetworkObject[] networkObjects = FindObjectsByType<NetworkObject>(FindObjectsSortMode.None);
        //runner.RegisterSceneObjects(SceneRef.FromIndex(SceneManager.GetSceneByName(sceneType.GetIdentifier()).buildIndex), networkObjects);

        //foreach (NetworkObject netObject in networkObjects)
        //    netObject.gameObject.SetActive(true);

        //Give the player physics a frame to update the position to match the floor.
        yield return new WaitForFixedUpdate();

        while (!completeLoad)
            yield return null;

        subRoutine = LoadingScreen.instance.Routine_HideLoadingScreen();
        while (subRoutine.MoveNext())
            yield return null;

        currentScene = sceneType;
        loadSceneRoutine = null;
        inGame = true;
        PlayerSettingsManager.instance.uiFocused.value = false;
    }

    private IEnumerator Routine_LoadGameScene(SceneType sceneType, SpawnLocationType spawnLocation)
    {
        IEnumerator subRoutine = LoadingScreen.instance.Routine_ShowLoadingScreen();
        while (subRoutine.MoveNext())
            yield return null;

        AsyncOperation operation = SceneManager.UnloadSceneAsync(currentScene.GetIdentifier());
        while (!operation.isDone)
            yield return null;

        operation = SceneManager.LoadSceneAsync(sceneType.GetIdentifier(), LoadSceneMode.Additive);
        while (!operation.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneType.GetIdentifier()));

        yield return new WaitForFixedUpdate();

        subRoutine = LoadingScreen.instance.Routine_HideLoadingScreen();
        while (subRoutine.MoveNext())
            yield return null;

        currentScene = sceneType;
        loadSceneRoutine = null;
    }

    public void CancelLoad()
    {
        if (loadSceneRoutine != null)
        {
            StopCoroutine(loadSceneRoutine);
            loadSceneRoutine = null;
        }
    }
}
