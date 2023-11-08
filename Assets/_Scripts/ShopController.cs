using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public int rubies; // Для тестирования. В реальной игре, это будет храниться в PersistentData
    public TMP_Text rubiesText;
    public Button buyAirStrikeButton;
    public Button buyDiversionButton;
    private CurrencyManager currencyManager;

    void Start()
    {
        currencyManager = CurrencyManager.Instance;
        rubies = PersistentData.Instance.Get("Currency", 0); // Получаем начальное количество рубинов

        buyAirStrikeButton.onClick.AddListener(() => BuyItem("AirStrike", 1));
        buyDiversionButton.onClick.AddListener(() => BuyItem("Diversion", 1));
        
        rubiesText.text = "Rubies: " + rubies;
    }

    void BuyItem(string itemName, int cost)
    {
        if (rubies >= cost)
        {
            rubies -= cost;
            rubiesText.text = "Rubies: " + rubies;
            currencyManager.DecreaseRubies(cost); // Уменьшаем количество рубинов
            for (int i = 0; i < 5; i++)
            {
                Authentication.Instance.AddItemToInventory(itemName, 
                    () =>
                    {
                        Debug.Log("Bought " + itemName);
                    }, 
                    error => Debug.LogError("Failed to Bought item: " + error));
            }
        }
        else
        {
            Debug.Log("Not enough rubies");
        }
    }
}