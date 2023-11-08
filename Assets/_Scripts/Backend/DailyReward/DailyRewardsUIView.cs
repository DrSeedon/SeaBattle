using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DailyRewardsUIView : StaticInstance<DailyRewardsUIView>
{
    public DailyRewardUIEntry rewardItemPrefab; // Префаб элемента награды
    public Transform rewardsContainer; // Контейнер для элементов награды
    private List<DailyRewardUIEntry> rewardItems = new List<DailyRewardUIEntry>(); // Список элементов награды

    private void Start()
    {
        // Создаем элементы награды
        for (int i = 1; i <= 7; i++)
        {
            DailyRewardUIEntry rewardItem = Instantiate(rewardItemPrefab, rewardsContainer);
            rewardItems.Add(rewardItem);
        }

        UpdateRewardsUI();
    }

    public void UpdateRewardsUI()
    {
        int consecutiveDays = PersistentData.Instance.Get<int>("ConsecutiveDays", 0);
        
        for (int i = 0; i < rewardItems.Count; i++)
        {
            rewardItems[i].SetData(i + 1, GetRewardText(i + 1), (i + 1) <= consecutiveDays);
        }
    }

    private string GetRewardText(int day)
    {
        switch (day)
        {
            case 1: return "10 Points + 1 Airstrike";
            case 2: return "12 Points + 1 Airstrike";
            case 3: return "14 Points + 1 Airstrike";
            case 4: return "16 Points + 2 Airstrikes + Diversion";
            case 5: return "18 Points + 1 Airstrike";
            case 6: return "20 Points + 1 Airstrike";
            case 7: return "30 Points + 2 Airstrikes + Diversion + 7-day Premium";
            default: return "Unknown Reward";
        }
    }
}
