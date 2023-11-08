using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatingUIView : MonoBehaviour
{
    public Transform ratingTableContainer; // Ссылка на контейнер для записей рейтинга
    public GameObject ratingEntryPrefab;   // Префаб строки рейтинга
    
    public Transform eventRatingTableContainer;
    public GameObject ratingEntryUIPrefab;

    public void AddRatingEntry(int rank, string username, int score)
    {
        GameObject newEntry = Instantiate(ratingEntryPrefab, ratingTableContainer);
        TMP_Text entryText = newEntry.GetComponentInChildren<TMP_Text>();
        entryText.text = $"#{rank} Username: {username} - Score: {score}";
    }

    public void AddEventRatingEntry(int rank, string username, int score)
    {
        GameObject newEntry = Instantiate(ratingEntryUIPrefab, eventRatingTableContainer);
        TMP_Text entryText = newEntry.GetComponentInChildren<TMP_Text>();
        entryText.text = $"#{rank} Username: {username} - Score: {score}";
    }
}
