using System.Collections;
using EasyButtons;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 10; // ширина сетки
    public int height = 10; // высота сетки
    public float cellSize = 1.0f; // размер клетки

    public GameObject cellPrefab;

    public GameObject[,] gridArray; // двумерный массив, представляющий сетку
    public bool[,] occupied; // массив, представляющий занятые позиции на сетке

    // создаем сетку игрового поля
    public void CreateGrid()
    {
        gridArray = new GameObject[width, height];
        occupied = new bool[width, height];

        // заполняем сетку пустыми объектами, которые будут отображаться в виде клеток поля
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var cell = Instantiate(cellPrefab, transform);
            cell.name = "Cell [" + x + "," + y + "]";
            cell.transform.localPosition = new Vector3(x * cellSize, 0, y * cellSize);
            gridArray[x, y] = cell;
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
                    if (occupied[posX, posY])
                        return false;
                }
            }
        }

        return true;
    }

    // перегрузка метода OccupyPosition с тремя параметрами
    public void OccupyPosition(int x, int y, bool isOccupied)
    {
        // проверяем, не выходит ли позиция за пределы сетки
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }

        occupied[x, y] = isOccupied;
    }
    
    // очищаем занятые позиции на сетке
    public void ResetGrid()
    {
        occupied = new bool[width, height];
        
        // Сбрасываем ссылки на корабли в ячейках
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = gridArray[x, y].GetComponent<Cell>();
                cell.ship = null;
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

}