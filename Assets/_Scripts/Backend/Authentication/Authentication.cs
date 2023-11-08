using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Authentication : StaticInstance<Authentication>
{
    public string serverBaseURL = "https://test.perovx.ru";
    public string loginPath = "/auth/login";
    public string registerPath = "/auth/register";
    public string playerInfoPath = "/info/self/get";
    public string setSelfAuthDatePath = "/info/setSelfAuthDate";
    public string ratingTablePath = "/rating/getTable";
    public string eventRatingTablePath = "/rating/getEventTable";
    public string getUserInfoPath = "/info/get/{id}";
    public string increaseScorePath = "/rating/self/increaseScore";
    public string setPremiumEndDatePath = "/info/premium/self/setEndDate";
    public string increaseCurrencyPath = "/rating/self/increaseCurrency";
    public string getRewardsPath = "/rewards/self/get";
    public string claimRewardsPath = "/rewards/self/claim";
    public string addRewardsPath = "/rewards/self/add";
    public string decreaseScorePath = "/rating/self/decreaseScore";
    public string decreaseCurrencyPath = "/rating/self/decreaseCurrency";
    public string addItemToInventoryPath = "/inventory/self/addItem";
    public string removeItemFromInventoryPath = "/inventory/self/removeItem";
    public string getInventoryPath = "/inventory/self/get";
    public string addBattlePath = "/battles/add";
    public string getServerTimePath = "/utils/getServerTime";
    public string getReferalsPath = "/info/referals/self/get";
    public string getBattlesHistoryPath = "/battles/self/get";



    

    private string LoginURL => serverBaseURL + loginPath;
    private string RegisterURL => serverBaseURL + registerPath;
    private string PlayerInfoURL => serverBaseURL + playerInfoPath;
    private string SetSelfAuthDateURL => serverBaseURL + setSelfAuthDatePath;
    private string RatingTableURL => serverBaseURL + ratingTablePath;
    private string EventRatingTableURL => serverBaseURL + eventRatingTablePath;
    private string GetUserInfoURL(int id) => serverBaseURL + getUserInfoPath.Replace("{id}", id.ToString());
    private string IncreaseScoreURL => serverBaseURL + increaseScorePath;
    private string SetPremiumEndDateURL => serverBaseURL + setPremiumEndDatePath;
    private string IncreaseCurrencyURL => serverBaseURL + increaseCurrencyPath;
    private string GetRewardsURL => serverBaseURL + getRewardsPath;
    private string ClaimRewardsURL => serverBaseURL + claimRewardsPath;
    private string AddRewardsURL => serverBaseURL + addRewardsPath;
    private string DecreaseScoreURL => serverBaseURL + decreaseScorePath;
    private string DecreaseCurrencyURL => serverBaseURL + decreaseCurrencyPath;
    private string AddItemToInventoryURL => serverBaseURL + addItemToInventoryPath;
    private string RemoveItemFromInventoryURL => serverBaseURL + removeItemFromInventoryPath;
    private string GetInventoryURL => serverBaseURL + getInventoryPath;
    private string AddBattleURL => serverBaseURL + addBattlePath;
    private string GetServerTimeURL => serverBaseURL + getServerTimePath;
    private string GetReferalsURL => serverBaseURL + getReferalsPath;
    private string GetBattlesHistoryURL => serverBaseURL + getBattlesHistoryPath;
    
    public void GetBattlesHistory(Action<List<BattleEntry>> onSuccess, Action<string> onError)
    {
        StartCoroutine(WebRequestManager.SendGetRequest(this, GetBattlesHistoryURL,
            successResponse =>
            {
                JObject jsonResponse = JObject.Parse(successResponse);
                List<BattleEntry> battlesHistory = jsonResponse["result"].ToObject<List<BattleEntry>>();
                onSuccess?.Invoke(battlesHistory);
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            },
            needAuth: true));
    }
    
    public void GetReferals(Action<List<int>> onSuccess, Action<string> onError)
    {
        StartCoroutine(WebRequestManager.SendGetRequest(this, GetReferalsURL,
            successResponse =>
            {
                ReferalsResponse referalsResponse = JsonUtility.FromJson<ReferalsResponse>(successResponse);
                onSuccess?.Invoke(referalsResponse.result);
            },
            failResponse => { onError?.Invoke(failResponse); Debug.LogError(failResponse); },
            needAuth: true));
    }
    
    public void GetServerTime(Action<DateTime> onSuccess, Action<string> onError)
    {
        StartCoroutine(WebRequestManager.SendGetRequest(this, GetServerTimeURL,
            successResponse =>
            {
                ServerTimeResponse serverTimeResponse = JsonUtility.FromJson<ServerTimeResponse>(successResponse);
                DateTime serverTime = DateTime.Parse(serverTimeResponse.time);
                onSuccess?.Invoke(serverTime);
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }

    
    public void AddBattle(int firstUserId, int secondUserId, int winnerId, Action onSuccess, Action<string> onError)
    {
        AddBattleRequest addBattleRequest = new AddBattleRequest(firstUserId, secondUserId, winnerId);
        string jsonBody = JsonUtility.ToJson(addBattleRequest);

        StartCoroutine(WebRequestManager.SendPostRequest(this, AddBattleURL, jsonBody,
            successResponse =>
            {
                onSuccess?.Invoke();
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }

    public void AddItemToInventory(string item, Action onSuccess, Action<string> onError)
    {
        AddItemRequest addItemRequest = new AddItemRequest { item = item };
        string jsonBody = JsonUtility.ToJson(addItemRequest);

        StartCoroutine(WebRequestManager.SendPostRequest(this, AddItemToInventoryURL, jsonBody,
            successResponse => { onSuccess?.Invoke(); },
            failResponse => { onError?.Invoke(failResponse); Debug.LogError(failResponse); },
            needAuth: true));
    }

    public void RemoveItemFromInventory(string item, Action onSuccess, Action<string> onError)
    {
        RemoveItemRequest removeItemRequest = new RemoveItemRequest { item = item };
        string jsonBody = JsonUtility.ToJson(removeItemRequest);

        StartCoroutine(WebRequestManager.SendPostRequest(this, RemoveItemFromInventoryURL, jsonBody,
            successResponse => { onSuccess?.Invoke(); },
            failResponse => { onError?.Invoke(failResponse); Debug.LogError(failResponse); },
            needAuth: true));
    }

    public void GetInventory(Action<Dictionary<string, int>> onSuccess, Action<string> onError)
    {
        StartCoroutine(WebRequestManager.SendGetRequest(this, GetInventoryURL,
            successResponse =>
            {
                JObject jsonResponse = JObject.Parse(successResponse);
                Dictionary<string, int> inventory = jsonResponse["result"].ToObject<Dictionary<string, int>>();
                onSuccess?.Invoke(inventory);
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            },
            needAuth: true));
    }
    
    public void AddRewards(List<Reward> rewards, Action onSuccess, Action<string> onError)
    {
        AddRewardsData requestData = new AddRewardsData(rewards);
        string jsonBody = JsonUtility.ToJson(requestData);

        StartCoroutine(WebRequestManager.SendPostRequest(this, AddRewardsURL, jsonBody,
            successResponse => onSuccess?.Invoke(),
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }
    
    public void GetRewards(Action<List<List<Reward>>> onSuccess, Action<string> onError)
    {
        StartCoroutine(WebRequestManager.SendGetRequest(this, GetRewardsURL,
            successResponse =>
            {
                JObject jsonResponse = JObject.Parse(successResponse);
                var rewardsArray = jsonResponse["result"].ToObject<List<List<Reward>>>();
                onSuccess?.Invoke(rewardsArray);
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }

    public void ClaimRewards(Action onSuccess, Action<string> onError)
    {
        StartCoroutine(WebRequestManager.SendGetRequest(this, ClaimRewardsURL,
            successResponse =>
            {
                onSuccess?.Invoke();
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }
    
    public void SetPremiumEndDate(DateTime endDate, Action onSuccess, Action<string> onError)
    {
        // Преобразование DateTime в строку в требуемом формате
        string endDateString = endDate.ToString("yyyy-MM-dd HH:mm:ss");

        SetPremiumEndDateData requestData = new SetPremiumEndDateData(endDateString);
        string jsonBody = JsonUtility.ToJson(requestData);

        StartCoroutine(WebRequestManager.SendPostRequest(this, SetPremiumEndDateURL, jsonBody,
            successResponse =>
            {
                onSuccess?.Invoke();
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }
    
    public void IncreaseScore(int count, Action onSuccess, Action<string> onError)
    {
        IncreaseScoreData requestData = new IncreaseScoreData(count); // Изменение счета
        string jsonBody = JsonUtility.ToJson(requestData);

        StartCoroutine(WebRequestManager.SendPostRequest(this, IncreaseScoreURL, jsonBody,
            successResponse =>
            {
                // Получаем текущий счет из постоянных данных
                int currentScore = PersistentData.Instance.Get<int>("Score");
                // Обновляем значение счета в постоянных данных
                PersistentData.Instance.Set("Score", currentScore + count);
                onSuccess?.Invoke();
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }
    public void IncreaseCurrency(int count, Action onSuccess, Action<string> onError)
    {
        IncreaseCurrencyData requestData = new IncreaseCurrencyData(count);
        string jsonBody = JsonUtility.ToJson(requestData);

        StartCoroutine(WebRequestManager.SendPostRequest(this, IncreaseCurrencyURL, jsonBody,
            successResponse =>
            {
                // Получаем текущий счет из постоянных данных
                int currentCurrency = PersistentData.Instance.Get<int>("Currency");
                // Обновляем значение валюты в постоянных данных
                PersistentData.Instance.Set("Currency", currentCurrency + count);
                onSuccess?.Invoke();
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }

    public void DecreaseScore(int count, Action onSuccess, Action<string> onError)
    {
        // Получаем текущий счет из постоянных данных
        int currentScore = PersistentData.Instance.Get<int>("Score");

        // Вычисляем новый счет, устанавливаем его равным нулю, если он отрицательный
        int newScore = currentScore - count < 0 ? 0 : currentScore - count;

        DecreaseScoreData requestData = new DecreaseScoreData(Mathf.Abs(newScore - currentScore)); // Изменение счета
        string jsonBody = JsonUtility.ToJson(requestData);

        StartCoroutine(WebRequestManager.SendPostRequest(this, DecreaseScoreURL, jsonBody,
            successResponse =>
            {
                // Обновляем значение счета в постоянных данных
                PersistentData.Instance.Set("Score", newScore);
                onSuccess?.Invoke();
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }
    public void DecreaseCurrency(int count, Action onSuccess, Action<string> onError)
    {
        // Получаем текущую валюту из постоянных данных
        int currentCurrency = PersistentData.Instance.Get<int>("Currency");

        // Если уменьшение валюты приведет к отрицательному значению, прерываем выполнение
        if (count > currentCurrency)
        {
            onError?.Invoke("Not enough currency");
            Debug.LogError("Not enough currency");
            return;
        }

        DecreaseCurrencyData requestData = new DecreaseCurrencyData(count);
        string jsonBody = JsonUtility.ToJson(requestData);

        StartCoroutine(WebRequestManager.SendPostRequest(this, DecreaseCurrencyURL, jsonBody,
            successResponse =>
            {
                // Обновляем значение валюты в постоянных данных
                PersistentData.Instance.Set("Currency", currentCurrency - count);
                onSuccess?.Invoke();
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }


    
    public void GetUserInfo(int id, Action<PlayerInfo> onSuccess, Action<string> onError)
    {
        StartCoroutine(WebRequestManager.SendGetRequest(this, GetUserInfoURL(id),
            successResponse =>
            {
                PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(successResponse);
                onSuccess?.Invoke(playerInfo);
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }
    
    public void GetRatingTable(Action<RatingTable> onSuccess, Action<string> onError)
    {
        StartCoroutine(WebRequestManager.SendGetRequest(this, RatingTableURL,
            successResponse =>
            {
                RatingTable ratingTable = JsonUtility.FromJson<RatingTable>(successResponse);
                onSuccess?.Invoke(ratingTable);
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }
    
    public void GetEventRatingTable(Action<EventRatingTable> onSuccess, Action<string> onError)
    {
        StartCoroutine(WebRequestManager.SendGetRequest(this, EventRatingTableURL,
            successResponse =>
            {
                EventRatingTable ratingTable = JsonUtility.FromJson<EventRatingTable>(successResponse);
                onSuccess?.Invoke(ratingTable);
            },
            failResponse =>
            {
                onError?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }


    public void SetSelfAuthDate(string authDate, Action onSuccess, Action<string> onFail)
    {
        AuthDateData requestContent = new AuthDateData(authDate);
        string jsonBody = JsonUtility.ToJson(requestContent);

        StartCoroutine(WebRequestManager.SendPostRequest(this, SetSelfAuthDateURL, jsonBody,
            successResponse =>
            {
                onSuccess?.Invoke();
            },
            onError =>
            {
                onFail?.Invoke(onError);
                Debug.LogError(onError);
            },
            needAuth: true));
    }
    
    public void GetPlayerInfo(Action<PlayerInfo> onSuccess, Action<string> onFail)
    {
        string accessToken = PersistentData.Instance.Get<string>("AccessToken");
        StartCoroutine(WebRequestManager.SendGetRequest(this, PlayerInfoURL,
            successResponse =>
            {
                PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(successResponse);
                onSuccess?.Invoke(playerInfo);
                Debug.Log(successResponse);
            },
            failResponse =>
            {
                onFail?.Invoke(failResponse);
                Debug.LogError(failResponse);
            }, needAuth: true));
    }

    public void Login(string username, string password, Action onSuccess, Action<string> onFail)
    {
        StartCoroutine(SendAuthRequest(LoginURL, username, password, null, onSuccess, onFail));
    }

    public void Register(string username, string password, string referralCode, Action onSuccess, Action<string> onFail)
    {
        StartCoroutine(SendAuthRequest(RegisterURL, username, password, referralCode, onSuccess, onFail));
    }

    private IEnumerator SendAuthRequest(string url, string username, string password, string referralCode, Action onSuccess, Action<string> onFail)
    {
        string jsonBody;

        if (string.IsNullOrEmpty(referralCode))
        {
            LoginData loginData = new LoginData(username, password);
            jsonBody = JsonUtility.ToJson(loginData);
        }
        else
        {
            RegisterData registerData = new RegisterData(username, password, referralCode);
            jsonBody = JsonUtility.ToJson(registerData);
        }

        yield return WebRequestManager.SendPostRequest(this, url, jsonBody,
            successResponse =>
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(successResponse);
                PersistentData.Instance.Set("Username", username);
                PersistentData.Instance.Set("Password", password);
                PersistentData.Instance.Set("AccessToken", response.accessToken);
                PersistentData.Instance.Set("RefreshToken", response.refreshToken);
                onSuccess?.Invoke();
            },
            onError =>
            {
                onFail?.Invoke(onError);
                Debug.LogError(onError);
            });
    }

    [Serializable]
    private class AuthResponse
    {
        public string accessToken;
        public string refreshToken;
    }

    [Serializable]
    public class LoginData
    {
        public string username;
        public string password;

        public LoginData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
    
    [Serializable]
    public class RegisterData
    {
        public string username;
        public string password;
        public string referal_code;

        public RegisterData(string username, string password, string referal_code = null)
        {
            this.username = username;
            this.password = password;
            this.referal_code = referal_code;
        }
    }
    
    [Serializable]
    public class PlayerInfo
    {
        public int id;
        public string username;
        public string self_referal_code;
        public string referal_code;
        public int score;
        public int currency;
        public int event_score;
        public string last_auth_date;
        public string premium_start_date;
        public string premium_end_date;
        public bool premium_active;
    }
    
    [Serializable]
    public class AuthDateData
    {
        public string auth_date;

        public AuthDateData(string authDate)
        {
            this.auth_date = authDate;
        }
    }
    [Serializable]
    public class RatingTable
    {
        public List<RatingEntry> result;
    }

    [Serializable]
    public class RatingEntry
    {
        public int id;
        public int score;
    }
    
    [Serializable]
    public class EventRatingTable
    {
        public List<EventRatingEntry> result;
    }

    [Serializable]
    public class EventRatingEntry
    {
        public int id;
        public int event_score;
    }
    
    [Serializable]
    public class IncreaseScoreData
    {
        public int count;

        public IncreaseScoreData(int count)
        {
            this.count = count;
        }
    }

    [Serializable]
    public class SetPremiumEndDateData
    {
        public string end_date;

        public SetPremiumEndDateData(string endDate)
        {
            this.end_date = endDate;
        }
    }
    
    [Serializable]
    private class IncreaseCurrencyData
    {
        public int count;

        public IncreaseCurrencyData(int count)
        {
            this.count = count;
        }
    }
    
    [Serializable]
    public class RewardList
    {
        public List<List<Reward>> result;
    }

    [Serializable]
    public class Reward
    {
        public string type;
        public string item;
        public int count;
    }

    
    [Serializable]
    private class AddRewardsData
    {
        public List<Reward> reward;

        public AddRewardsData(List<Reward> reward)
        {
            this.reward = reward;
        }
    }
    [Serializable]
    private class DecreaseScoreData
    {
        public int count;

        public DecreaseScoreData(int count)
        {
            this.count = count;
        }
    }
    [Serializable]
    private class DecreaseCurrencyData
    {
        public int count;

        public DecreaseCurrencyData(int count)
        {
            this.count = count;
        }
    }
    [Serializable]
    private class AddItemRequest
    {
        public string item;
    }
    [Serializable]
    private class RemoveItemRequest
    {
        public string item;
    }
    
    [Serializable]
    private class InventoryResponse
    {
        public Dictionary<string, int> result;
    }
    [Serializable]
    private class AddBattleRequest
    {
        public int first_user_id;
        public int second_user_id;
        public int winner_id;

        public AddBattleRequest(int firstUserId, int secondUserId, int winnerId)
        {
            this.first_user_id = firstUserId;
            this.second_user_id = secondUserId;
            this.winner_id = winnerId;
        }
    }
    
    [Serializable]
    public class ServerTimeResponse
    {
        public string time;
    }

    [Serializable]
    private class ReferalsResponse
    {
        public List<int> result;
    }
    
    [Serializable]
    public class BattleEntry
    {
        public int id;
        public int first_user_id;
        public int second_user_id;
        public int winner_id;
    }


}
