using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private string reloadSceneName;
    [SerializeField] private string nextScene;

    public void DelayCallLoadNextScene()
    {
        Invoke("LoadNextScene" , 7);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(reloadSceneName);
    }
}