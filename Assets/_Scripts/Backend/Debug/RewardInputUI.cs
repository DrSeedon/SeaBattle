using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardInputUI : MonoBehaviour
{
    public TMP_Dropdown typeDropdown; // Dropdown для выбора типа награды
    public TMP_InputField itemInputField; // Поле ввода для названия предмета
    public TMP_InputField countInputField; // Поле ввода для количества

    // Получение списка наград из интерфейса
    public List<Authentication.Reward> GetRewards()
    {
        List<Authentication.Reward> rewards = new List<Authentication.Reward>();

        // Получение выбранного типа награды
        string selectedType = typeDropdown.options[typeDropdown.value].text;

        // Создание награды на основе выбранного типа
        switch (selectedType)
        {
            case "item":
                rewards.Add(new Authentication.Reward { type = "item", item = itemInputField.text });
                break;
            case "score":
                rewards.Add(new Authentication.Reward { type = "score", count = int.Parse(countInputField.text) });
                break;
            case "currency":
                rewards.Add(new Authentication.Reward { type = "currency", count = int.Parse(countInputField.text) });
                break;
        }

        return rewards;
    }
}