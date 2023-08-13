using Colyseus.Schema;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField] private Health _health;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private float _maxHeadAngle = 90f;
    [SerializeField] private float _minHeadAngle = -90f;
    [SerializeField] private float _jumpForce = 50f;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private float _jumpDelay = .2f;

    private float _inputH;
    private float _inputV;
    private float _rotateY;
    private float _currentRotateX;

    private float _jumpTime;
    private bool _isSit = false;

    private void Start()
    {
        Transform camera = Camera.main.transform;
        camera.parent = _cameraPoint;
        camera.localPosition = Vector3.zero;
        camera.localRotation = Quaternion.identity;

        _health.SetMax(MaxHealth);
        _health.SetCurrent(MaxHealth);

    }

    public void SetInput(float h, float v, float rotateY)
    {
        _inputH = h;
        _inputV = v;
        _rotateY += rotateY;
    }

    void FixedUpdate()
    {
        Move();
        RotateY();
    }

    private void Move()
    {
        //Vector3 direction = new Vector3(_inputH, 0, _inputV).normalized;
        //transform.position += direction * Time.deltaTime * _speed;

        Vector3 velocity = (transform.forward * _inputV + transform.right * _inputH).normalized * Speed;
        velocity.y = _rigidbody.velocity.y;
        Velocity = velocity;

        _rigidbody.velocity = Velocity;
    }

    private void RotateY()
    {
        _rigidbody.angularVelocity = new Vector3(0, _rotateY, 0);
        _rotateY = 0;
    }

    public void RotateX(float value)
    {
        _currentRotateX = Mathf.Clamp(_currentRotateX + value, _minHeadAngle, _maxHeadAngle);
        _head.localEulerAngles = new Vector3(_currentRotateX, 0, 0);
    }

    public void GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY, out bool isSit)
    {
        position = transform.position;
        velocity = _rigidbody.velocity;

        rotateX = _head.localEulerAngles.x;
        rotateY = transform.eulerAngles.y;

        isSit = _isSit;
    }

    //private bool _isFly = true;
    //private void OnCollisionStay(Collision collision)
    //{
    //    var contactPoints = collision.contacts;
    //    for (int i = 0; i < contactPoints.Length; i++)
    //    {
    //        if (contactPoints[i].normal.y > .45f) _isFly = false;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    _isFly = true;
    //}

    public void Jump()
    {
        //if (_isFly) return;
        if (_checkFly.IsFly) return;
        if (Time.time - _jumpTime < _jumpDelay) return;

        _jumpTime = Time.time;
        _rigidbody.AddForce(0, _jumpForce, 0, ForceMode.VelocityChange);
    }

    public bool TrySit(bool isSit)
    {
        _isSit = isSit;
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

    internal void OnChange(List<DataChange> changes) // 3.2 При получении урона от врага игрок обновляет свой ХП
    {
        foreach (var dataChange in changes)
        {
            switch (dataChange.Field)
            {
                case "loss":
                    MultiplayerManager.Instance.LossCounter.SetPlayerLoss((byte)dataChange.Value);
                    break;
                case "currentHP":
                    _health.SetCurrent((sbyte)dataChange.Value);
                    break;
                    
                default:
                    Debug.LogWarning("Не обрабатывается изменение поля " + dataChange.Field);
                    break;
            }
        }
    }
}
