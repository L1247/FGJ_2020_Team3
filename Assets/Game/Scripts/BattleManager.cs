using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private string reloadSceneName;
    
    public void ReloadScene()
    {
        SceneManager.LoadScene(reloadSceneName);
    }
}
