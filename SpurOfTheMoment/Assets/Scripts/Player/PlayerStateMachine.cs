using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerStateMachine : MonoBehaviour
{
    [HideInInspector] public enum PlayerState { Idle = 0, Shooting = 1, Reloading = 2, Dodging = 3, Dead = 4, Stunned = 5 };

    private PlayerState currentState = PlayerState.Idle;
    private PlayerShoot playerShoot;
    private PlayerHealth playerHealth;
    private Animator playerAnimator;

    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimator = GetComponent<Animator>();

        GameManager.Instance.RegisterPlayer(this);
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return; // Prevent redundant state change

        // Allow changing to any state from Idle, but prevent changing between action states
        if (currentState != PlayerState.Idle && newState != PlayerState.Idle)
        {
            return;
        }

        currentState = newState;
        Debug.Log($"Switched to {newState} state");

        // Toggle bullet time based on Idle state
        GameManager.Instance.ToggleBulletTime(currentState == PlayerState.Idle);

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
                break;
            case PlayerState.Stunned:
                StartCoroutine(HandleStunState());
                // Animator?
                break;
        }
    }

    private IEnumerator HandleShootingState()
    {
        playerShoot.Shoot();
        yield return new WaitForSeconds(playerShoot.shootDuration);
        ChangeState(PlayerState.Idle);
    }

    private IEnumerator HandleReloadState()
    {
        yield return new WaitForSeconds(playerShoot.reloadDuration);
        playerShoot.Reload();
        ChangeState(PlayerState.Idle);
    }

    private IEnumerator HandleDodgeState()
    {
        playerHealth.Dodge(true);
        yield return new WaitForSeconds(playerHealth.dodgeDuration);
        playerHealth.Dodge(false);
        ChangeState(PlayerState.Idle);
    }

    private IEnumerator HandleStunState()
    {
        yield return new WaitForSeconds(GameManager.Instance.stunTime); // Adjust stun duration as needed
        ChangeState(PlayerState.Idle);
    }
}
