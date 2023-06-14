using System;
using UnityEngine;
using Vertx.Debugging;

public class ShipCell : MonoBehaviour
{
    public Ship ship; // ссылка на родительский корабль
    public ShipMovement shipMovement; // ссылка на родительский корабль
    
    private void OnMouseDown()
    {
        shipMovement.OnMouseDown();
    }
    
    private void OnMouseUp()
    {
        shipMovement.OnMouseUp();
    }
}