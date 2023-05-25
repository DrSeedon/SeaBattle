using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    void OnMouseDown()
    {
        // Здесь можно указать какое-то действие, которое будет выполняться при клике на объекте
        Debug.Log("Вы кликнули на " + gameObject.name);
    }
}
