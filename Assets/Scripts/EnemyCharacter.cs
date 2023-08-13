using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

public class EnemyCharacter : Character
{
    public Vector3 TargetPosition { get; private set; } = Vector3.zero;
    
    private string _sessionID;

    [SerializeField] private Health _health;
    [SerializeField] private Transform _head;
    //[SerializeField] private float _smoothing = 1f;

    private float _velocityMagnitude = 0f;
    private float _averageInterval;
    private float _rotateX = 0f;
    private float _rotateY = 0f;

    public void Init(string sessionID)
    {
        _sessionID = sessionID;
    }

    private void Start()
    {
        TargetPosition = transform.position;
    }

    private void Update()
    {
        if (_velocityMagnitude > .1f)
        {
            float maxDistance = _velocityMagnitude * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, maxDistance);
        }
        else
        {
            transform.position = TargetPosition; // При движении по прямой и вправо-лево в конце рывок
            //transform.position = Vector3.Lerp(TargetPosition, transform.position, _averageInterval); // а теперь начал скользить
        }
    }

    private void FixedUpdate()
    {
        RotateXY(_rotateX, _rotateY);
    }

    public void SetMaxHP(int value)// устанавливаем здоровье свойства Health от класса родителя Character  
    {
        MaxHealth = value;
        _health.SetMax(value);
        _health.SetCurrent(value);
    }

    public void RestorHP(int newValue)
    {
        _health.SetCurrent(newValue);
    }

    public void SetSpeed(float value) => Speed = value;// устанавливаем скорость свойства Speed от класса родителя Character

    public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval, in bool setSit)
    {
        TargetPosition = position + (velocity * averageInterval);
        _velocityMagnitude = velocity.magnitude;
        _averageInterval = averageInterval;
        Velocity = velocity;

        TrySit(setSit);
    }

    public void ApplyDamage(int damage) // 1. получил урон, отправил на сервер
    {
        _health.ApplyDamage (damage);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"id", _sessionID },
            {"value", damage }
        };

        MultiplayerManager.Instance.SendMessage("damage", data);
    }

    public void SetRotateX(float value)
    {
        _rotateX = value;
    }

    public void SetRotateY(float value)
    {
        _rotateY = value;
    }

    public bool TrySit(bool isSit)
    {
        if (isSit)
        {
            sit?.Invoke();
            return true;
        }
        else
        {
            stand?.Invoke();
            return true;
        }
    }

    private void RotateXY(float x, float y)
    {
        _head.localEulerAngles = new Vector3(x, 0, 0);
        transform.localEulerAngles = new Vector3(0, y, 0);
    }
}
