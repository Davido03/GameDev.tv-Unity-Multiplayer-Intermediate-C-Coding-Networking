using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate( // spawn prefab on the server
            unitPrefab, 
            unitSpawnPoint.position, 
            unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient); // spawn unit on the network so clients receive, and make the client who owns the spawner the owner.
    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; } // if mouse button not clicked, exit method

        if(!hasAuthority) { return; } // if client does not own spawner, exit method

        CmdSpawnUnit();
    }

    #endregion
}
