using Colyseus.Schema;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyCharacter _character;
    [SerializeField] private EnemyGun _gun;
    [SerializeField] private WeaponHolder _weapon;
    private List<float> _recieveTimeInterval = new List<float>() { 0, 0, 0, 0, 0 };
    private float AverageInterval
    {
        get
        {
            int recieveTimeIntervalCount = _recieveTimeInterval.Count;
            float summ = 0;
            for (int i = 0; i < recieveTimeIntervalCount; i++)
            {
                summ += _recieveTimeInterval[i];
            }
            return summ / recieveTimeIntervalCount;
        }
    }
    private float _lastRecieveTime = 0f;
    private Player _player;
    private bool SetSit = false;


    public void Init(string key, Player player)
    {
        _character.Init(key);

        _player = player;
        _character.SetSpeed(_player.speed);
        _character.SetMaxHP(_player.maxHP);
        _player.OnChange += OnChange;
    }

    public void Shoot(in ShootInfo info)
    {
        Vector3 position = new Vector3(info.pX, info.pY, info.pZ);
        Vector3 velocity = new Vector3(info.vX, info.vY, info.vZ);

        _gun.Shoot(position, velocity);
    }


    public void ChangeWeapon(byte weaponIndex)
    {
        _weapon.SetWeaponIndex(weaponIndex);
    }

    public void Destroy()
    {
        _player.OnChange -= OnChange;
        Destroy(gameObject);
    }

    private void SaveReceiveTime()
    {
        float interval = Time.time - _lastRecieveTime;
        _lastRecieveTime = Time.time;

        _recieveTimeInterval.Add(interval);
        _recieveTimeInterval.RemoveAt(0);
    }    

    internal void OnChange(List<DataChange> changes)
    {
        SaveReceiveTime();

        Vector3 position = _character.TargetPosition;
        Vector3 velocity = _character.Velocity;


        foreach (var dataChange in changes)
        {
            switch (dataChange.Field)
            {
                case "loss":
                    MultiplayerManager.Instance.LossCounter.SetEnemyLoss((byte)dataChange.Value);
                    break;
                case "currentHP":
                    if ((sbyte)dataChange.Value > (sbyte)dataChange.PreviousValue)
                        _character.RestorHP((sbyte)dataChange.Value);
                    break;

                case "pX":
                    position.x = (float)dataChange.Value;
                    break;
                case "pY":
                    position.y = (float)dataChange.Value;
                    break;
                case "pZ":
                    position.z = (float)dataChange.Value;
                    break;

                case "vX":
                    velocity.x = (float)dataChange.Value;
                    break;
                case "vY":
                    velocity.y = (float)dataChange.Value;
                    break;
                case "vZ":
                    velocity.z = (float)dataChange.Value;
                    break;

                case "rX":
                    _character.SetRotateX((float)dataChange.Value);
                    break;
                case "rY":
                    _character.SetRotateY((float)dataChange.Value);
                    break;

                case "sit":
                    SetSit = ((bool)dataChange.Value);
                    break;

                default:
                    Debug.LogWarning("Не обрабатывается изменение поля " + dataChange.Field);
                    break;
            }
        }

        _character.SetMovement(position, velocity, AverageInterval, SetSit);
    }
}
