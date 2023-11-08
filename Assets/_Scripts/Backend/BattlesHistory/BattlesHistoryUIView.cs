using System;
using TMPro;
using UnityEngine;

public class BattlesHistoryUIView : MonoBehaviour
{
    public Transform battlesHistoryContainer; // Ссылка на контейнер для записей истории боев
    public GameObject battleEntryPrefab;      // Префаб строки истории боя

    public void AddBattleEntry(int id, string enemyName, string winnerName)
    {
        GameObject newEntry = Instantiate(battleEntryPrefab, battlesHistoryContainer);
        TMP_Text entryText = newEntry.GetComponentInChildren<TMP_Text>();
        entryText.text = $"Battle ID: {id} - Enemy: {enemyName} - Winner: {winnerName}";
    }

}
