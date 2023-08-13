using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : Gun
{
    [SerializeField] protected Bullet _bulletPrefab;
    public void Shoot(Vector3 position, Vector3 velocity)
    {
        Instantiate(_bulletPrefab, position, Quaternion.identity).Init(velocity);
        shoot?.Invoke(); //событие для аниматора
    }
}
