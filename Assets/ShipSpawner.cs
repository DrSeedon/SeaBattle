using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
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

    [Button]
    public void SpawnShips()
    {
        StartCoroutine(SpawnShipsCoroutine());
    }
    
    // Создаем корабли на сцене с задержкой
    public IEnumerator SpawnShipsCoroutine()
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

                    yield return new WaitForSeconds(delay); // Задержка в пол секунды между размещениями кораблей
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
        this.ships.Clear();
        
        // удаляем все корабли на сцене
        Ship[] ships = FindObjectsOfType<Ship>();
        foreach (Ship ship in ships)
        {
            Destroy(ship.gameObject);
        }
        
        // очищаем занятые позиции на сетке
        grid.ResetGrid();
    }
    
}
