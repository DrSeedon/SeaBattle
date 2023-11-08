using System;
using System.Globalization;
using UnityEngine;

public class DailyRewards : MonoBehaviour
{
    private PersistentData persistentData;
    private Authentication authService; // Предположим, что у вас есть ссылка на ваш Authentication-сервис

    private void Start()
    {
        // Получаем экземпляр PersistentData
        persistentData = PersistentData.Instance;
        authService = Authentication.Instance;
        
        // Обновляем ежедневные награды
        UpdateDailyRewards();
    }

    public void UpdateDailyRewards()
    {
        DateTime currentLogin = DateTime.UtcNow;
        DateTime lastLogin = DateTime.ParseExact(persistentData.Get<string>("LastAuthDate", currentLogin.ToString("yyyy-MM-dd HH:mm:ss")), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        TimeSpan timeSinceLastLogin = currentLogin - lastLogin;

        if (timeSinceLastLogin.TotalHours >= 24)
        {
            int consecutiveDays = persistentData.Get<int>("ConsecutiveDays", 0);
            
            if (consecutiveDays < 7)
            {
                consecutiveDays++;
            }

            int rewardPoints = 10 + (consecutiveDays - 1) * 2;

            // Special 7-day reward
            if (consecutiveDays == 7)
            {
                rewardPoints += 10; // Additional 10 points for the 7th day
                // Activate premium mode for 7 days
            }

            CurrencyManager.Instance.IncreaseScore(rewardPoints);

            // Сохраняем новые значения в PersistentData
            persistentData.Set("ConsecutiveDays", consecutiveDays); 

            // Add airstrikes and other bonuses
            AddBonuses(consecutiveDays);
        }
        else if (timeSinceLastLogin.TotalHours > 48)
        {
            // Reset if more than 48 hours have passed
            persistentData.Set("ConsecutiveDays", 0);
        }
        
        // Обновляем UI
        DailyRewardsUIView.Instance.UpdateRewardsUI();
    }
    public void AddBonuses(int days)
    {
        int airStrikes = 1; // По умолчанию 1 авиаудар

        if (days == 4 || days == 7)
        {
            airStrikes = 2; // 2 авиаудара на 4-й и 7-й день
        }

        for (int i = 0; i < airStrikes; i++)
        {
            authService.AddItemToInventory("AirStrike", 
                () => Debug.Log("AirStrike added successfully"), 
                error => Debug.LogError("Failed to add AirStrike: " + error));
        }

        if (days == 4 || days == 7)
        {
            // Добавляем диверсии в инвентарь
            authService.AddItemToInventory("Diversion", 
                () => Debug.Log("Diversion added successfully"), 
                error => Debug.LogError("Failed to add Diversion: " + error));
        }

        if (days == 7)
        {
            // Получаем текущую дату окончания премиума
            DateTime currentPremiumEndDate = DateTime.ParseExact(PersistentData.Instance.Get<string>("PremiumEndDate", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            // Прибавляем 7 дней к текущей дате окончания премиума
            DateTime newPremiumEndDate = currentPremiumEndDate.AddDays(7);

            // Устанавливаем новую дату окончания премиума
            authService.SetPremiumEndDate(newPremiumEndDate,
                () => Debug.Log("Premium end date set successfully"),
                error => Debug.LogError("Failed to set premium end date: " + error));
            
            // Обновляем дату в PersistentData
            PersistentData.Instance.Set("PremiumEndDate", newPremiumEndDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

}
