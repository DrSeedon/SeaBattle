using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayingLimit : MonoBehaviour
{
    public UIPanel сhooseMode;
    public TMP_Text remainingMatchesText;  // UI Text to display remaining matches
    public TMP_Text timerText;             // UI Text to display time until next match

    private void Update()
    {
        // Обновляем UI каждый кадр (можно сделать это реже, если нужно)
        UpdateUI();
    }

    public void OpenChooseModeMenu()
    {
        if (CanStartNewMatch())
        {
            сhooseMode.Show();
        }
        else
        {
            Debug.Log("Лимит закончился");
        }
    }

    public bool CanStartNewMatch()
    {
        int currentMatches = PersistentData.Instance.Get("CurrentMatches", 0);
        string lastMatchTimeStr = PersistentData.Instance.Get<string>("LastMatchTime", null);

        if (string.IsNullOrEmpty(lastMatchTimeStr))
        {
            // Если игра еще не началась, можно начать новую партию
            return true;
        }

        DateTime lastMatchTime = DateTime.Parse(lastMatchTimeStr);
        TimeSpan timeSinceLastMatch = DateTime.UtcNow - lastMatchTime;

        if (currentMatches >= 10 && timeSinceLastMatch.TotalHours < 12)
        {
            // Не можем начать новую партию
            return false;
        }

        if (timeSinceLastMatch.TotalHours >= 12)
        {
            // Сброс счетчика, если прошло 12 часов
            PersistentData.Instance.Set("CurrentMatches", 0);
        }

        return true;
    }

    private void UpdateUI()
    {
        int currentMatches = PersistentData.Instance.Get("CurrentMatches", 0);
        string lastMatchTimeStr = PersistentData.Instance.Get<string>("LastMatchTime", null);

        if (string.IsNullOrEmpty(lastMatchTimeStr))
        {
            remainingMatchesText.text = "Оставшиеся матчи: 10";
            timerText.text = "Можно играть!";
            return;
        }

        DateTime lastMatchTime = DateTime.Parse(lastMatchTimeStr);
        TimeSpan timeSinceLastMatch = DateTime.UtcNow - lastMatchTime;
        TimeSpan timeUntilNextMatch = TimeSpan.FromHours(12) - timeSinceLastMatch;

        // Обновляем текстовые поля
        remainingMatchesText.text = $"Оставшиеся матчи: {10 - currentMatches}";
        timerText.text = timeUntilNextMatch.TotalHours >= 12 ? "Можно играть!" : $"Время до следующей игры: {timeUntilNextMatch:hh\\:mm\\:ss}";
    }
}
