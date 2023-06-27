using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Cell : MonoBehaviour
{
    public Material materialEmpty;
    public Material materialMiss;
    public Material materialShip;
    public Material materialDestroyed;

    public Renderer rend;

    // Ссылка на корабль, находящийся в ячейке
    public List<Ship> ships;
    public ShipCell shipCell;

    //Можно ли кликать?
    public bool isActive = false;

    // Флаг, указывающий, была ли ячейка подвергнута попаданию
    public bool isHit;
    
    //Положение на сетке
    public int gridRow;
    public int gridColumn;
    
    


    public bool IsOccupied =>
        // Если список кораблей не пуст, значит ячейка занята
        ships.Count > 0;

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
            ChangeMaterial(isDestroyed ? materialDestroyed : materialShip);
            return true;
        }
        else
        {
            // Передача хода другому игроку, если игрок попал по пустому полю
            GameManager.Instance.SwitchTurn();
            ChangeMaterial(materialMiss);
            return false;
        }
    }
}