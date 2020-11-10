using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> myUnits = new List<Unit>();

    #region Server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned; // subscribe unit's ServerOnUnitSpawned event to ServerHandleUnitSpawned method.
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned; // subscribe unit's ServerOnUnitDespawned event to ServerHandleUnitDespawned method.
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned; // unsubscribe unit's ServerOnUnitSpawned event from ServerHandleUnitSpawned method.
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned; // unsubscribe unit's ServerOnUnitDespawned event from ServerHandleUnitDespawned method.
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; } // if unit's owner is not same as the client, exit method.

        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Remove(unit);
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        if (!isClientOnly) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned; 
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned; 
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned; 
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        if (!hasAuthority) { return; } // if client does not own unit, exit method.

        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (!hasAuthority) { return; }

        myUnits.Remove(unit);
    }

    #endregion


}
