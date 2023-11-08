using System;
using System.Collections.Generic;
using UnityEngine;
public class RatingController : MonoBehaviour
{
    private Authentication authService;
    public RatingUIView ratingUIView;

    private void OnEnable()
    {
        ShowRatingTable();
        ShowEventRatingTable();
    }
    
    private void Start()
    {
        authService = Authentication.Instance;
    }

    public void ShowRatingTable()
    {
        // Удаление старых строк
        foreach (Transform child in ratingUIView.ratingTableContainer)
        {
            Destroy(child.gameObject);
        }

        authService.GetRatingTable(
            ratingTable =>
            {
                List<RatingEntryWithUser> fullRatingTable = new List<RatingEntryWithUser>();

                // Подсчет количества ответов
                int countResponses = 0;

                foreach (Authentication.RatingEntry entry in ratingTable.result)
                {
                    authService.GetUserInfo(entry.id,
                        userInfo =>
                        {
                            fullRatingTable.Add(new RatingEntryWithUser { username = userInfo.username, score = entry.score });
                            countResponses++;

                            // Проверка, все ли ответы получены
                            if (countResponses == ratingTable.result.Count)
                            {
                                // Сортировка таблицы по очкам и ID
                                fullRatingTable.Sort((a, b) =>
                                {
                                    int scoreComparison = b.score.CompareTo(a.score);
                                    return scoreComparison != 0 ? scoreComparison : a.id.CompareTo(b.id);
                                });
                                
                                // Отображение отсортированной таблицы
                                int rank = 1; // Переменная для отслеживания текущего рейтинга
                                foreach (RatingEntryWithUser ratingEntry in fullRatingTable)
                                {
                                    ratingUIView.AddRatingEntry(rank, ratingEntry.username, ratingEntry.score);
                                    rank++; // Увеличение рейтинга на 1 для следующей записи
                                }
                            }
                        },
                        error =>
                        {
                            Debug.LogError("Failed to get user info: " + error);
                        });
                }
            },
            error =>
            {
                Debug.LogError("Failed to get rating table: " + error);
            });
    }
    
    public void ShowEventRatingTable()
    {
        // Удаление старых строк
        foreach (Transform child in ratingUIView.eventRatingTableContainer)
        {
            Destroy(child.gameObject);
        }

        authService.GetEventRatingTable(
            ratingTable =>
            {
                List<RatingEntryWithUser> fullEventRatingTable = new List<RatingEntryWithUser>();

                // Подсчет количества ответов
                int countResponses = 0;

                foreach (Authentication.EventRatingEntry entry in ratingTable.result)
                {
                    authService.GetUserInfo(entry.id,
                        userInfo =>
                        {
                            fullEventRatingTable.Add(new RatingEntryWithUser { username = userInfo.username, score = entry.event_score });
                            countResponses++;

                            // Проверка, все ли ответы получены
                            if (countResponses == ratingTable.result.Count)
                            {
                                // Сортировка таблицы по очкам и ID
                                fullEventRatingTable.Sort((a, b) =>
                                {
                                    int scoreComparison = b.score.CompareTo(a.score);
                                    return scoreComparison != 0 ? scoreComparison : a.id.CompareTo(b.id);
                                });
                                
                                int rank = 1; // Переменная для отслеживания текущего рейтинга
                                foreach (RatingEntryWithUser ratingEntry in fullEventRatingTable)
                                {
                                    ratingUIView.AddEventRatingEntry(rank, ratingEntry.username, ratingEntry.score);
                                    rank++; // Увеличение рейтинга на 1 для следующей записи
                                }
                            }
                        },
                        error =>
                        {
                            Debug.LogError("Failed to get user info: " + error);
                        });
                }
            },
            error =>
            {
                Debug.LogError("Failed to get event rating table: " + error);
            });
    }

    [Serializable]
    public struct RatingEntryWithUser
    {
        public int id; // добавим поле ID
        public string username;
        public int score;
    }


}
