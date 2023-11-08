using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class GameMode
{
    public GameObject ModeObject;
    public UnityEvent StartGameEvent;

    public GameMode(GameObject obj)
    {
        ModeObject = obj;
    }

    public void Activate()
    {
        ModeObject.SetActive(true);
    }

    public void Deactivate()
    {
        ModeObject.SetActive(false);
    } 
    
    public void Start()
    {
        StartGameEvent?.Invoke();
    }
    
    
}

public class GameModeUI : MonoBehaviour
{
    public List<GameMode> GameModes;
    public Button nextButton;
    public Button prevButton;
    public Button startGameButton;

    private int currentIndex = 0;

    void Start()
    {
        nextButton.onClick.AddListener(NextMode);
        prevButton.onClick.AddListener(PrevMode);
        startGameButton.onClick.AddListener(StartGame);
    }

    void NextMode()
    {
        GameModes[currentIndex].Deactivate();
        currentIndex = (currentIndex + 1) % GameModes.Count;
        GameModes[currentIndex].Activate();
    }

    void PrevMode()
    {
        GameModes[currentIndex].Deactivate();
        currentIndex = (currentIndex - 1 + GameModes.Count) % GameModes.Count;
        GameModes[currentIndex].Activate();
    }

    void StartGame()
    {
        GameModes[currentIndex].Start();
    }
}
