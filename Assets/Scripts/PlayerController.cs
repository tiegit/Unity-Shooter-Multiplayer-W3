using GameDevWare.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private PlayerGun _gun;
    [SerializeField] private WeaponHolder _weapon;
    [SerializeField] private float _mouseSensetivity = 2f;
    private MultiplayerManager _multiplayerManager;
    private bool _hold = false;

    private void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
    }

    private void Update()
    {
        if (_hold) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        bool isShoot = Input.GetMouseButton(0);

        bool space = Input.GetKeyDown(KeyCode.Space);

        bool leftCTRL = Input.GetKey(KeyCode.LeftControl);

        _weapon.SetScrollChange(mouseScroll);

        _player.SetInput(h, v, mouseX * _mouseSensetivity);
        _player.RotateX(-mouseY * _mouseSensetivity);
                
        if (space) _player.Jump();

        _player.TrySit(leftCTRL);

        if (isShoot && _gun.TryShoot(out ShootInfo shootInfo)) SendShoot(ref shootInfo);

        if (mouseScroll != 0)
        {
            WeaponInfo weaponInfo = new WeaponInfo()
            {
                key = _multiplayerManager.GetSessionID(),
                wI = _weapon.GetSelectWeaponIndex()
            };

            SendChangedWeapon(weaponInfo);
        }

        SendMove(); 
    }

    private void SendChangedWeapon(WeaponInfo weaponInfo)
    {
        weaponInfo.key = _multiplayerManager.GetSessionID();

        string json = JsonUtility.ToJson(weaponInfo);

        _multiplayerManager.SendMessage("weapon", json);
    }

    private void SendShoot(ref ShootInfo shootInfo)
    {
        shootInfo.key = _multiplayerManager.GetSessionID();

        string json = JsonUtility.ToJson(shootInfo);

        _multiplayerManager.SendMessage("shoot", json);
    }

    private void SendMove()
    {
        _player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY, out bool isSit);
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "pX", position.x },
            { "pY", position.y },
            { "pZ", position.z },

            { "vX", velocity.x },
            { "vY", velocity.y },
            { "vZ", velocity.z },

            { "rX", rotateX },
            { "rY", rotateY },

            { "sit", isSit}
        };
        _multiplayerManager.SendMessage("move", data);
    }
    
    internal void Restart(string jsonRestartInfo) // перемещаем игрока из последней позиции жестко, не плавно (устанавливая скорость 0)
    {
        RestartInfo info = JsonUtility.FromJson<RestartInfo>(jsonRestartInfo); // берем из json информацию
        StartCoroutine(HoldCorutine()); // блокируем управление
        
        _player.transform.position = new Vector3(info.x, 0, info.z); // перемещаем плеера
        _player.SetInput(0, 0, 0); // перенастраиваем инпут

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "pX", info.x },
            { "pY", 0 },
            { "pZ", info.z },

            { "vX", 0 },
            { "vY", 0 },
            { "vZ", 0 },

            { "rX", 0 },
            { "rY", 0 },

            { "sit", false}
        };
        _multiplayerManager.SendMessage("move", data); // отправляем положение на сервер
    }

    private IEnumerator HoldCorutine()
    {
        _hold = true;
        yield return new WaitForSecondsRealtime(_restartDelay);
        _hold = false;
    }
}

[System.Serializable]
public struct ShootInfo
{
    public string key;

    public float pX;
    public float pY;
    public float pZ;

    public float vX;
    public float vY;
    public float vZ;

    public float rX;
    public float rY;
    public float rZ;
}

[Serializable]
public struct RestartInfo
{
    public float x;
    public float z;
}

[Serializable]
public struct WeaponInfo
{
    public string key;
    public byte wI;
}