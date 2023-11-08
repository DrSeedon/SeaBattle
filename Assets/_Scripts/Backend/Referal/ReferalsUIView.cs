using TMPro;
using UnityEngine;

public class ReferalsUIView : MonoBehaviour
{
    public Transform referalsTableContainer; // Ссылка на контейнер для записей рефералов
    public GameObject referalEntryPrefab;   // Префаб строки реферала

    public void AddReferalEntry(string username, int score)
    {
        GameObject newEntry = Instantiate(referalEntryPrefab, referalsTableContainer);
        TMP_Text entryText = newEntry.GetComponentInChildren<TMP_Text>();
        entryText.text = $"Username: {username} - Score: {score}";
    }
}