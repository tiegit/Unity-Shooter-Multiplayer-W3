using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public Bullet _bulletPrefab;

    public Transform _bulletPoint;
    public float _bulletSpeed = 5f;
    public float _shootDelay = 0.2f;
    public int _damage = 10;
}
