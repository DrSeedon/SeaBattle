using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class BoardManager : MonoBehaviour
{
    public GridManager gridManager;
    public ShipSpawner shipSpawner;

    public GameObject boardParent;

    /// <summary>
    /// Создает поле и корабли
    /// </summary>
    public async Task CreateBoard()
    {
        gridManager.CreateGrid();
        await shipSpawner.SpawnShips();
    }
    

    /// <summary>
    /// Отключает или включает возможность клика на ячейку
    /// </summary>
    /// <param name="value"></param>
    public void SetCellsActive(bool value)
    {
        gridManager.SetCellsActive(value);
    }

    /// <summary>
    /// Отключает или включает видимость поля
    /// </summary>
    /// <param name="value"></param>
    public void SetBoardActive(bool value)
    {
        boardParent.SetActive(value);
    }
    
    public void StartGame()
    {
        SetShipMovement(false);
        gridManager.ChangeAllCellsMaterial(false);
    }
    
    /// <summary>
    /// Отключает или включает возможность двигать корабли
    /// </summary>
    /// <param name="value"></param>
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

    /// <summary>
    /// Отключает или включает видимость кораблей
    /// </summary>
    /// <param name="value"></param>
    public void SetShipVisible(bool value)
    {
        foreach (Ship ship in shipSpawner.ships)
        {
            ship.shipMesh.SetActive(value);
            foreach (var shipCell in ship.shipCells)
            {
                shipCell.GetComponent<MeshRenderer>().enabled = value;
            }
        }
        //shipSpawner.CheckShipsValid();
    }
    
    
}