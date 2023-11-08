using System;
using UnityEngine;
using Vertx.Debugging;

public class ShipCell : MonoBehaviour
{
    public Ship ship; // ссылка на родительский корабль
    public ShipMovement shipMovement; // ссылка на родительский корабль
    
    public Material materialDefault;
    public Material materialAlt;
    
    public Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetMaterial(bool value)
    {
        rend.material = value ? materialDefault : materialAlt;
        Debug.Log(name + " " + value);
    }
    
    private void OnMouseDown()
    {
        shipMovement.OnMouseDown();
    }
    
    private void OnMouseUp()
    {
        shipMovement.OnMouseUp();
    }
}