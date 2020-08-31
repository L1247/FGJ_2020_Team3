
using UnityEngine;
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
    private void Awake()
    {
        dialog_start = true;
        SKIPBUT.SetActive(false);
        BGMAUDIO.Play();
    } 
    void Update()
    {
        sec +=Time.deltaTime;
        if (dialog_start)
        {
            Front.Play();
        }

        if (sec>=7)
        {
            SKIPBUT.SetActive(true);
        }

        if (sec>=60)
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
        SceneManager.LoadScene(waitForLoadScene);
    } 
    private void LoadScene()
    {
        SceneManager.LoadScene(waitForLoadScene);
    }
}
