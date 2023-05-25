using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameplayManager : MonoBehaviour
{
    public GridManager gridManager;
    public ShipSpawner shipSpawner;

    private void Start()
    {
        gridManager.CreateGrid();
        shipSpawner.SpawnShips();
    }
}