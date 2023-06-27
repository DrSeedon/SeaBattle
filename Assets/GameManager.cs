using System.Collections.Generic;
using System.Threading.Tasks;
using EasyButtons;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    public List<BoardManager> boardManagers;
    public bool isGameStarted = false;
    
    public int currentPlayerIndex = 0;
    public int differentPlayerIndex => 1 - currentPlayerIndex;

    // Функция, вызываемая при старте игры
    async void Start()
    {
        foreach (BoardManager boardManager in boardManagers)
        {
            await boardManager.CreateBoard();
            //boardManager.SetBoardActive(false);
        }
    }

    [Button]
    public void GameStart()
    {
        isGameStarted = true;
        foreach (BoardManager boardManager in boardManagers)
        {
            boardManager.StartGame();
        }
        SetTurn(1);
    }
    
    

    // Функция для переключения хода на другого игрока
    public void SwitchTurn()
    {
        SetTurn(differentPlayerIndex); // Это переключит индекс между 0 и 1
    }

    public async void SetTurn(int value)
    {
        if (!isGameStarted) return;
        
        foreach (BoardManager boardManager in boardManagers)
        {
            boardManager.SetCellsActive(false);
        }

        currentPlayerIndex = value;
        Debug.Log("Ход передается игроку " + currentPlayerIndex);

        await Task.Delay(500);
        
        boardManagers[differentPlayerIndex].SetBoardActive(false);
        boardManagers[currentPlayerIndex].SetBoardActive(true);

        foreach (BoardManager boardManager in boardManagers)
        {
            if (boardManager.isEnemy)
                boardManager.SetCellsActive(true);
        }
    }
}