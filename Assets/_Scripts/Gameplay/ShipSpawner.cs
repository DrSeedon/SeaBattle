using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyButtons;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipSpawner : MonoBehaviour
{
    public GridManager grid; // ссылка на объект сетки игрового поля
    public List<ShipsContainer> shipsContainer;
    public List<Ship> ships;
    public int maxPlacementAttempts = 100;

    public float delay = 0f;

    [Serializable]
    public class ShipsContainer
    {
        public int count;
        public Ship shipPrefab;
    }
    
    [System.Serializable]
    public struct ShipData : INetworkSerializeByMemcpy
    {
        public Vector2Int startPosition;
        public bool isHorizontal; // true if horizontal, false if vertical
        public int size; // размер корабля
    }

    
    public List<ShipData> SerializeShips()
    {
        List<ShipData> shipsData = new List<ShipData>();
        List<Ship> tempShips = new List<Ship>(ships); // Создаем временный список для работы

        while (tempShips.Count > 0)
        {
            Ship ship = tempShips[0]; // Берем первый корабль из списка
            ShipData data = new ShipData();
            data.startPosition = new Vector2Int(ship.x, ship.y);
            data.isHorizontal = !ship.isVertical;
            data.size = ship.size;
            shipsData.Add(data);
            tempShips.RemoveAt(0); // Удаляем корабль из временного списка
        }
        return shipsData;
    }


    public void PlaceShips(List<ShipData> shipsData)
    {
        List<Ship> tempShips = new List<Ship>(ships); // Создаем временный список для работы

        foreach (ShipData data in shipsData)
        {
            Ship ship = FindShipBySize(data.size, tempShips); // Ищем корабль среди тех, что остались в списке
            ship.x = data.startPosition.x;
            ship.y = data.startPosition.y;
            ship.isVertical = !data.isHorizontal;
            ship.PlaceShipOnGrid(ship.x, ship.y);
            tempShips.Remove(ship); // Удаляем корабль из временного списка
        }
        //CheckShipsValid();
    }


    private Ship FindShipBySize(int size, List<Ship> shipsToSearch)
    {
        foreach (Ship ship in shipsToSearch)
        {
            if (ship.size == size)
                return ship;
        }
        return null; // если корабль не найден
    }

    public bool isValidShipPlace = true;
    
    public void CheckShipsValid()
    {
        isValidShipPlace = true; // начнем с предположения, что все корабли размещены правильно

        foreach (Ship ship in ships)
        {
            // Clear previous occupied positions
            foreach (Vector2Int position in ship.occupiedPositions)
                grid.AddShipFromCell(ship, position.x, position.y, false);

            if (!ship.CheckIfValidPosition(ship.x, ship.y))
            {
                isValidShipPlace = false; // если хотя бы один корабль размещен неправильно, устанавливаем в false
            }
            
            foreach (ShipCell cell in ship.shipCells) 
                cell.SetMaterial(ship.CheckIfValidPosition(ship.x, ship.y));

            // Set new occupied positions
            ship.occupiedPositions.Clear();
            for (var i = 0; i < ship.size; i++)
            {
                var offsetX = ship.isVertical ? 0 : i;
                var offsetY = ship.isVertical ? i : 0;
                Vector2Int position = new(ship.x + offsetX, ship.y + offsetY);
                ship.occupiedPositions.Add(position);
                grid.AddShipFromCell(ship, position.x, position.y, true);
            }
        }
    }

    [Button]
    public async Task SpawnShips()
    {
        await SpawnShipsCoroutine();
    }

    // Создаем корабли на сцене с задержкой
    public async Task SpawnShipsCoroutine()
    {
        bool allShipsPlaced = false;
        int tryCount = 0;

        while (!allShipsPlaced)
        {
            tryCount++;

            ClearGrid(); // очищаем поле

            allShipsPlaced = true; // Предполагаем, что все корабли будут успешно размещены

            foreach (var shipContainer in shipsContainer)
            {
                for (int i = 0; i < shipContainer.count; i++)
                {
                    var ship = Instantiate(shipContainer.shipPrefab, transform);
                    ship.name = "Ship " + i + " size: " + ship.size;
                    ship.grid = grid;
                    ship.shipSpawner = this;

                    bool positionFound = ship.SetRandomPosition();

                    if (positionFound)
                    {
                        ship.gameObject.SetActive(true);
                        ships.Add(ship);
                    }
                    else
                    {
                        allShipsPlaced = false; // Если хотя бы один корабль не может быть размещен, помечаем флаг как false
                        Destroy(ship.gameObject);
                        break; // Прерываем цикл для данного типа корабля и переходим к следующему типу
                    }

                    // Задержка в полсекунды между размещениями кораблей
                    await Task.Delay((int)(delay * 1000));
                }

                if (!allShipsPlaced)
                {
                    break; // Прерываем цикл создания кораблей, если хотя бы один корабль не может быть размещен
                }
            }

            if (tryCount >= maxPlacementAttempts)
            {
                Debug.Log("Не удалось разместить все корабли после " + tryCount + " попыток");
                break; // Прерываем цикл размещения кораблей, если превышено максимальное количество попыток
            }
        }

        Debug.Log("Попыток расстановки всех кораблей " + tryCount);
    }
    // очищаем поле и удаляем все корабли
    private void ClearGrid()
    {
        // Удаляем все ранее созданные корабли
        foreach (var ship in this.ships)
        {
            Destroy(ship.gameObject);
        }
        ships.Clear();
        
        // очищаем занятые позиции на сетке
        grid.ResetGrid();
    }
    
}
