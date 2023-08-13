using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f; //здесь нужен Пул объектов добавить
    [SerializeField] private Rigidbody _rigidbody;
    private int _damage;
    public void Init(Vector3 velocity, int damage = 0) // damage = 0 чтобы пули врага не наносили урон, чтобы враг мог наносить урон другому врагу тут надо убрать параметр по умолчнию
    {
        _rigidbody.velocity = velocity;
        _damage = damage;
        StartCoroutine(DelayDestroy());
    }

    private IEnumerator DelayDestroy()
    {
        yield return new WaitForSecondsRealtime(_lifeTime);
        Destroy();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out EnemyCharacter enemy))
        {
            enemy.ApplyDamage(_damage);
        }
        Destroy();
    }
}