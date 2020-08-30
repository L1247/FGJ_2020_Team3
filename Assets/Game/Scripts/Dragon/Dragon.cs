using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    [SerializeField] private Animator    _animator;
    [SerializeField] private AudioClip   _audioClip;
    [SerializeField] private AudioSource _audioSource;
    public void FireBallAfter()
    {
        _audioSource.PlayOneShot(_audioClip);
        _animator.SetTrigger("DragonFire");
    }
}
