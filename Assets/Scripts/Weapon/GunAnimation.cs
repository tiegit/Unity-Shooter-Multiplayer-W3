using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    private const string _shoot = "Shoot";

    [SerializeField] private Gun _gun;
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _gun.shoot += Shoot;
    }

    private void Shoot()
    {
        _animator.SetTrigger(_shoot);
    }

    private void OnDestroy()
    {
        _gun.shoot -= Shoot;
    }
}
