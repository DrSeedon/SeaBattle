using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AuthUserActions : StaticInstance<AuthUserActions>
{
    private Authentication authService;
    public AuthUIView authUIView;

    private void Start()
    {
        authService = Authentication.Instance;
        authUIView.loginButton.onClick.AddListener(OnLoginButtonClicked);
        authUIView.registerButton.onClick.AddListener(OnRegisterButtonClicked);
        
        // Попытка автоматического входа, если учетные данные сохранены
        if (PersistentData.Instance.HasKey("Username") && PersistentData.Instance.HasKey("Password"))
        {
            string username = PersistentData.Instance.Get<string>("Username");
            string password = PersistentData.Instance.Get<string>("Password");

            PerformLogin(username, password);
        }
    }
    

    private void OnEnable()
    {
        GetPlayerData();
    }

    public void OnLoginButtonClicked()
    {
        string username = authUIView.usernameField.text;
        string password = authUIView.passwordField.text;

        PerformLogin(username, password);
    }

    private void PerformLogin(string username, string password)
    {
        authService.Login(username, password,
            onSuccess: () =>
            {
                PersistentData.Instance.Set("Username", username);
                PersistentData.Instance.Set("Password", password);
                authUIView.UpdateStatusText("Login Success");

                GetPlayerData();
            },
            onFail: (error) => { authUIView.UpdateStatusText("Login Failed: " + error); });
    }

    public void GetPlayerData()
    {
        // Получение данных игрока после успешного входа в систему
        authService.GetPlayerInfo(
            playerInfo =>
            {
                // Проверка срока окончания премиум аккаунта
                if (playerInfo.premium_end_date != "")
                {
                    DateTime premiumEndDate = DateTime.Parse(playerInfo.premium_end_date);
                    if (DateTime.UtcNow <= premiumEndDate)
                    {
                        playerInfo.premium_active = true;
                    }
                    else
                    {
                        playerInfo.premium_active = false;
                    }
                }

                // Отобразить информацию о игроке в текстовом поле
                authUIView.UpdatePlayerInfoText($"Id: {playerInfo.id}\n" +
                                                $"Username: {playerInfo.username}\n" +
                                                $"Self Referral Code: {playerInfo.self_referal_code}\n" +
                                                $"Referral Code: {playerInfo.referal_code}\n" +
                                                $"Score: {playerInfo.score}\n" +
                                                $"Currency: {playerInfo.currency}\n" +
                                                $"Event Score: {playerInfo.event_score}\n" +
                                                $"Last Auth Date: {playerInfo.last_auth_date}\n" +
                                                $"Premium Start Date: {playerInfo.premium_start_date}\n" +
                                                $"Premium End Date: {playerInfo.premium_end_date}\n" +
                                                $"Premium Active: {playerInfo.premium_active}");
                SavePlayerInfo(playerInfo);
            },
            error => { authUIView.UpdatePlayerInfoText("Failed to get player info: " + error); });
    }

    private void OnDisable()
    {
        // Запись даты последнего входа
        string authDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        authService.SetSelfAuthDate(authDate,
            onSuccess: () => { Debug.Log("Auth date set successfully."); },
            onFail: (error) => { Debug.LogError("Failed to set auth date: " + error); });
    }


    public void OnRegisterButtonClicked()
    {
        string username = authUIView.usernameField.text;
        string password = authUIView.passwordField.text;
        string referralCode = authUIView.referralCodeField.text;

        authService.Register(username, password, referralCode,
            onSuccess: () =>
            {
                CurrencyManager.Instance.IncreaseScore(30);
                // Если реферальный код был предоставлен, увеличиваем счет на 30
                if (!string.IsNullOrEmpty(referralCode))
                {
                    CurrencyManager.Instance.IncreaseScore(30);
                }
            },
            onFail: (error) => { authUIView.UpdateStatusText("Registration Failed: " + error); });
    }


    public void SavePlayerInfo(Authentication.PlayerInfo playerInfo)
    {
        PersistentData.Instance.Set("Id", playerInfo.id);
        PersistentData.Instance.Set("Username", playerInfo.username);
        PersistentData.Instance.Set("SelfReferralCode", playerInfo.self_referal_code);
        PersistentData.Instance.Set("ReferralCode", playerInfo.referal_code);
        PersistentData.Instance.Set("Score", playerInfo.score);
        PersistentData.Instance.Set("Currency", playerInfo.currency);
        PersistentData.Instance.Set("EventScore", playerInfo.event_score);
        PersistentData.Instance.Set("LastAuthDate", playerInfo.last_auth_date);
        PersistentData.Instance.Set("PremiumStartDate", playerInfo.premium_start_date);
        PersistentData.Instance.Set("PremiumEndDate", playerInfo.premium_end_date);
        PersistentData.Instance.Set("PremiumActive", playerInfo.premium_active ? 1 : 0);
    }
}