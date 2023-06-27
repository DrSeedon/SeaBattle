using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class SmartBot : MonoBehaviour
{
    private enum BotMode 
    {
        Random,
        Search,
        Destroy
    }

    private BotMode mode = BotMode.Random;
    private Cell lastHitCell = null;
    private Cell firstHitCell = null;
    private List<Cell> searchCells = new List<Cell>();
    
    
    public GridManager gridManager;

    public void EnemyHit()
    {
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

            if (cellToHit == null)
            {
                mode = BotMode.Random;
                EnemyHit();
                return;
            }

            bool isHit = cellToHit.TakeHit();
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

                EnemyHit();
            }
            else
            {
                if (mode != BotMode.Random)
                {
                    searchCells.Remove(cellToHit);
                    if (searchCells.Count == 0)
                    {
                        mode = BotMode.Random;
                        lastHitCell = null;
                        firstHitCell = null;
                    }
                }

                GameManager.Instance.SwitchTurn();
            }
        
    }

    private Cell GetRandomCell()
    {
        Cell randomElement;
        do
        {
            int rows = gridManager.gridArray.GetLength(0);
            int cols = gridManager.gridArray.GetLength(1);

            Random random = new Random();
            int randomRow = random.Next(0, rows);
            int randomCol = random.Next(0, cols);

            randomElement = gridManager.gridArray[randomRow, randomCol];

        } while (randomElement.isHit || randomElement.IsOccupied);

        return randomElement;
    }

    private void PopulateSearchCells(Cell hitCell)
    {
        int hitRow = hitCell.gridRow; // вы должны добавить свойства gridRow и gridColumn в класс Cell
        int hitCol = hitCell.gridColumn;

        int[] rowOffsets = {-1, 1, 0, 0};
        int[] colOffsets = {0, 0, -1, 1};

        for(int i = 0; i < 4; i++)
        {
            int newRow = hitRow + rowOffsets[i];
            int newCol = hitCol + colOffsets[i];

            if (newRow >= 0 && newRow < gridManager.gridArray.GetLength(0) && newCol >= 0 && newCol < gridManager.gridArray.GetLength(1))
            {
                Cell cell = gridManager.gridArray[newRow, newCol];
                if (!cell.isHit)
                    searchCells.Add(cell);
            }
        }
    }

    private Cell GetSearchCell()
    {
        if(searchCells.Count > 0)
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
        return GetSearchCell(); // Этот метод будет похож на GetSearchCell, так как мы просто выбираем следующую клетку из списка searchCells
    }
}
