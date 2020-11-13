using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnerInstance = Instantiate( // spawn a unitSpawner on the server.
            unitSpawnerPrefab, 
            conn.identity.transform.position, 
            conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnerInstance, conn); // spawn a unitSpawnerInstance on all the clients, and then give ownership to the player who was added.
    }

    public override void OnServerSceneChanged(string sceneName) //whenever server changes the scene
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
