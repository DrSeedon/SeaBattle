using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public int size; // размер корабля
    public GridManager grid; // ссылка на объект сетки игрового поля
    List<Vector2Int> occupiedPositions;

    private bool isVertical; // корабль расположен вертикально или горизонтально?
    private int x; // координата x корабля на сетке
    private int y; // координата y корабля на сетке

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
    
    private bool CheckIfValidPosition(int startX, int startY)
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
    
    // размещаем корабль на сетке
    private void PlaceShipOnGrid(int startX, int startY)
    {
        for (int i = 0; i < size; i++)
        {
            int offsetX = isVertical ? 0 : i;
            int offsetY = isVertical ? i : 0;

            Vector2Int position = new Vector2Int(startX + offsetX, startY + offsetY);
            occupiedPositions.Add(position);
            grid.OccupyPosition(position.x, position.y, true);
        }

        // устанавливаем позицию и поворот корабля в соответствии с размещением на сетке
        transform.position = new Vector3(startX * grid.cellSize, 0, startY * grid.cellSize);
        transform.rotation = Quaternion.Euler(0, isVertical ? 0 : 90, 0);
    }
}
