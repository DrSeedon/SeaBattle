using System;
using System.Collections.Generic;
using UnityEngine;

public class BattlesHistoryController : MonoBehaviour
{
    private Authentication authService;
    public BattlesHistoryUIView battlesHistoryUIView;
    private int consecutiveWins = 0;
    private int consecutiveLosses = 0;

    private void OnEnable()
    {
        ShowBattlesHistory();
    }

    private void Start()
    {
        authService = Authentication.Instance;
    }

    public void ShowBattlesHistory()
    {
        // Удаление старых записей
        foreach (Transform child in battlesHistoryUIView.battlesHistoryContainer)
        {
            Destroy(child.gameObject);
        }

        int yourUserId = PersistentData.Instance.Get("Id", -1);
        if (yourUserId == -1) return;

        authService.GetBattlesHistory(
            battlesHistory =>
            {
                List<Authentication.BattleEntry> sortedBattles = new List<Authentication.BattleEntry>(battlesHistory);
                sortedBattles.Sort((a, b) => a.id.CompareTo(b.id)); // Сортируем по идентификатору битвы

                foreach (Authentication.BattleEntry battle in sortedBattles)
                {
                    authService.GetUserInfo(battle.first_user_id,
                        firstUserInfo =>
                        {
                            authService.GetUserInfo(battle.second_user_id,
                                secondUserInfo =>
                                {
                                    string winnerName = "Draw";
                                    if (battle.winner_id != 0)
                                    {
                                        winnerName = battle.winner_id == yourUserId ? "You won!" : "You lost!";
                                    }

                                    // Выбираем имя врага в зависимости от того, является ли текущий игрок первым или вторым пользователем
                                    string enemyName = battle.first_user_id == yourUserId ? secondUserInfo.username : firstUserInfo.username;

                                    battlesHistoryUIView.AddBattleEntry(battle.id, enemyName, winnerName);
                                },
                                error => Debug.LogError("Failed to get second user info: " + error));
                        },
                        error => Debug.LogError("Failed to get first user info: " + error));
                }
                
                // Обновляем информацию о подряд идущих победах и поражениях
                UpdateConsecutiveResults(battlesHistory);
            },
            error =>
            {
                Debug.LogError("Failed to get battles history: " + error);
            });
    }

    private void UpdateConsecutiveResults(List<Authentication.BattleEntry> battlesHistory)
    {
        int yourUserId = PersistentData.Instance.Get("Id", -1);
        if (yourUserId == -1) return;

        consecutiveWins = 0;
        consecutiveLosses = 0;

        // Предположим, что battlesHistory отсортирован по времени в порядке убывания (новейшие битвы первые)
        foreach (var battle in battlesHistory)
        {
            if (battle.winner_id == yourUserId)
            {
                consecutiveWins++;
                consecutiveLosses = 0;
            }
            else if (battle.winner_id != 0)  // 0 означает ничью
            {
                consecutiveLosses++;
                consecutiveWins = 0;
            }
        
            if (consecutiveWins >= 3 || consecutiveLosses >= 3) break;
        }

        // Проверяем, давали ли уже награду или штраф за текущую серию побед или поражений
        int rewardedWins = PersistentData.Instance.Get("RewardedConsecutiveWins", 0);
        int rewardedLosses = PersistentData.Instance.Get("RewardedConsecutiveLosses", 0);

        if (consecutiveWins >= 3 && rewardedWins < consecutiveWins)
        {
            // Даем награду
            CurrencyManager.Instance.IncreaseScore(50);
            PersistentData.Instance.Set("RewardedConsecutiveWins", consecutiveWins);
        }
        else if (consecutiveLosses >= 3 && rewardedLosses < consecutiveLosses)
        {
            // Даем штраф
            CurrencyManager.Instance.DecreaseScore(30);
            PersistentData.Instance.Set("RewardedConsecutiveLosses", consecutiveLosses);
        }
    }


}