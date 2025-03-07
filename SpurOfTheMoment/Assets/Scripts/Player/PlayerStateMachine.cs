using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerStateMachine : MonoBehaviour
{
    [HideInInspector] public enum PlayerState { Idle = 0, Shooting = 1, Reloading = 2, Dodging = 3, Dead = 4, Stunned = 5};

    private PlayerState currentState = PlayerState.Idle;
    private PlayerShoot playerShoot;
    private PlayerHealth playerHealth;
    private Animator playerAnimator;
    private Coroutine activeCoroutine;

    private void Start()
    {
        playerShoot = GetComponent<PlayerShoot>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimator = GetComponent<Animator>();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(this);
        }
        else
        {
            Debug.LogError("GameManager Instance is NULL!");
        }

        // Ensure Idle animation plays on start
        ExecuteStateChange(PlayerState.Idle);
    }

    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState == PlayerState.Dead || GameManager.Instance.IsGameOver())
        {
            Debug.Log("Cannot change state, game is over!");
            return;
        }

        if (currentState == newState || (currentState != PlayerState.Idle && newState != PlayerState.Idle))
        {
            Debug.Log("Redundant state change or invalid transition");
            return; // Prevent redundant state change or invalid transitions
        }

        int playerIndex = GetComponent<PlayerInput>().playerIndex;
        Debug.Log("Changing " + playerIndex + " from " + currentState + " to " + newState);
        GameManager.Instance.QueuePlayerAction(playerIndex, newState);
    }

    public void ResetToIdle()
    {
        if (currentState == PlayerState.Dead) return;
        int playerIndex = GetComponent<PlayerInput>().playerIndex;
        ExecuteStateChange(PlayerState.Idle);
        GameManager.Instance.ClearQueuedAction(playerIndex);
    }

    public void ExecuteStateChange(PlayerState newState)
    {
        if (currentState == PlayerState.Dead) return;
        currentState = newState;
        Debug.Log($"Switched to {newState} state");

        UpdateState();
    }

    private void UpdateState()
    {
        playerAnimator.SetInteger("State", (int)currentState);

        switch (currentState)
        {
            case PlayerState.Idle:
                playerAnimator.Play("Idle", 0, 0);
                break;
            case PlayerState.Shooting:
                StartCoroutine(HandleShootingState());
                playerAnimator.Play("Shooting", 0, 0);
                break;
            case PlayerState.Reloading:
                StartCoroutine(HandleReloadState());
                playerAnimator.Play("Reloading", 0, 0);
                break;
            case PlayerState.Dodging:
                StartCoroutine(HandleDodgeState());
                playerAnimator.Play("Dodging", 0, 0);
                break;
            case PlayerState.Dead:
                playerAnimator.Play("Player_Die", 0, 0);
                StartCoroutine(HandleDeathState());
                break;
            case PlayerState.Stunned:
                StartCoroutine(HandleStunState());
                // Animator?
                break;
        }
    }

    public void SetAnimationPaused(bool pause)
    {
        if (playerAnimator != null)
        {
            playerAnimator.speed = pause ? 0 : 1; // 0 = pause, 1 = resume
        }
    }

    private IEnumerator HandleShootingState()
    {
        playerShoot.Shoot();

        float timer = 0f;
        while (timer < playerShoot.shootDuration)
        {
            if (!GameManager.Instance.IsBulletTimeActive()) // Only count time when bullet time is off
            {
                timer += Time.deltaTime;
            }
            yield return null; // Wait for next frame
        }

        ResetToIdle();
    }

    private IEnumerator HandleReloadState()
    {
        float timer = 0f;
        while (timer < playerShoot.reloadDuration)
        {
            if (!GameManager.Instance.IsBulletTimeActive())
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }

        playerShoot.Reload();
        ResetToIdle();
    }

    private IEnumerator HandleDodgeState()
    {
        playerHealth.Dodge(true);

        float timer = 0f;
        while (timer < playerHealth.dodgeDuration)
        {
            if (!GameManager.Instance.IsBulletTimeActive())
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }

        playerHealth.Dodge(false);
        ResetToIdle();
    }

    private IEnumerator HandleStunState()
    {
        float timer = 0f;
        while (timer < GameManager.Instance.stunTime)
        {
            if (!GameManager.Instance.IsBulletTimeActive())
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }

        if (currentState != PlayerState.Dead) ResetToIdle();
    }

    private IEnumerator HandleDeathState()
    {
        if (playerAnimator != null)
        {
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
        }

        GameManager.Instance.PlayerDied(this);
    }
}
