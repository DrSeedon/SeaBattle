using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Material defaultMaterial;
    public Material alternateMaterial;
    private Renderer rend;
    public Ship ship; // ссылка на корабль, находящийся в ячейке
    private bool isHit = false; // флаг, указывающий, была ли ячейка подвергнута попаданию

    public bool IsHit()
    {
        return isHit;
    }

    public void TakeHit()
    {
        if (isHit)
            return;

        isHit = true;

        if (ship != null)
        {
            ship.TakeDamage();
        }
    }

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = defaultMaterial;
    }

    void OnMouseDown()
    {
        if (isHit)
            return;

        TakeHit();

        
        rend.material = alternateMaterial;

        Debug.Log("Вы кликнули на " + gameObject.name);
    }
}

