using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerNetwork : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>("DefaultPlayer",
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> playerId = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    
    public NetworkVariable<int> playerScore = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    
    public PlayerNetwork enemy;
    public BoardManager boardManager;

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += (prev, newValue) =>
        {
            MultiplayerGame.Instance.UpdateName();
        };

        playerId.OnValueChanged += (prev, newValue) => { };

        if (IsOwner)
        {
            playerName.Value = PersistentData.Instance.Get("Username", "Unknown");
            playerId.Value = PersistentData.Instance.Get("Id", 0);
            playerScore.Value = PersistentData.Instance.Get("Score", 0);
        }

        MultiplayerGame.Instance.PlayerAdded(this);
    }
    
    

    //от сервера клиентам
    [ServerRpc(RequireOwnership = false)]
    public void WiningServerRpc(bool value)
    {
        WiningClientRpc(value);
        Debug.Log("Это только сервер????");
    }

    [ClientRpc]
    public void WiningClientRpc(bool value)
    {
        Wining(value);
    }

    private void Wining(bool value)
    {
        if (IsOwner)
        {
            NetworkManagerUI.Instance.AddText($"Wining: {value}");
            NetworkManagerUI.Instance.Disconnect();
        
            // Ваши стандартные очки за победу или поражение
            if (value)
            {
                CurrencyManager.Instance.IncreaseScore(15);
            }
            else
            {
                CurrencyManager.Instance.DecreaseScore(5);
            }

            // Дополнительные очки за победу над игроком с более высоким рейтингом
            if (value && playerScore.Value < enemy.playerScore.Value)
            {
                int bonusPoints = enemy.playerScore.Value - playerScore.Value;
                CurrencyManager.Instance.IncreaseScore(bonusPoints);
            }

            AddBattle(PersistentData.Instance.Get("Id", 0), enemy.playerId.Value, value);
        }
    }


    [Button]
    public void AddBattle(int myUserId, int enemyUserId, bool didIWin)
    {
        int winnerId = didIWin ? myUserId : enemyUserId;

        // Обновление счетчика партий и времени последней партии
        int currentMatches = PersistentData.Instance.Get("CurrentMatches", 0);
        PersistentData.Instance.Set("CurrentMatches", currentMatches + 1);
        PersistentData.Instance.Set("LastMatchTime", DateTime.UtcNow.ToString());

        
        Authentication.Instance.AddBattle(myUserId, enemyUserId, winnerId,
            () => Debug.Log("Battle added successfully"),
            error => Debug.LogError("Failed to add battle: " + error));

    }

    //от клиентов серверу
    [ClientRpc]
    public void PlayerReadyClientRpc(ShipSpawner.ShipData[] shipDataArray)
    {
        List<ShipSpawner.ShipData> shipDataList = new List<ShipSpawner.ShipData>(shipDataArray);
        MultiplayerGame.Instance.PlayerReady(this, shipDataList);
    }

    //от сервера клиентам
    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc(ShipSpawner.ShipData[] shipDataArray)
    {
        PlayerReadyClientRpc(shipDataArray);
    }


    [ServerRpc(RequireOwnership = false)]
    public void NotifyServerOfTurnChangeServerRpc(bool value)
    {
        NotifyServerOfTurnChangeClientRpc(value);
    }

    [ClientRpc]
    public void NotifyServerOfTurnChangeClientRpc(bool value)
    {
        if (IsOwner)
        {
            GameManager.Instance.NotifyClientsOfTurnChange(value);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void NotifyServerOfHitServerRpc(int x, int y)
    {
        NotifyServerOfHitClientRpc(x, y);
    }

    [ClientRpc]
    public void NotifyServerOfHitClientRpc(int x, int y)
    {
        if (IsOwner)
        {
            GameManager.Instance.ReceiveEnemyHit(x, y);
        }
    }
}