using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryController : MonoBehaviour
{
    private Authentication authService;
    public Transform inventoryContainer; // Ссылка на контейнер для записей инвентаря
    public GameObject inventoryItemPrefab; // Префаб строки инвентаря
    public TMP_InputField addItemInputField; // Поле ввода для добавления элемента
    public TMP_InputField removeItemInputField; // Поле ввода для удаления элемента
    public Button addItemButton; // Кнопка для добавления элемента
    public Button removeItemButton; // Кнопка для удаления элемента
    private void OnEnable()
    {
        authService = Authentication.Instance;
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        authService.GetInventory(DisplayInventory, error => Debug.LogError("Failed to get inventory: " + error));
    }
    
    private void Start()
    {
        addItemButton.onClick.AddListener(OnAddItemButtonClicked);
        removeItemButton.onClick.AddListener(OnRemoveItemButtonClicked);
    }

    private void OnAddItemButtonClicked()
    {
        string itemName = addItemInputField.text;
        if (!string.IsNullOrEmpty(itemName))
        {
            authService.AddItemToInventory(itemName, 
                () =>
                {
                    Debug.Log("Item added successfully");
                    UpdateInventory();
                }, 
                error => Debug.LogError("Failed to add item: " + error));
        }
    }

    private void OnRemoveItemButtonClicked()
    {
        string itemName = removeItemInputField.text;
        if (!string.IsNullOrEmpty(itemName))
        {
            authService.RemoveItemFromInventory(itemName, 
                () =>
                {
                    Debug.Log("Item removed successfully");
                    UpdateInventory();
                }, 
                error => Debug.LogError("Failed to remove item: " + error));
        }
    }
    
    private void DisplayInventory(Dictionary<string, int> inventory)
    {
        // Удаление старых записей
        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }

        // Добавление новых записей
        foreach (var item in inventory)
        {
            // Если количество элемента равно нулю, пропустите эту итерацию
            if (item.Value == 0) continue;

            GameObject newInventoryItem = Instantiate(inventoryItemPrefab, inventoryContainer);
            InventoryItemUI inventoryItemUI = newInventoryItem.GetComponent<InventoryItemUI>();
            inventoryItemUI.Initialize(item.Key, item.Value);
        }
    }
}