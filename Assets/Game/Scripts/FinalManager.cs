using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FinalManager : MonoBehaviour
{
    private bool canEsc;
    public GameObject back ;

    [SerializeField] private GameObject NukeObj;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("EscEnable" , 15f);
    }

    void EscEnable()
    {
        canEsc = true;
        CallNukeAnimation();
    }

    private void CallNukeAnimation()
    {
        NukeObj.gameObject.SetActive(true);
        Invoke("OpenReturnbuttom",10f);
    }
    void OpenReturnbuttom() {
        back.SetActive(true);
    }
    public void BackScene() {
        SceneManager.LoadScene(0);
    }
    void Update()
    {
        if (canEsc && Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
}
