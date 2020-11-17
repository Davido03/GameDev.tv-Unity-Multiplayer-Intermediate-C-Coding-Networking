using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7f;
    [SerializeField] private float unitSpawnDuration = 5f;

    [SyncVar(hook =nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits(); // try to produce units every frame
        }

        if (isClient)
        {
            UpdateTimerDisplay(); // try to update timer display every frame
        }
    }

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
    private void ProduceUnits()
    {
        if (queuedUnits == 0) { return; } // if no queued units, exit method

        unitTimer += Time.deltaTime; // add frame time onto unit timer

        if (unitTimer < unitSpawnDuration) { return; } // exit method until unitTimer equal to or greater than unitSpawnDuration

        GameObject unitInstance = Instantiate( // spawn prefab on the server
        unitPrefab.gameObject,
        unitSpawnPoint.position,
        unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance.gameObject, connectionToClient); // spawn unit on the network so clients receive, and make the client who owns the spawner the owner.

        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange; // set up random spawn offset so units don't stack onto each other
        spawnOffset.y = unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>(); 
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset); // move unit to the spawn point position + a random offset

        queuedUnits--; // unit produced so decrement queuedUnits
        unitTimer = 0f; // reset unit timer
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (queuedUnits == maxUnitQueue) { return; }

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.GetResources() < unitPrefab.GetResourceCost()) { return; }

        queuedUnits++;

        player.SetResources(player.GetResources() - unitPrefab.GetResourceCost());
    }

    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;

        if (newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp( // make sure progress go smoothly between values, to prevent jitteriness
                unitProgressImage.fillAmount,
                newProgress,
                ref progressImageVelocity,
                0.1f
            );
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; } // if mouse button not clicked, exit method

        if(!hasAuthority) { return; } // if client does not own spawner, exit method

        CmdSpawnUnit();
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }

    #endregion
}
