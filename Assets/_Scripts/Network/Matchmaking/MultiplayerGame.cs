using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerGame : PersistentSingleton<MultiplayerGame>
{
    public NetworkVariable<int> playersCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> readyPlayersCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    
    public PlayerNetwork mineNetwork;
    public PlayerNetwork enemyNetwork;

    public List<PlayerNetwork> players = new List<PlayerNetwork>();

    public GameManager gameManager;

    public void EraseAll()
    {
        playersCount.Value = 0;
        players.Clear();
    }

    public UIPanel multiplayerPanel;
    
    public void PlayerReady()
    {
        ShipSpawner spawner = mineNetwork.boardManager.shipSpawner;
        if (!spawner.isValidShipPlace)
        {
            NetworkManagerUI.Instance.AddText("Корабли размещены неправильно. Пожалуйста, разместите их правильно перед тем, как отметить готовность.");
            return; // Если корабли размещены неправильно, выходим из функции
        }
        multiplayerPanel.Hide();
        var shipDataList = spawner.SerializeShips();
        var shipDataArray = shipDataList.ToArray();
        gameManager.GameShipReady();
        mineNetwork.PlayerReadyServerRpc(shipDataArray);
    }

    public void PlayerReady(PlayerNetwork playerNetwork, List<ShipSpawner.ShipData> shipData)
    {
        Debug.Log("PlayerReady called on: " + (playerNetwork.IsOwner ? "Owner" : "Not Owner"));
    
        // Увеличиваем количество готовых игроков
        readyPlayersCount.Value++;
    
        NetworkManagerUI.Instance.AddText(playerNetwork.playerName.Value + " ready");

        if (!playerNetwork.IsOwner) // Если это данные вражеского игрока
        {
            // Здесь вы можете использовать shipData, чтобы установить корабли врага на вашем поле
            playerNetwork.boardManager.shipSpawner.PlaceShips(shipData);
        }

        // Проверяем, готовы ли все игроки начать игру
        if (readyPlayersCount.Value == playersCount.Value)
        {
            GameStart();
        }
    }

    public void UpdateName()
    {
        if (mineNetwork != null)
            if (enemyNetwork != null)
                NetworkManagerUI.Instance.AddText($"{mineNetwork.playerName.Value} vs {enemyNetwork.playerName.Value}");
    }

    public void PlayerAdded(PlayerNetwork playerNetwork)
    {
        NetworkManagerUI.Instance.AddText($"Player join. Name: {playerNetwork.playerName.Value.ToString()} Id: {playerNetwork.playerId.Value} Score: {playerNetwork.playerScore.Value}");
        playersCount.Value++;
        players.Add(playerNetwork);

        if (playerNetwork.IsOwner)
        {
            mineNetwork = playerNetwork;
        }
        else
        {
            enemyNetwork = playerNetwork;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playersCount.OnValueChanged += (prev, newValue) =>
        {
            NetworkManagerUI.Instance.AddText("Player added. Old count: " + prev + " New: " + newValue);
            CheckValue();
        };
    }
    private void GameCreate()
    {
        NetworkManagerUI.Instance.AddText("Show Game");
        
        mineNetwork.enemy = enemyNetwork;
        enemyNetwork.enemy = mineNetwork;

        gameManager.boardManagerPlayer = mineNetwork.boardManager;
        gameManager.boardManagerEnemy = enemyNetwork.boardManager;

        gameManager.CreateGame();
        
        
        foreach (var cell in enemyNetwork.boardManager.gridManager.gridArray)
        {
            cell.OnDestroyedCellHit += (row, col) =>
            {
                // Добавляем 5 очков в случае уничтожения
                CurrencyManager.Instance.IncreaseScore(5);
                // Добавляем другие бонусы здесь, если необходимо
            };
        }
    }

    public void GameStart()
    {
        gameManager.StartGameMultiplayer();
        
        if (mineNetwork.IsOwnedByServer)
        {
            mineNetwork.NotifyServerOfTurnChangeClientRpc(true);
            enemyNetwork.NotifyServerOfTurnChangeClientRpc(false);
        }
    }

    private int prevValue = 0;
    private async void CheckValue()
    {
        await Task.Delay(200);
        if (prevValue == playersCount.Value) return;
        prevValue = playersCount.Value;
           
        if(playersCount.Value == 1)
            WaitStart();
        if(playersCount.Value == 2)
            GameCreate();
    }
    
    private void WaitStart()
    {
        NetworkManagerUI.Instance.AddText("Show Lobby");
    }
}
