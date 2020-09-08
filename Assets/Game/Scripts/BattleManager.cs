using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private string       reloadSceneName;
    [SerializeField] private string       nextScene;
    private static           bool         _doesnotSeenOpeningAnimation; //尚未看過入場動畫
    [SerializeField] private GameObject[] HideObjs;
    [SerializeField] private Animator     Witch ,   Opening , Dragon , Gem;
    [SerializeField] private GameObject   FakeGem , RealGem;
    public GameObject Dragon2;
    
    private void Start()
    {
        if (_doesnotSeenOpeningAnimation == false) //尚未撥放=F ---> 已經撥放過
        {
            foreach (var hideObj in HideObjs)
            {
                hideObj.SetActive(true);
            }

            // 女巫變身
            WitchChange();
        }
        else
        {
             CallDragonFallAnimation(); // 龍下降動畫
            
        }
    }
    public void DFlyAnimation() {   
        StartCoroutine("Fly2");

    }
    public IEnumerator Fly2() {
        
        yield return new WaitForSeconds(15f); //打怪時間
        print(" fly");
        Dragon.SetTrigger("Fly");
        yield return new WaitForSeconds(5f); //飛行
        Dragon2.SetActive(true);
        yield return new WaitForSeconds(5.2f); //End 5.20 
        Dragon2.SetActive(false);
        Dragon.SetTrigger("Fall");
        DFlyAnimation();
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
        DFlyAnimation();
    }

    public void CallGemAnimation()
    {
        RealGem.gameObject.SetActive(false);
        FakeGem.gameObject.SetActive(true);
        Gem.Play("Gem");       
    }

    private void OnApplicationQuit()
    {
        _doesnotSeenOpeningAnimation = false;
    }
}