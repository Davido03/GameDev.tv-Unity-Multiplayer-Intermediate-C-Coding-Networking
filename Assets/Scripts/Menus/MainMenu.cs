using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;

    [SerializeField] private bool useSteam = false;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        if (!useSteam) { return; }

        // link steam's callbacks to our methods
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4); // friends only lobby, with max of 4 players
            return;
        }

        NetworkManager.singleton.StartHost();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) // if lobby not created successfully
        {
            landingPagePanel.SetActive(true);
            return;
        }

        // if lobby created successfully

        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(
        new CSteamID(callback.m_ulSteamIDLobby),
        "HostAddress",
        SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby); // let player join lobby
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) { return; } // if the host exit the method

        // if not the host

        string hostAddress = SteamMatchmaking.GetLobbyData( //get the address of host
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");

        NetworkManager.singleton.networkAddress = hostAddress; // set network address to that host address
        NetworkManager.singleton.StartClient(); // connect to that host address

        landingPagePanel.SetActive(false);
    }
}
