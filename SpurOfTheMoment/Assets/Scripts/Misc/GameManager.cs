using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //TEMP
    public int stunTime = 2;

    [SerializeField] private float maxHeatTime = 5f;
    [SerializeField] private float minHeatTime = 2f;
    [SerializeField] private float heatReductionPerTurn = 0.5f;
    private float currentHeatTime;
    private float heatTimer;

    private bool isBulletTimeActive = false;
    private List<Bullet> activeBullets = new();
    private List<PlayerStateMachine> players = new();

    [SerializeField] private GameObject timePauseOverlay;
    [SerializeField] private Slider heatBar;
    [SerializeField] private Transform player1Spawn;
    [SerializeField] private Transform player2Spawn;

    [SerializeField] private GameObject playerPrefab; // Player prefab to spawn both players

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentHeatTime = maxHeatTime;
        ToggleBulletTime(true);

        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        if (players.Count >= 2) return; // Prevent duplicate spawns

        // Spawn Player 1
        GameObject player1 = Instantiate(playerPrefab, player1Spawn.position, Quaternion.identity);
        RegisterPlayer(player1.GetComponent<PlayerStateMachine>());

        // Spawn Player 2
        GameObject player2 = Instantiate(playerPrefab, player2Spawn.position, Quaternion.identity);
        RegisterPlayer(player2.GetComponent<PlayerStateMachine>());

        Debug.Log("Both players spawned successfully!");
    }

    public void RegisterPlayer(PlayerStateMachine player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            Debug.Log($"Player {players.Count} registered.");
        }
    }

    private void Update()
    {
        if (!isBulletTimeActive) return;

        heatTimer -= Time.deltaTime;
        UpdateHeatBar();

        if (heatTimer <= 0)
        {
            ForcePlayersStun();
        }
    }

    public void ToggleBulletTime(bool isActive)
    {
        isBulletTimeActive = isActive;
        foreach (var bullet in activeBullets)
        {
            bullet?.SetFrozen(isBulletTimeActive);
        }

        if (isBulletTimeActive)
        {
            heatTimer = currentHeatTime;
            currentHeatTime = Mathf.Max(minHeatTime, currentHeatTime - heatReductionPerTurn);
        }

        ShowHeatBar(isBulletTimeActive);
        timePauseOverlay.SetActive(isBulletTimeActive);
    }

    public void RegisterBullet(Bullet bullet) => activeBullets.Add(bullet);
    public void UnregisterBullet(Bullet bullet) => activeBullets.Remove(bullet);

    private void ForcePlayersStun()
    {
        players.ForEach(player => player.ChangeState(PlayerStateMachine.PlayerState.Stunned));
        ToggleBulletTime(false);
    }

    private void ShowHeatBar(bool isVisible)
    {
        if (heatBar == null) return;
        heatBar.gameObject.SetActive(isVisible);
        heatBar.maxValue = currentHeatTime;
        heatBar.value = currentHeatTime;
    }

    private void UpdateHeatBar()
    {
        if (heatBar != null) heatBar.value = heatTimer;
    }
}