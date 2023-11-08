using System;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Material materialEmpty;
    public Material materialMiss;
    public Material materialShip;
    public Material materialDestroyed;

    public Renderer rend;

    public List<Ship> ships;
    public ShipCell shipCell;
    public bool isActive = false;
    public bool isHit;
    public int gridRow;
    public int gridColumn;

    public bool IsOccupied => ships.Count > 0;

    public event Action<int, int> OnEmptyCellHit; // Событие, вызываемое при попадании по пустой ячейке
    public event Action<int, int> OnCellHit;      // передает координаты
    public event Action<int, int> OnDestroyedCellHit; // Событие, вызываемое при уничтожении ячейки


    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void OnMouseDown()
    {
        if (!isActive) return;
        TakeHit();
        Debug.Log("Вы кликнули на " + gameObject.name);
    }

    public void AddShipToCell(Ship ship, bool isAdd)
    {
        if (isAdd)
            ships.Add(ship);
        else
            ships.Remove(ship);
        ChangeOccupied(IsOccupied);
    }

    public void ChangeOccupied(bool value)
    {
        rend.material = value ? materialMiss : materialEmpty;
    }

    public void ChangeMaterial(Material value)
    {
        rend.material = value;
    }

    public bool TakeHit()
    {
        if (isHit) return false;

        isHit = true;
        

        if (IsOccupied)
        {
            var isDestroyed = ships[0].TakeDamage();
            
            // Если корабль уничтожен и размер корабля равен 1
            if (isDestroyed && ships[0].size == 1)
            {
                OnDestroyedCellHit?.Invoke(gridRow, gridColumn);
            }
            
            OnCellHit?.Invoke(gridRow, gridColumn);
            
            ChangeMaterial(isDestroyed ? materialDestroyed : materialShip);
            return true;
        }
        else
        {
            // Вызываем событие OnEmptyCellHit, передавая текущий объект ячейки
            OnEmptyCellHit?.Invoke(gridRow, gridColumn);
            ChangeMaterial(materialMiss);
            return false;
        }
    }
    
    public bool TakeHitWithoutFeedback()
    {
        if (isHit) return false;

        isHit = true;
        

        if (IsOccupied)
        {
            var isDestroyed = ships[0].TakeDamage();
            
            ChangeMaterial(isDestroyed ? materialDestroyed : materialShip);
            return true;
        }
        else
        {
            ChangeMaterial(materialMiss);
            return false;
        }
    }
    
    
}