using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Dragon;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private string          reloadSceneName;
    [SerializeField] private string          nextScene;
    private static           bool            _doesnotSeenOpeningAnimation; //尚未看過入場動畫
    [SerializeField] private GameObject[]    HideObjs;
    [SerializeField] private Animator        Witch ,   Opening , Dragon , Gem;
    [SerializeField] private GameObject      FakeGem , RealGem;
    [SerializeField] private FireBallSpawner _fireBallSpawner;
    public                   GameObject      Dragon2;

    private void Start()
    {
        CallDragonFallAnimation(); // 龍下降動畫
        return;
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

    public void CallNextActionStart()
    {
        StartCoroutine("BossAction");
    }

    public IEnumerator BossAction()
    {
        yield return new WaitForSeconds(5f); //打怪時間

        var behaviour = Random.Range(0 , 3);
        if (behaviour < 2)
        {
        #region Fire Ball action flow

            var waitTime = _fireBallSpawner.Excute();
            yield return new WaitForSeconds(waitTime); // 執行飛行

        #endregion
        }
        else if (behaviour >= 2)
        {
        #region Fly actoin flow

            Dragon.SetTrigger("Fly");
            yield return new WaitForSeconds(5f); // 執行飛行
            Dragon2.SetActive(true);
            yield return new WaitForSeconds(5.2f); // 飛行結束End 5.20 
            Dragon2.SetActive(false);
            Dragon.SetTrigger("Fall");

        #endregion
        }
        CallNextActionStart();
    }

    public void CallDragonFallAnimation()
    {
        RealGem.gameObject.SetActive(true);
        FakeGem.gameObject.SetActive(false);
        Dragon.gameObject.SetActive(true);
        Dragon.transform.position += Vector3.up * 10;
        Dragon.SetTrigger("Fall");
        CallNextActionStart();
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