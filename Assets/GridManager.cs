using System;
using System.Collections;
using EasyButtons;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 10; // ширина сетки
    public int height = 10; // высота сетки
    public float cellSize = 1.0f; // размер клетки

    public Cell cellPrefab;

    public Cell[,] gridArray; // двумерный массив, представляющий сетку

    // создаем сетку игрового поля
    public void CreateGrid()
    {
        gridArray = new Cell[width, height];

        // заполняем сетку пустыми объектами, которые будут отображаться в виде клеток поля
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            Cell cell = Instantiate(cellPrefab, transform);
            cell.name = "Cell [" + x + "," + y + "]";
            cell.transform.localPosition = new Vector3(x * cellSize, 0, y * cellSize);
            cell.gridRow = x;
            cell.gridColumn = y;
            gridArray[x, y] = cell;
        }
    }

    public void SetCellsActive(bool value)
    {
        foreach (Cell cell in gridArray)
        {
            cell.isActive = value;
        }
    }

    public void ChangeAllCellsMaterial(bool value)
    {
        foreach (Cell cell in gridArray)
        {
            cell.ChangeOccupied(value);
        }
    }

    public bool IsPositionFree(int x, int y)
    {
        // Проверка границ поля
        if (x < 0 || x >= width || y < 0 || y >= height)
            return false;

        // Проверка занятости позиций на сетке и их соседей
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int posX = x + i;
                int posY = y + j;

                if (posX >= 0 && posX < width && posY >= 0 && posY < height)
                {
                    if (gridArray[posX, posY].IsOccupied)
                        return false;
                }
            }
        }

        return true;
    }

    public void AddShipFromCell(Ship ship, int x, int y, bool isAdd)
    {
        // проверяем, не выходит ли позиция за пределы сетки
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }

        gridArray[x, y].AddShipToCell(ship, isAdd);
    }

    
    // очищаем занятые позиции на сетке
    public void ResetGrid()
    {
        // Сбрасываем ссылки на корабли в ячейках
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y].ships.Clear();
                gridArray[x, y].ChangeOccupied(gridArray[x, y].IsOccupied);
            }
        }
    }

    // Получаем мировые координаты ячейки на основе ее позиции на сетке
    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x * cellSize, 0f, z * cellSize);
    }

    public Vector3Int GetNearestGridPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        int x = Mathf.RoundToInt(localPosition.x / cellSize);
        int y = Mathf.RoundToInt(localPosition.z / cellSize);
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);
        return new Vector3Int(x, 0, y);
    }
    
    public Vector3Int GetUnboundedGridPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        int x = Mathf.RoundToInt(localPosition.x / cellSize);
        int y = Mathf.RoundToInt(localPosition.z / cellSize);
        return new Vector3Int(x, 0, y);
    }


}