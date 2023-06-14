using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    
    [Header("Внутренние переменные")]
    public float yOffset = 0f; // Смещение по вертикали (y)
    public bool isDragging = false;
    public Vector3 offset;
  
    [Header("Автоматическое заполнение")]
    public GridManager gridManager; // Ссылка на объект менеджера сетки
    public Ship ship; // Ссылка на объект класса Ship
    
    private void Start()
    {
        // Находим объект GridManager в сцене
        gridManager = FindObjectOfType<GridManager>();

        // Находим объект класса Ship
        ship = GetComponent<Ship>();
    }

    public void OnMouseDown()
    {
        // Запоминаем смещение между позицией объекта и позицией указателя мыши
        offset = ship.transform.position - GetMouseWorldPosition();
        LogHelper.Instance.LogText(() => offset);
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
            LogHelper.Instance.LogText(() => targetPosition);
            ship.transform.position = targetPosition;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        LogHelper.Instance.LogText(() => mousePosition);
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private void SnapToGrid()
    {
        Vector3Int gridPosition = gridManager.GetNearestGridPosition(transform.position);
        LogHelper.Instance.LogText(() => gridPosition);
        ship.transform.position = gridManager.GetWorldPosition(gridPosition.x, gridPosition.z);
    }
}
