using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalManager : MonoBehaviour
{
    private bool canEsc;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (canEsc && Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
}
