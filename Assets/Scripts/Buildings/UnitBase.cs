using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;

        ServerOnBaseDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    #endregion
}
