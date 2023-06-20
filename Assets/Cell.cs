using System;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    
    public Material materialDefault;
    public Material materialAlt;
    
    public Renderer rend;

    // Ссылка на корабль, находящийся в ячейке
    public List<Ship> ship; 
    public ShipCell shipCell;
    
    // Флаг, указывающий, была ли ячейка подвергнута попаданию
    public bool isHit = false; 
    public bool isOccupied = false; 

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void Occupied(bool value)
    {
        isOccupied = value;
        rend.material = isOccupied ? materialAlt : materialDefault;
        Debug.Log(name + " " + value);
    }

    public void TakeHit()
    {
        if (isHit)
            return;

        isHit = true;
        
        if (ship != null)
        {
            //ship.TakeDamage();
        }
    }

    void OnMouseDown()
    {
        if (isHit)
            return;

        TakeHit();
        Debug.Log("Вы кликнули на " + gameObject.name);
    }
}

