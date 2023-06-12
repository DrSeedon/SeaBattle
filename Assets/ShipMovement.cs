using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public GridManager gridManager; // Ссылка на объект менеджера сетки
    public float yOffset = 0f; // Смещение по вертикали (y)

    public bool isDragging = false;
    public Vector3 offset;

    public Ship ship; // Ссылка на объект класса Ship

    private void Start()
    {
        // Находим объект GridManager в сцене
        gridManager = FindObjectOfType<GridManager>();

        // Находим объект класса Ship в родительской иерархии
        ship = GetComponentInParent<Ship>();
    }

    public void OnMouseDown()
    {
        // Запоминаем смещение между позицией объекта и позицией указателя мыши
        offset = ship.transform.position - GetMouseWorldPosition();
        isDragging = true;
    }

    public void OnMouseUp()
    {
        isDragging = false;

        // Перемещение объекта на ближайшую свободную позицию на сетке
        SnapToGrid();
    }

    private void Update()
    {
        if (isDragging)
        {
            // Обновляем позицию объекта в соответствии с позицией указателя мыши
            Vector3 targetPosition = GetMouseWorldPosition() + offset;
            targetPosition.y = yOffset; // Задаем смещение по вертикали
            ship.transform.position = targetPosition;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private void SnapToGrid()
    {
        Vector3Int gridPosition = gridManager.GetNearestGridPosition(transform.position);
        Vector3Int freePosition = FindNearestFreePosition(gridPosition);

        if (freePosition != Vector3Int.zero)
        {
            ship.transform.position = gridManager.GetWorldPosition(freePosition.x, freePosition.z);
            gridManager.OccupyPosition(freePosition.x, freePosition.z, true);
        }
        else
        {
            ship.transform.position = gridManager.GetWorldPosition(gridPosition.x, gridPosition.z);
        }
    }

    private Vector3Int FindNearestFreePosition(Vector3Int gridPosition)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int posX = gridPosition.x + i;
                int posZ = gridPosition.z + j;

                if (gridManager.IsPositionFree(posX, posZ))
                {
                    return new Vector3Int(posX, 0, posZ);
                }
            }
        }

        return Vector3Int.zero;
    }
}
