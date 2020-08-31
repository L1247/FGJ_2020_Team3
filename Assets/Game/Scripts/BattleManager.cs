using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private string       reloadSceneName;
    [SerializeField] private string       nextScene;
    public           bool         _doesnotSeenOpeningAnimation; //尚未播放
    [SerializeField] private GameObject[] HideObjs;
    [SerializeField] private Animator     Witch ,   Opening , Dragon , Gem;
    [SerializeField] private GameObject   FakeGem , RealGem;
    
    private void Start()
    {
        if (_doesnotSeenOpeningAnimation == true) //
        {
            foreach (var hideObj in HideObjs)
            {
                hideObj.SetActive(true);
            }
            Witch.gameObject.SetActive(true);

            // 女巫變身
            WitchChange();
        }
        else
        {
             CallDragonFallAnimation();
        }
    }

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
        _doesnotSeenOpeningAnimation = true;
        SceneManager.LoadScene(reloadSceneName);
    }

    public void WitchChange()
    {
        Witch.SetTrigger("Change");
    }

    public void CallOpeningAnimation()
    {
        Witch.gameObject.SetActive(false);
        Opening.SetTrigger("Trigger");
    }

    public void CallDragonFallAnimation()
    {
        RealGem.gameObject.SetActive(true); 
        FakeGem.gameObject.SetActive(false);
        Dragon.gameObject.SetActive(true);
        Dragon.transform.position += Vector3.up * 10; 
        Dragon.SetTrigger("Fall");
    }

    public void CallGemAnimation()
    {
        RealGem.gameObject.SetActive(false);
        FakeGem.gameObject.SetActive(true);
        Gem.Play("Gem");       
    }
}