using System.Collections.Generic;
using System.Threading.Tasks;
using EasyButtons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : StaticInstance<GameManager>
{
    public BoardManager boardManagerPlayer;
    public BoardManager boardManagerEnemy;
    public bool isGameStarted = false;

    public int currentPlayerIndex = 0;
    public int differentPlayerIndex => 1 - currentPlayerIndex;
    
    public SmartBot smartBot;
    
    public PlayerNetwork currentPlayer; // Текущий игрок, который делает ход

    public bool isMultiplayerMode = false;

    public UIPanel WinningPanel;
    
    public async void CreateGame()
    {
        await boardManagerPlayer.CreateBoard();
        await boardManagerEnemy.CreateBoard();
        
        // Подписываемся на событие OnEmptyCellHit каждой ячейки
        foreach (Cell cell in boardManagerPlayer.gridManager.gridArray)
        {
            cell.OnEmptyCellHit += SwitchTurn;
            cell.OnCellHit += CheckForEndGame;
        }
        foreach (Cell cell in boardManagerEnemy.gridManager.gridArray)
        {
            cell.OnEmptyCellHit += SwitchTurn;
            cell.OnEmptyCellHit += SendEnemyHit;
            cell.OnCellHit += CheckForEndGame;
            cell.OnCellHit += SendEnemyHit;
        }
        
        //boardManagerEnemy.gameObject.SetActive(false); //offline должен быть закомментирован походу
    }

    public void ReceiveEnemyHit(int x, int y)
    {
        NetworkManagerUI.Instance.AddText($"ReceiveHit: x:{x}, y:{y}");
        Cell targetCell = MultiplayerGame.Instance.mineNetwork.boardManager.gridManager.gridArray[x, y];
        if (targetCell != null)
        {
            targetCell.TakeHitWithoutFeedback();
        }
        else
        {
            Debug.LogError($"No cell found at coordinates ({x},{y})");
        }
    }

    public void SendEnemyHit(int x, int y)
    {
        if (MultiplayerGame.Instance.enemyNetwork != null)
            MultiplayerGame.Instance.enemyNetwork.NotifyServerOfHitServerRpc(x, y);
    }

    public bool CheckVictory()
    {
        foreach (Ship ship in boardManagerEnemy.shipSpawner.ships)
        {
            if (!ship.IsDestroyed())
            {
                return false;
            }
        }

        return true;
    }
    public bool CheckDefeat()
    {
        foreach (Ship ship in boardManagerPlayer.shipSpawner.ships)
        {
            if (!ship.IsDestroyed())
            {
                return false;
            }
        }

        return true;
    }

    [Button]
    public void StartGame()
    {
        isGameStarted = true;
        boardManagerPlayer.StartGame();
        boardManagerEnemy.StartGame();

        boardManagerEnemy.SetShipVisible(false);
        SetTurn(0);
    }


    public void GameShipReady()
    {
        boardManagerPlayer.StartGame();
        boardManagerEnemy.StartGame();
    }
    
    public void StartGameMultiplayer()
    {
        isGameStarted = true;
        
        boardManagerEnemy.SetShipVisible(false);
        boardManagerEnemy.gameObject.SetActive(true);
        boardManagerEnemy.gridManager.ChangeAllCellsMaterial(false);
    }
    
    
    [Button]
    // Функция для переключения хода на другого игрока
    public async void SwitchTurn(int i, int i1)
    {
        if (CheckForEndGameBool()) return;

        if (isMultiplayerMode)
        {
            await Task.Delay(500);
            SetTurn(currentPlayer == MultiplayerGame.Instance.players[0]
                ? MultiplayerGame.Instance.players[1]
                : MultiplayerGame.Instance.players[0]);
            MultiplayerGame.Instance.enemyNetwork.NotifyServerOfTurnChangeServerRpc(true);
        }
        else
        {
            SetTurn(differentPlayerIndex); // Это переключит индекс между 0 и 1
        }
        
    }

    public void Winning(bool value)
    {
        if (value)
        {
            Debug.Log("Поздравляю! Вы победили!");
        }
        else
        {
            Debug.Log("Вы проиграли!");
        }
        
        WinningPanel.Show();

        if (MultiplayerGame.Instance != null)
        {
            MultiplayerGame.Instance.mineNetwork.WiningServerRpc(value);
            MultiplayerGame.Instance.enemyNetwork.WiningServerRpc(!value);
        }
    }

    public bool CheckForEndGameBool()
    {
        if (CheckVictory())
        {
            Winning(true);
            return true;
        }
        if (CheckDefeat())
        {
            Winning(false);
            return true;
        }
        return false;
    }
    public void CheckForEndGame(int i, int i1)
    {
        if (CheckVictory())
        {
            Winning(true);
        }
        if (CheckDefeat())
        {
            Winning(false);
        }
    }

    public async void SetTurn(int value)
    {
        if (!isGameStarted) return;

        boardManagerPlayer.SetCellsActive(false);
        boardManagerEnemy.SetCellsActive(false);

        currentPlayerIndex = value;
        
        if (smartBot != null)
        {
            await Task.Delay(500);
        }

        if (currentPlayerIndex == 0)
        {
            boardManagerPlayer.SetBoardActive(true);
            boardManagerEnemy.SetBoardActive(false);
            if (smartBot != null)
            {
                await Task.Delay(500);
                smartBot.EnemyHit();
            }
        }
        else
        {
            boardManagerPlayer.SetBoardActive(false);
            boardManagerEnemy.SetBoardActive(true);
        }

        boardManagerEnemy.SetCellsActive(true);
    }
    
    public void SetTurn(PlayerNetwork player)
    {
        if (!isGameStarted) return;

        boardManagerPlayer.SetCellsActive(false);
        boardManagerEnemy.SetCellsActive(false);

        currentPlayer = player;
        
        if (player == MultiplayerGame.Instance.mineNetwork) 
        {
            // Это ход локального игрока. Показываем поле врага и разрешаем атаковать его клетки.
            boardManagerEnemy.SetCellsActive(true);
            boardManagerEnemy.SetBoardActive(true);
            boardManagerPlayer.SetBoardActive(false);
        }
        else
        {
            // Это ход врага. Показываем поле локального игрока и запрещаем атаковать клетки.
            boardManagerPlayer.SetCellsActive(false); // запрещаем атаковать клетки локального игрока
            boardManagerPlayer.SetBoardActive(true);
            boardManagerEnemy.SetBoardActive(false);
        }
    }

    public void NotifyClientsOfTurnChange(bool value)
    {
        PlayerNetwork playerNetwork =
            value ? MultiplayerGame.Instance.mineNetwork : MultiplayerGame.Instance.enemyNetwork;
        if (currentPlayer != null)
            NetworkManagerUI.Instance.AddText("Switch Turn: prev: " + currentPlayer.playerName.Value + " New: " +
                                              playerNetwork.playerName.Value);
        else
            NetworkManagerUI.Instance.AddText("Switch Turn: prev: null " + " New: " +
                                              playerNetwork.playerName.Value);
        SetTurn(playerNetwork);
    }

}