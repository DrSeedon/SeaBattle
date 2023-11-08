using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public TMP_Text itemNameText; // Текст для отображения имени элемента
    public TMP_Text itemCountText; // Текст для отображения количества элемента

    public void Initialize(string itemName, int itemCount)
    {
        itemNameText.text = itemName;
        itemCountText.text = itemCount.ToString();
    }
}