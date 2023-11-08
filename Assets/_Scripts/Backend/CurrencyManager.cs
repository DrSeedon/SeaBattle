using System;
using UnityEngine;

public class CurrencyManager : StaticInstance<CurrencyManager>
{
    private Authentication authService;

    private void Start()
    {
        authService = Authentication.Instance;
    }

    public void IncreaseScore(int amount)
    {
        authService.IncreaseScore(amount,
            () =>
            {
                Debug.Log("IncreaseScore: " + amount);
                AuthUserActions.Instance.GetPlayerData();
            },
            error => Debug.LogError("Failed to increase score: " + error));
    }

    public void DecreaseScore(int amount)
    {
        authService.DecreaseScore(amount,
            () =>
            {
                Debug.Log("DecreaseScore: " + amount);
                AuthUserActions.Instance.GetPlayerData();
            },
            error => Debug.LogError("Failed to decrease score: " + error));
    }

    public void IncreaseRubies(int amount)
    {
        authService.IncreaseCurrency(amount,
            () =>
            {
                Debug.Log("IncreaseRubies: " + amount);
                AuthUserActions.Instance.GetPlayerData();
            },
            error => Debug.LogError("Failed to increase rubies: " + error));
    }

    public void DecreaseRubies(int amount)
    {
        authService.DecreaseCurrency(amount,
            () =>
            {
                Debug.Log("DecreaseRubies: " + amount);
                AuthUserActions.Instance.GetPlayerData();
            },
            error => Debug.LogError("Failed to decrease rubies: " + error));
    }
}