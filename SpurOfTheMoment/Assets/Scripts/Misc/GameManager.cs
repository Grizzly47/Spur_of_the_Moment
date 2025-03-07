using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static PlayerStateMachine;

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

    private PlayerState queuedActionP1 = PlayerState.Idle;
    private PlayerState queuedActionP2 = PlayerState.Idle;
    private bool isPlayer1Idle = true;
    private bool isPlayer2Idle = true;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1;
        currentHeatTime = maxHeatTime;
        SpawnPlayers();

        ToggleBulletTime(true);
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

    public void QueuePlayerAction(int playerIndex, PlayerState action)
    {
        // Get opponent index
        int opponentIndex = (playerIndex == 0) ? 1 : 0;

        // Get player state
        PlayerStateMachine player = players[playerIndex];
        PlayerStateMachine opponent = players[opponentIndex];

        // If player is not in Idle, they CANNOT act, so ignore the input
        if (player.GetCurrentState() != PlayerState.Idle)
        {
            return;
        }

        // Queue the action
        if (playerIndex == 0) queuedActionP1 = action;
        else queuedActionP2 = action;

        Debug.Log($"Player {playerIndex} queued action: {action}");

        // Check if the opponent is Idle
        if (opponent.GetCurrentState() == PlayerState.Idle)
        {
            Debug.Log($"Player {playerIndex} is waiting for Player {opponentIndex} to act...");
            return; // Wait until opponent acts
        }

        // If opponent is NOT Idle, execute the state change immediately
        ExecuteQueuedActions();
    }

    private void ExecuteQueuedActions()
    {
        Debug.Log("Executing queued actions...");

        // Execute for Player 1 if their state is changing
        if (players[0].GetCurrentState() != queuedActionP1)
        {
            players[0].ExecuteStateChange(queuedActionP1);
            Debug.Log($"Player 1 changed state to {queuedActionP1}");
        }

        // Execute for Player 2 if their state is changing
        if (players[1].GetCurrentState() != queuedActionP2)
        {
            players[1].ExecuteStateChange(queuedActionP2);
            Debug.Log($"Player 2 changed state to {queuedActionP2}");
        }

        ToggleBulletTime(false);
    }

    public void ClearQueuedAction(int playerIndex)
    {
        if (playerIndex == 0) queuedActionP1 = PlayerState.Idle;
        else queuedActionP2 = PlayerState.Idle;

        Debug.Log($"Player {playerIndex} action cleared, now Idle.");

        ToggleBulletTime(true);
    }

    public void ToggleBulletTime(bool isActive)
    {
        if (!isGameOver)
        {
            isBulletTimeActive = isActive;

            // Freeze/unfreeze bullets
            foreach (var bullet in activeBullets)
            {
                bullet?.SetFrozen(isBulletTimeActive);
            }

            // Pause the animation for the player who is NOT Idle
            if (players[0].GetCurrentState() != PlayerState.Idle)
            {
                players[0].SetAnimationPaused(isActive);
            }
            if (players[1].GetCurrentState() != PlayerState.Idle)
            {
                players[1].SetAnimationPaused(isActive);
            }

            // Reset heat timer and reduce max heat duration over time
            if (isBulletTimeActive)
            {
                heatTimer = currentHeatTime;
                currentHeatTime = Mathf.Max(minHeatTime, currentHeatTime - heatReductionPerTurn);
            }

            ShowHeatBar(isBulletTimeActive);
            timePauseOverlay.SetActive(isBulletTimeActive);
        }
    }

    public void RegisterBullet(Bullet bullet) => activeBullets.Add(bullet);
    public void UnregisterBullet(Bullet bullet) => activeBullets.Remove(bullet);

    private void ForcePlayersStun()
    {
        if (queuedActionP1 == PlayerState.Idle) queuedActionP1 = PlayerState.Stunned;
        if (queuedActionP2 == PlayerState.Idle) queuedActionP2 = PlayerState.Stunned;

        // cut
        isPlayer1Idle = false;
        isPlayer2Idle = false;

        ExecuteQueuedActions();
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

    public bool IsBulletTimeActive()
    {
        return isBulletTimeActive;
    }

    public void PlayerDied(PlayerStateMachine deadPlayer)
    {
        if (isGameOver) return; // Prevent multiple calls

        Debug.Log($"Player {players.IndexOf(deadPlayer)} died!");
        isGameOver = true; // Mark game as over

        // Determine the winner
        PlayerStateMachine winner = players.Find(p => p != deadPlayer);
        string winnerMessage = winner != null ? $"Player {players.IndexOf(winner) + 1} Wins!" : "Draw!";

        // Stop all player actions
        foreach (var player in players)
        {
            player.ExecuteStateChange(PlayerStateMachine.PlayerState.Stunned); // Or create a "GameOver" state
        }

        // Show Game Over UI
        ShowGameOverScreen(winnerMessage);
    }

    private void ShowGameOverScreen(string message)
    {
        Time.timeScale = 0;
        GameOverUI.Instance.ShowGameOver(message);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
}