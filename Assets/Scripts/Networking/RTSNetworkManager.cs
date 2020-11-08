using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnerInstance = Instantiate( // spawn a unitSpawner on the server.
            unitSpawnerPrefab, 
            conn.identity.transform.position, 
            conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnerInstance, conn); // spawn a unitSpawnerInstance on all the clients, and then give ownership to the player who was added.
    }
}
