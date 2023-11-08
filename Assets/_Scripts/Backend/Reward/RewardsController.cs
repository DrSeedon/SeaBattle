using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardsController : MonoBehaviour
{
    private Authentication authService; // Ссылка на ваш сервис аутентификации
    public Transform rewardsContainer; // Ссылка на контейнер для записей наград
    public GameObject rewardPrefab;    // Префаб строки награды
    public Button claimButton;
    public TMP_Text rewardsCountText;      // Текстовое поле для отображения количества наград

    private void OnEnable()
    {
        // Получение наград пользователя при запуске
        authService.GetRewards(DisplayRewards, error => Debug.LogError("Failed to get rewards: " + error));
    }

    private void Start()
    {
        authService = Authentication.Instance;
        claimButton.onClick.AddListener(ClaimReward);
    }
    
    private void DisplayRewards(List<List<Authentication.Reward>> rewards)
    {
        // Удаление старых записей
        foreach (Transform child in rewardsContainer)
        {
            Destroy(child.gameObject);
        }

        int totalRewards = 0;

        // Добавление новых записей и подсчет общего количества наград
        foreach (List<Authentication.Reward> rewardList in rewards)
        {
            foreach (Authentication.Reward reward in rewardList)
            {
                GameObject newRewardEntry = Instantiate(rewardPrefab, rewardsContainer);
                RewardUIEntry entry = newRewardEntry.GetComponent<RewardUIEntry>();
                entry.Initialize(reward);
                totalRewards++;
            }
        }

        // Обновление текстового поля с количеством наград
        rewardsCountText.text = "Total Rewards: " + totalRewards;
    }

    private void ClaimReward()
    {
        // Выдача награды пользователю
        authService.ClaimRewards(() => Debug.Log("Reward claimed successfully"),
            error => Debug.LogError("Failed to claim reward: " + error));
        authService.GetRewards(DisplayRewards, error => Debug.LogError("Failed to get rewards: " + error));
    }
}