using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ReferalsController : MonoBehaviour
{
    private Authentication authService;
    public ReferalsUIView referalsUIView;
    private Dictionary<string, int> inventory; // Сохраняем инвентарь

    private void OnEnable()
    {
        ShowReferalsTable();
    }

    private void Start()
    {
        authService = Authentication.Instance;
        UpdateInventory(); // Обновляем инвентарь при старте
    }

    private void UpdateInventory()
    {
        authService.GetInventory(DisplayInventory, error => Debug.LogError("Failed to get inventory: " + error));
    }

    private void DisplayInventory(Dictionary<string, int> updatedInventory)
    {
        inventory = updatedInventory;
    }

    public void ShowReferalsTable()
    {
        foreach (Transform child in referalsUIView.referalsTableContainer)
        {
            Destroy(child.gameObject);
        }

        authService.GetReferals(
            referals =>
            {
                int rewardedReferals = inventory.ContainsKey("RewardedReferalsCount") ? inventory["RewardedReferalsCount"] : 0;
                int newReferals = referals.Count - rewardedReferals;

                if (newReferals > 0)
                {
                    // Добавляем награды за новых рефералов
                    AddReferalRewards(newReferals);

                    for (int i = 0; i < newReferals; i++)
                    {
                        // Обновляем количество вознагражденных рефералов
                        authService.AddItemToInventory("RewardedReferalsCount", 
                            () => {}, 
                            error => {});
                    }
                }
                
                int countResponses = 0;
                foreach (int referalId in referals)
                {
                    authService.GetUserInfo(referalId,
                        userInfo =>
                        {
                            referalsUIView.AddReferalEntry(userInfo.username, userInfo.score);
                            countResponses++;
                            if (countResponses == referals.Count)
                            {
                                // дополнительные действия
                            }
                        },
                        error =>
                        {
                            Debug.LogError("Failed to get user info: " + error);
                        });
                }
            },
            error =>
            {
                Debug.LogError("Failed to get referals: " + error);
            });
    }

    private void AddReferalRewards(int numberOfReferals)
{
    // Увеличить очки
    CurrencyManager.Instance.IncreaseScore(50 * numberOfReferals);

    // Добавить авиаудары и диверсии
    for (int i = 0; i < numberOfReferals; i++)
    {
        authService.AddItemToInventory("AirStrike", 
            () => Debug.Log("AirStrike added successfully"), 
            error => Debug.LogError("Failed to add AirStrike: " + error));
            
        authService.AddItemToInventory("AirStrike", 
            () => Debug.Log("AirStrike added successfully"), 
            error => Debug.LogError("Failed to add AirStrike: " + error));
            
        authService.AddItemToInventory("Diversion", 
            () => Debug.Log("Diversion added successfully"), 
            error => Debug.LogError("Failed to add Diversion: " + error));
    }

    // Проверить, достигнуто ли каждые 10 рефералов
    int totalReferals = numberOfReferals + (inventory.ContainsKey("RewardedReferalsCount") ? inventory["RewardedReferalsCount"] : 0);
    if (totalReferals % 10 == 0)
    {
        // Добавить бонусы за каждые 10 рефералов
        authService.AddItemToInventory("AirStrike", 
            () => Debug.Log("AirStrike added successfully"), 
            error => Debug.LogError("Failed to add AirStrike: " + error));
            
        authService.AddItemToInventory("AirStrike", 
            () => Debug.Log("AirStrike added successfully"), 
            error => Debug.LogError("Failed to add AirStrike: " + error));
            
        authService.AddItemToInventory("Diversion", 
            () => Debug.Log("Diversion added successfully"), 
            error => Debug.LogError("Failed to add Diversion: " + error));

        // Установить премиум режим на 7 дней
        DateTime currentPremiumEndDate = DateTime.ParseExact(PersistentData.Instance.Get<string>("PremiumEndDate", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        DateTime newPremiumEndDate = currentPremiumEndDate.AddDays(7);

        
        authService.SetPremiumEndDate(newPremiumEndDate,
            () => Debug.Log("Premium end date set successfully"),
            error => Debug.LogError("Failed to set premium end date: " + error));
        
        PersistentData.Instance.Set("PremiumEndDate", newPremiumEndDate.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}

}
