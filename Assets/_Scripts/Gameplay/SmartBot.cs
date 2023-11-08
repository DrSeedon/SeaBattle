using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyButtons;
using UnityEngine;
using Random = System.Random;

public class SmartBot : MonoBehaviour
{
    public enum BotMode
    {
        Random,
        Search,
        Destroy
    }

    public BotMode mode = BotMode.Random;
    public Cell lastHitCell = null;
    public Cell firstHitCell = null;
    public List<Cell> searchCells = new();

    public Ship curShipToDestroy;

    public BoardManager boardToAttack;
    public GridManager gridManager;

    private void Start()
    {
        gridManager = boardToAttack.gridManager;
    }



    [Button]
    public async void EnemyHit()
    {
        bool isHit;
        do
        {
            await Task.Delay(500);
            Cell cellToHit = null;

            switch (mode)
            {
                case BotMode.Random:
                    cellToHit = GetRandomCell();
                    break;
                case BotMode.Search:
                    cellToHit = GetSearchCell();
                    break;
                case BotMode.Destroy:
                    cellToHit = GetDestroyCell();
                    break;
            }

            if (cellToHit == null) cellToHit = GetRandomCell();

            isHit = cellToHit.TakeHit();
            if (isHit)
            {
                lastHitCell = cellToHit;

                if (mode == BotMode.Random)
                {
                    mode = BotMode.Search;
                    firstHitCell = cellToHit;
                    PopulateSearchCells(cellToHit);
                }
                else if (mode == BotMode.Search)
                {
                    if (cellToHit.ships[0].IsDestroyed())
                    {
                        mode = BotMode.Random;
                        searchCells.Clear();
                        lastHitCell = null;
                        firstHitCell = null;
                    }
                    else
                    {
                        mode = BotMode.Destroy;
                        PopulateDestroyCells();
                    }
                }
                else if (mode == BotMode.Destroy)
                {
                    if (cellToHit.ships[0].IsDestroyed())
                    {
                        mode = BotMode.Random;
                        searchCells.Clear();
                        lastHitCell = null;
                        firstHitCell = null;
                    }
                }
            }

            if (!isHit && mode != BotMode.Random)
            {
                searchCells.Remove(cellToHit);
                if (searchCells.Count == 0)
                {
                    mode = BotMode.Random;
                    lastHitCell = null;
                    firstHitCell = null;
                }
            }
        } while (isHit);
    }


    private Cell GetRandomCell()
    {
        Cell randomElement = new();
        do
        {
            if (gridManager.IsGridFull()) break; // Завершаем цикл, если сетка полностью заполнена

            var rows = gridManager.gridArray.GetLength(0);
            var cols = gridManager.gridArray.GetLength(1);

            Random random = new();
            var randomRow = random.Next(0, rows);
            var randomCol = random.Next(0, cols);

            randomElement = gridManager.gridArray[randomRow, randomCol];
        } while (randomElement.isHit);

        return randomElement;
    }

    private void PopulateSearchCells(Cell hitCell)
    {
        var hitRow = hitCell.gridRow; // вы должны добавить свойства gridRow и gridColumn в класс Cell
        var hitCol = hitCell.gridColumn;

        int[] rowOffsets = { -1, 1, 0, 0 };
        int[] colOffsets = { 0, 0, -1, 1 };

        for (var i = 0; i < 4; i++)
        {
            var newRow = hitRow + rowOffsets[i];
            var newCol = hitCol + colOffsets[i];

            if (newRow >= 0 && newRow < gridManager.gridArray.GetLength(0) && newCol >= 0 &&
                newCol < gridManager.gridArray.GetLength(1))
            {
                Cell cell = gridManager.gridArray[newRow, newCol];
                if (!cell.isHit)
                    searchCells.Add(cell);
            }
        }
    }

    private Cell GetSearchCell()
    {
        if (searchCells.Count > 0)
        {
            Cell cell = searchCells[0];
            searchCells.RemoveAt(0);
            return cell;
        }
        else
        {
            return null;
        }
    }

    private void PopulateDestroyCells()
    {
        // Здесь мы пытаемся определить направление корабля и добавить клетки вдоль этого направления в searchCells
        // Это может быть сложно, если у вас нет информации о расположении корабля
        // Но вы можете попытаться сделать это, сравнивая gridRow и gridColumn последней и первой попадающих клеток
    }

    private Cell GetDestroyCell()
    {
        return
            GetSearchCell(); // Этот метод будет похож на GetSearchCell, так как мы просто выбираем следующую клетку из списка searchCells
    }
}