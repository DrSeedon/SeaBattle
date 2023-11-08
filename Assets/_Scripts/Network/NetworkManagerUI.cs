using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : StaticInstance<NetworkManagerUI>
{
    public TMP_Text Text;
    
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SingletonOnOnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += SingletonOnOnClientDisconnectCallback;
        NetworkManager.Singleton.OnServerStarted += SingletonOnOnServerStarted;
        NetworkManager.Singleton.OnTransportFailure += SingletonOnOnTransportFailure;
    }
    

    private void SingletonOnOnTransportFailure()
    {
        AddText("SingletonOnOnTransportFailure");
        Disconnect();
    }

    private void SingletonOnOnServerStarted()
    {
        AddText("SingletonOnOnServerStarted");
    }

    private void SingletonOnOnClientDisconnectCallback(ulong obj)
    {
        AddText("SingletonOnOnClientDisconnectCallback " + obj);
        Disconnect();
    }

    private void SingletonOnOnClientConnectedCallback(ulong obj)
    {
        AddText("SingletonOnOnClientConnectedCallback " + obj);
    }

    public void AddText(string text)
    {
        Text.text += text + "\n";
    }

    public void StartHost()
    {
        AddText("StartHost");
        NetworkManager.Singleton.StartHost();        
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    public ulong id;
    public UIPanel UIPanel;
    public void Disconnect()
    {
        //NetworkManager.Singleton.DisconnectClient(id);
        if (SimpleMatchmaking.Instance != null) SimpleMatchmaking.Instance.DestroyLobby();
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
        if (MultiplayerGame.Instance != null) MultiplayerGame.Instance.EraseAll();
        UIPanel.Show();
    }

    public void StartClient()
    {
        AddText("StartClient");
        NetworkManager.Singleton.StartClient();
    }

    protected override void OnApplicationQuit()
    {
        Disconnect();
        base.OnApplicationQuit();
    }

}
