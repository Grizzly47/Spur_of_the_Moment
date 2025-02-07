using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float maxHeatTime = 5f; // Starting heat time
    [SerializeField] private float minHeatTime = 2f; // Minimum allowed heat time
    [SerializeField] private float heatReductionPerTurn = 0.5f; // How much heat shrinks per bullet time
    private float currentHeatTime;
    private float heatTimer;

    private bool isBulletTimeActive = false;
    private List<Bullet> activeBullets = new List<Bullet>();
    private List<PlayerStateMachine> players = new List<PlayerStateMachine>();

    [SerializeField] private GameObject timePauseOverlay;
    [SerializeField] private Slider heatBar;

    public int stunTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentHeatTime = maxHeatTime;
        ToggleBulletTime(true);
    }

    private void Update()
    {
        if (isBulletTimeActive)
        {
            heatTimer -= Time.deltaTime;
            UpdateHeatBar();
            if (heatTimer <= 0)
            {
                ForcePlayersStun();
            }
        }
    }

    public void RegisterPlayer(PlayerStateMachine player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }


    public void ToggleBulletTime(bool isActive)
    {
        isBulletTimeActive = isActive;
        foreach (Bullet bullet in activeBullets)
        {
            if (bullet != null)
            {
                bullet.SetFrozen(isBulletTimeActive);
            }
        }
        if (isBulletTimeActive)
        {
            heatTimer = currentHeatTime;
            currentHeatTime = Mathf.Max(minHeatTime, currentHeatTime - heatReductionPerTurn);
            ShowHeatBar(true);
        }
        else
        {
            ShowHeatBar(false);
        }

        timePauseOverlay.SetActive(isBulletTimeActive);
    }

    public void RegisterBullet(Bullet bullet)
    {
        activeBullets.Add(bullet);
    }

    public void UnregisterBullet(Bullet bullet)
    {
        activeBullets.Remove(bullet);
    }

    private void ForcePlayersStun()
    {
        // Apply stun to ALL players
        foreach (var player in players)
        {
            player.ChangeState(PlayerStateMachine.PlayerState.Stunned);
        }

        ToggleBulletTime(false); // Exit bullet time
    }

    private void ShowHeatBar(bool isVisible)
    {
        if (heatBar != null)
        {
            heatBar.gameObject.SetActive(isVisible);
            heatBar.maxValue = currentHeatTime; // Ensure correct max value
            heatBar.value = currentHeatTime; // Start full
        }
    }

    private void UpdateHeatBar()
    {
        if (heatBar != null)
        {
            heatBar.value = heatTimer;
        }
    }
}
