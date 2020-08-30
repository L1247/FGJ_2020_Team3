using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class Dialog_index_GM : MonoBehaviour
{

    public Animation Front;
    public Animation fadeout;
    
    public GameObject talkBG;
    public GameObject SKIPBUT;

    public AudioSource BGMAUDIO;
    
    public  float  sec          = 0f;
    private bool   dialog_start = false;
    public string waitForLoadScene;

    // Start is called before the first frame update

    private void Awake()
    {
        dialog_start = true;
        SKIPBUT.SetActive(false);
        BGMAUDIO.Play();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        sec +=Time.deltaTime;
        if (dialog_start)
        {
            Front.Play();
        }

        if (sec>=4)
        {
            SKIPBUT.SetActive(true);
        }

        if (sec>=9)
        {
            fadeout.Play();
        }

        if (sec>=12)
        {
            LoadScene();
        }
    }

    public void SKIPBUTTON()
    {
        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(waitForLoadScene);
    }
}
