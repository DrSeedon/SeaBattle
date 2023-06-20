using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    private const float minDragDistance = 10f; // Минимальное расстояние для считывания драг-н-дропа

    [Header("Внутренние переменные")] public float yOffset; // Смещение по вертикали (y)

    public bool isDragging;
    public Vector3 offset;

    [Header("Автоматическое заполнение")] public GridManager gridManager; // Ссылка на объект менеджера сетки

    public Ship ship; // Ссылка на объект класса Ship

    public bool isPlacementValid = true;

    private Vector3 initialMousePosition; // Начальная позиция мыши при клике

    private void Start()
    {
        // Находим объект GridManager в сцене
        gridManager = FindObjectOfType<GridManager>();

        // Находим объект класса Ship
        ship = GetComponent<Ship>();
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

    public void OnMouseDown()
    {
        // Запоминаем смещение между позицией объекта и позицией указателя мыши
        offset = ship.transform.position - GetMouseWorldPosition();
        initialMousePosition = Input.mousePosition;
        isDragging = true;
    }

    public void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        Vector3 currentMousePosition = Input.mousePosition;
        var dragDistance = Vector3.Distance(currentMousePosition, initialMousePosition);

        if (dragDistance < minDragDistance)
        {
            // Обновляем ориентацию корабля
            ship.isVertical = !ship.isVertical;

            // Устанавливаем угол поворота корабля в зависимости от его ориентации
            var rotationAngle = ship.isVertical ? 0f : 90f;

            // Поворачиваем корабль на 0 или 90 градусов
            ship.transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }

        // Get the grid position the ship was snapped to
        Vector3Int snappedGridPosition = SnapToGrid();
        // Correct the position of the ship
        CorrectShipPosition(snappedGridPosition);
    }

    private void CorrectShipPosition(Vector3Int snappedGridPosition)
    {
        // Variables to store the largest offset of any cell that is out of bounds
        var largestXOffset = 0;
        var largestZOffset = 0;

        // Check if any part of the ship is out of bounds
        foreach (ShipCell cell in ship.shipCells)
        {
            Vector3Int cellGridPosition = gridManager.GetUnboundedGridPosition(cell.transform.position);

            // If a cell is out of bounds, calculate the offset
            if (cellGridPosition.x < 0)
                largestXOffset = Mathf.Max(largestXOffset, -cellGridPosition.x);
            else if (cellGridPosition.x >= gridManager.width)
                largestXOffset = Mathf.Max(largestXOffset, cellGridPosition.x - gridManager.width + 1);

            if (cellGridPosition.z < 0)
                largestZOffset = Mathf.Max(largestZOffset, -cellGridPosition.z);
            else if (cellGridPosition.z >= gridManager.height)
                largestZOffset = Mathf.Max(largestZOffset, cellGridPosition.z - gridManager.height + 1);
        }

        // Adjust the grid position by the largest offsets
        if (snappedGridPosition.x < 0)
            snappedGridPosition.x += largestXOffset;
        else
            snappedGridPosition.x -= largestXOffset;

        if (snappedGridPosition.z < 0)
            snappedGridPosition.z += largestZOffset;
        else
            snappedGridPosition.z -= largestZOffset;

        // Set the ship's position to the corrected grid position
        ship.transform.position = gridManager.GetWorldPosition(snappedGridPosition.x, snappedGridPosition.z);

        // Update ship's grid coordinates
        ship.x = snappedGridPosition.x;
        ship.y = snappedGridPosition.z;
        
        // Clear previous occupied positions
        foreach (Vector2Int position in ship.occupiedPositions)
            gridManager.OccupyPosition(position.x, position.y, false);

        // Set new occupied positions
        ship.occupiedPositions.Clear();
        for (var i = 0; i < ship.size; i++)
        {
            var offsetX = ship.isVertical ? 0 : i;
            var offsetY = ship.isVertical ? i : 0;
            Vector2Int position = new(ship.x + offsetX, ship.y + offsetY);
            ship.occupiedPositions.Add(position);
            gridManager.OccupyPosition(position.x, position.y, true);
        }
        
        ShipSpawner.Instance.CheckShipsValid();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private Vector3Int SnapToGrid()
    {
        Vector3Int gridPosition = gridManager.GetNearestGridPosition(transform.position);
        ship.transform.position = gridManager.GetWorldPosition(gridPosition.x, gridPosition.z);

        return gridPosition;
    }
}