using Colyseus;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : ColyseusManager<MultiplayerManager>
{
    [field : SerializeField] public LossCounter LossCounter { get; private set; } // только ссылку получает

    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private EnemyController _enemy;

    private ColyseusRoom<State> _room;// класс на стороне клиента
    private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();

    protected override void Awake()
    {
        base.Awake();

        Instance.InitializeClient();
        Connect();
    }

    private async void Connect()
    {
        Dictionary<string, object> data = new Dictionary<string, object>() 
        {
            { "speed", _player.Speed}, // здесь можно добавить любые данные
            { "hp", _player.MaxHealth}
        };

        _room = await Instance.client.JoinOrCreate<State>("state_handler", data);

        _room.OnStateChange += OnChange;

        _room.OnMessage<string>("Shoot", ApplyShoot);

        _room.OnMessage<string>("Weapon", ApplyWeaponIndex);
    }

    private void ApplyWeaponIndex(string jsonWeaponInfo)
    {
        WeaponInfo weaponInfo = JsonUtility.FromJson<WeaponInfo>(jsonWeaponInfo);
        if (_enemies.ContainsKey(weaponInfo.key) == false)
        {
            Debug.LogError("Врага нет, а он пытается поменять оружие");
            return;
        }

        _enemies[weaponInfo.key].ChangeWeapon(weaponInfo.wI);
    }

    private void ApplyShoot(string jsonShootInfo)
    {
        ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(jsonShootInfo);
        
        if (_enemies.ContainsKey(shootInfo.key) == false)
        {
            Debug.LogError("Врага нет, а он пытается стрелять");
            return;
        }

        _enemies[shootInfo.key].Shoot(shootInfo);
    }

    private void OnChange(State state, bool isFirstState)
    {
        if (isFirstState == false) return;

        state.players.ForEach((key, player) =>
        {
            if (key == _room.SessionId) CreatePlayer(player);
            else CreateEnemy(key, player);
        });

        _room.State.players.OnAdd += CreateEnemy;
        _room.State.players.OnRemove += RemoveEnemy;
    }

    private void CreatePlayer(Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ);

        var playerCharacter = Instantiate(_player, position, Quaternion.identity); // ссылка на игрока
        player.OnChange += playerCharacter.OnChange; // 3.1 Подписка на изменение при создании игрока

        _room.OnMessage<string>("Restart", playerCharacter.GetComponent<PlayerController>().Restart);
    }

    private void CreateEnemy(string key, Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ);

        var enemy = Instantiate(_enemy, position, Quaternion.identity);
        enemy.Init(key, player);

        _enemies.Add(key, enemy);
    }

    private void RemoveEnemy(string key, Player player)
    {
        if (_enemies.ContainsKey(key) == false) return;

        var enemy = _enemies[key];
        enemy.Destroy();

        _enemies.Remove(key);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _room.Leave();
    }

    public void SendMessage(string key, Dictionary<string, object> data) {
        _room.Send(key, data);
    }

    public void SendMessage(string key, string data)
    {
        _room.Send(key, data);
    }

    public string GetSessionID() => _room.SessionId;
}
