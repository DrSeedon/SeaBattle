using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;
using Vertx.Debugging;

public class Ship : MonoBehaviour
{
    
    [Header("Обязательное заполнение")] 
    
    public List<ShipCell> shipCells = new List<ShipCell>();
    public GameObject shipMesh;

    [Header("Внутренние переменные")]
    public int size; // размер корабля
    public List<Vector2Int> occupiedPositions;
    public bool isVertical; // корабль расположен вертикально или горизонтально?
    public int x; // координата x корабля на сетке
    public int y; // координата y корабля на сетке
    public int health; // здоровье корабля
    
    public List<Cell> destroyedCells;


    [Header("Автоматическое заполнение")]
    public GridManager grid; // ссылка на объект сетки игрового поля
    public ShipSpawner shipSpawner; // ссылка на объект сетки игрового поля
    public ShipMovement shipMovement;

    [Button]
    public void GatherShipCells() {
        shipCells = new List<ShipCell>(GetComponentsInChildren<ShipCell>());
    }

    #region PublicMethod

    private void PopulateDestroyCells()
    {
        // Очищаем список разрушенных клеток перед заполнением.
        destroyedCells.Clear();

        foreach (Vector2Int pos in occupiedPositions)
        {
            // Получаем клетку на позиции pos
            Cell cell = grid.gridArray[pos.x, pos.y];

            // Добавляем клетку в список разрушенных клеток
            destroyedCells.Add(cell);

            // Получаем соседние клетки
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    // Проверяем, не выходят ли соседние клетки за пределы сетки
                    if ((pos.x + i) >= 0 && (pos.x + i) < grid.width && (pos.y + j) >= 0 && (pos.y + j) < grid.height)
                    {
                        Cell neighbourCell = grid.gridArray[pos.x + i, pos.y + j];

                        // Добавляем соседние клетки в список разрушенных клеток
                        destroyedCells.Add(neighbourCell);
                    }
                }
            }
        }
    }
    
    public bool TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            PopulateDestroyCells();

            foreach (Cell destroyedCell in destroyedCells)
            {
                // Проверка чтобы не помечать уже уничтоженные корабли
                if (!destroyedCell.isHit)
                {
                    destroyedCell.ChangeMaterial(destroyedCell.materialMiss);
                    destroyedCell.isHit = true; // Пометим ячейку как уничтоженную
                }
            }
            
            
            for (var i = 0; i < size; i++)
            {
                var offsetX = isVertical ? 0 : i;
                var offsetY = isVertical ? i : 0;
                Vector2Int position = new(x + offsetX, y + offsetY);
                Cell cell = grid.gridArray[position.x, position.y];
                cell.ChangeMaterial(cell.materialDestroyed);
            }

            // Делаем корабль невидимым, а не удаляем его
            gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    public bool IsDestroyed()
    {
        // Проверяем, был ли корабль уничтожен
        return health <= 0;
    }

    // устанавливаем корабль на случайной свободной позиции на сетке
    public bool SetRandomPosition()
    {
        occupiedPositions = new List<Vector2Int>();

        List<Vector2Int> availablePositions = GetAllAvailablePositions();
        ShuffleList(availablePositions);

        foreach (Vector2Int position in availablePositions)
        {
            for (int rotation = 0; rotation < 2; rotation++)
            {
                Debug.Log("Попытка поставить корабль " + name);
                x = position.x;
                y = position.y;
                isVertical = rotation == 0;

                if (CheckIfValidPosition(x, y))
                {
                    PlaceShipOnGrid(x, y);
                    return true;
                }
            }
        }

        return false;
    }


    #endregion
    
    #region PrivateMethod

    
    // Получаем список всех доступных позиций на сетке
    private List<Vector2Int> GetAllAvailablePositions()
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        for (int i = 0; i < grid.width; i++)
        {
            for (int j = 0; j < grid.height; j++)
            {
                availablePositions.Add(new Vector2Int(i, j));
            }
        }

        return availablePositions;
    }
    
    // Перемешиваем список случайным образом
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public bool CheckIfValidPosition(int startX, int startY)
    {
        List<Vector2Int> positionsToCheck = GetOccupiedPositionsToCheck(startX, startY);

        foreach (Vector2Int position in positionsToCheck)
        {
            if (!grid.IsPositionFree(position.x, position.y))
            {
                return false;
            }
        }

        return true;
    }

    private List<Vector2Int> GetOccupiedPositionsToCheck(int startX, int startY)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int i = 0; i < size; i++)
        {
            int offsetX = isVertical ? 0 : i;
            int offsetY = isVertical ? i : 0;

            Vector2Int position = new Vector2Int(startX + offsetX, startY + offsetY);
            positions.Add(position);
        }

        return positions;
    }

    public void PlaceShipOnGrid(int startX, int startY)
    {
        health = size; // здоровье корабля равно его размеру
        shipMovement = GetComponent<ShipMovement>();
        
        for (int i = 0; i < size; i++)
        {
            int offsetX = isVertical ? 0 : i;
            int offsetY = isVertical ? i : 0;

            Vector2Int position = new Vector2Int(startX + offsetX, startY + offsetY);
            occupiedPositions.Add(position);
            grid.AddShipFromCell(this, position.x, position.y, true);
            
            // Обновляем ссылку на Cell в соответствующей ShipCell
            ShipCell shipCell = shipCells[i];
            shipCell.ship = this;
            shipCell.shipMovement = shipMovement;

            // Добавляем ссылку на корабль в каждую ячейку
            Cell cell = grid.gridArray[position.x, position.y].GetComponent<Cell>();
            cell.shipCell = shipCell;
        }

        // устанавливаем позицию и поворот корабля в соответствии с размещением на сетке
        transform.position = new Vector3(startX * grid.cellSize, 0, startY * grid.cellSize);
        transform.rotation = Quaternion.Euler(0, isVertical ? 0 : 90, 0);
    }



    #endregion
    
}
