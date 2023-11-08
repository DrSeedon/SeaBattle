using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SimpleMatchmaking : StaticInstance<SimpleMatchmaking> {

    public Lobby _connectedLobby;
    public QueryResponse _lobbies;
    public UnityTransport _transport;
    public const string JoinCodeKey = "j";
    public string _playerId;

    public Lobby lobby;
    
    protected override async void Awake()
    {
        base.Awake();
        _transport = FindObjectOfType<UnityTransport>();
    }

    public async void CreateOrJoinLobby() {
        await Authenticate();
        
        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();

        //if (_connectedLobby != null) _buttons.SetActive(false);
    }

    public async Task Authenticate() {
        var options = new InitializationOptions();
        
        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _playerId = AuthenticationService.Instance.PlayerId;
    }
    
    /*
    public async void CreateButtonLobby()
    {
        foreach (var button in joinLobbyButtons)
        {
            Destroy(button);
        }
        joinLobbyButtons.Clear();

        var lobbys = await Lobbies.Instance.QueryLobbiesAsync();
        
        // создаем кнопки для каждого лобби
        foreach (Lobby lobby in lobbys.Results)
        {
            // создаем кнопку из префаба
            GameObject button = Instantiate(buttonPrefab, buttonParent);
            joinLobbyButtons.Add(button);
            // настраиваем текст на кнопке
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = lobby.Name;

            // настраиваем обработчик клика на кнопке
            Button buttonComponent = button.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => JoinLobby(lobby.Id));
        }
    }
    */

    public async void JoinLobby(string lobbyId)
    {
        //_connectedLobby = await JoinLobbyAsync(lobbyId);
    }
		
    public async Task<Lobby> JoinLobbyAsync(string lobbyId)
    {
        try {
            // вызываем метод из ILobbyService для присоединения к лобби
            var lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
        
            // If we found one, grab the relay allocation details
            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);
            
            // Set the details to the transform
            SetTransformAsClient(a);

            // Join the game room as a client
            NetworkManagerUI.Instance.StartClient();
            return lobby;
        }
        catch (Exception e) {
            Debug.Log($"Error JoinLobbyAsync" + e.Message);
            return null;
        }
    }

    public async Task<Lobby> QuickJoinLobby() {
        try {
            // Attempt to join a lobby in progress
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            // If we found one, grab the relay allocation details
            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);
            
            // Set the details to the transform
            SetTransformAsClient(a);

            // Join the game room as a client
            NetworkManagerUI.Instance.StartClient();
            return lobby;
        }
        catch (Exception e) {
            Debug.Log($"No lobbies available via quick join");
            return null;
        }
    }

    public async void CreateLobbyVoid()
    {
        //await CreateLobby();
    }

    public async Task<Lobby> CreateLobby() {
        
            const int maxPlayers = 2;

            // Create a relay allocation and generate a join code to share with the lobby
            var a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            // Create a lobby, adding the relay join code to the lobby data
            var options = new CreateLobbyOptions {
                Data = new Dictionary<string, DataObject> { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };
            lobby = await Lobbies.Instance.CreateLobbyAsync("Useless Lobby Name", maxPlayers, options);

            // Send a heartbeat every 15 seconds to keep the room alive
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            // Set the game room to use the relay allocation
            _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

            // Start the room. I'm doing this immediately, but maybe you want to wait for the lobby to fill up
            NetworkManagerUI.Instance.StartHost();
            return lobby;
        
    }

    public async void DestroyLobby()
    {
        if (lobby != null)
        {
            await Lobbies.Instance.DeleteLobbyAsync(lobby.Id);
            lobby = null;
        }
    }

    private void SetTransformAsClient(JoinAllocation a) {
        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }

    private static IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds) {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true) {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public void OnDestroy() {
        try {
            StopAllCoroutines();
            // todo: Add a check to see if you're host
            if (_connectedLobby != null) {
                if (_connectedLobby.HostId == _playerId) Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                else Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
            }
        }
        catch (Exception e) {
            Debug.Log($"Error shutting down lobby: {e}");
        }
    }
}