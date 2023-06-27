using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class BoardManager : MonoBehaviour
{
    public GridManager gridManager;
    public ShipSpawner shipSpawner;
    public SmartBot smartBot;

    public GameObject boardParent;

    public bool isEnemy = false;

    public async Task CreateBoard()
    {
        gridManager.CreateGrid();
        await shipSpawner.SpawnShips();
    }

    public void SetCellsActive(bool value)
    {
        gridManager.SetCellsActive(value);
    }

    public void SetBoardActive(bool value)
    {
        boardParent.SetActive(value);
        if (value) EnemyHit();
    }

    public async void EnemyHit()
    {
        if (smartBot!=null)
        {
            await Task.Delay(500);
            smartBot.EnemyHit();
        }
        
    }
    
    public void StartGame()
    {
        SetShipMovement(false);
        gridManager.ChangeAllCellsMaterial(false);
        if(isEnemy)SetShipVisible(false);
    }
    
    // Функция для управления возможностью двигать корабли
    public void SetShipMovement(bool value)
    {
        foreach (Ship ship in shipSpawner.ships)
        {
            foreach (var shipCell in ship.shipCells)
            {
                shipCell.GetComponent<BoxCollider>().enabled = value;
            }
            
            /*
            var shipMovement = ship.GetComponent<ShipMovement>();
            if (shipMovement != null)
            {
                shipMovement.isCanMove = value;
            }
            */
        }
    }

    public void SetShipVisible(bool value)
    {
        foreach (Ship ship in shipSpawner.ships)
        {
            foreach (var shipCell in ship.shipCells)
            {
                shipCell.GetComponent<MeshRenderer>().enabled = value;
            }
        }
        
    }
}