using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Scene nextScene;
    private AsyncOperation asyncLoad;
    private AsyncOperation asyncUnload;

    public void LoadLevel(string nextSceneName)
    {
        StartCoroutine(NextLevel(nextSceneName));
        asyncLoad.allowSceneActivation = true;
    }

    public void Return()
    {
        StartCoroutine(NextLevel("Menu"));
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator NextLevel(string nextSceneName)
    {
        nextScene = SceneManager.GetSceneByName(nextSceneName);

        asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
    }
}
