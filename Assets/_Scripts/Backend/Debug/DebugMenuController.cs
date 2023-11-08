using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenuController : MonoBehaviour
{
    private Authentication authService; // Ссылка на ваш сервис аутентификации
    
    public TMP_InputField scoreInputField; // Поле ввода для изменения счета
    public Button setScoreButton; // Кнопка для изменения счета
    
    public TMP_InputField currencyInputField; // Поле ввода для изменения валюты
    public Button setCurrencyButton; // Кнопка для изменения валюты
    
    public RewardInputUI rewardInputUI; // Ссылка на интерфейс ввода награды
    public Button addRewardButton;

    private void Start()
    {
        authService = Authentication.Instance;
        // Подписка на события нажатия кнопок
        setScoreButton.onClick.AddListener(OnSetScoreButtonClicked);
        setCurrencyButton.onClick.AddListener(OnSetCurrencyButtonClicked);
        addRewardButton.onClick.AddListener(AddReward);
    }
    
    private void AddReward()
    {
        List<Authentication.Reward> rewards = rewardInputUI.GetRewards();
        authService.AddRewards(rewards,
            () => Debug.Log("Rewards added successfully"),
            error => Debug.LogError("Failed to add rewards: " + error));
    }
    
    private void OnSetScoreButtonClicked()
    {
        // Получение значения изменения счета из поля ввода
        if (int.TryParse(scoreInputField.text, out int scoreDelta))
        {
            if (scoreDelta >= 0)
            {
                authService.IncreaseScore(scoreDelta,
                    () => Debug.Log("Score increased successfully"),
                    error => Debug.LogError("Failed to increase score: " + error));
            }
            else
            {
                authService.DecreaseScore(-scoreDelta,
                    () => Debug.Log("Score decreased successfully"),
                    error => Debug.LogError("Failed to decrease score: " + error));
            }
        }
        else
        {
            Debug.LogError("Invalid score value entered!");
        }
    }

    private void OnSetCurrencyButtonClicked()
    {
        // Получение значения изменения валюты из поля ввода
        if (int.TryParse(currencyInputField.text, out int currencyDelta))
        {
            if (currencyDelta >= 0)
            {
                authService.IncreaseCurrency(currencyDelta,
                    () => Debug.Log("Currency increased successfully"),
                    error => Debug.LogError("Failed to increase currency: " + error));
            }
            else
            {
                authService.DecreaseCurrency(-currencyDelta,
                    () => Debug.Log("Currency decreased successfully"),
                    error => Debug.LogError("Failed to decrease currency: " + error));
            }
        }
        else
        {
            Debug.LogError("Invalid currency value entered!");
        }
    }
}