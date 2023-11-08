using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PremiumController : MonoBehaviour
{
    private Authentication authService; // Ссылка на ваш сервис аутентификации
    public TMP_InputField daysInputField; // Поле ввода для ввода количества дней
    public Button setPremiumButton; // Кнопка для установки премиума

    
    private void Start()
    {
        authService = Authentication.Instance;
        // Подписка на событие нажатия кнопки
        setPremiumButton.onClick.AddListener(OnSetPremiumButtonClicked);
    }

    private void OnSetPremiumButtonClicked()
    {
        // Получение количества дней из поля ввода
        if (int.TryParse(daysInputField.text, out int days))
        {
            // Установка премиума на указанное количество дней
            DateTime endDate = DateTime.UtcNow.AddDays(days);
            authService.SetPremiumEndDate(endDate,
                () => Debug.Log("Premium end date set successfully"),
                error => Debug.LogError("Failed to set premium end date: " + error));
        }
        else
        {
            Debug.LogError("Invalid number of days entered!");
        }
    }
}