using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireBallAfter()
    {
        Debug.Log($"FrieBallAfter");
        _animator.SetTrigger("DragonFire");
    }
}
