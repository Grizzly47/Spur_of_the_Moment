using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerStateMachine : MonoBehaviour
{
    [HideInInspector]public enum PlayerState { Idle = 0, Shooting = 1, Reloading = 2, Dodging = 3, Dead = 4 };

    private PlayerState currentState = PlayerState.Idle;
    private PlayerShoot playerShoot;
    private PlayerHealth playerHealth;
    private Animator playerAnimator;


    // Functions
    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimator = GetComponent<Animator>();
    }

    // Add logic to make it for most states, you must be idle
    public void ChangeState(PlayerState _newState)
    {
        if (currentState == _newState) return; // Prevent switching to the same state
        if (currentState == PlayerState.Idle)
        {
            currentState = _newState;
        }
        else if (!(_newState == PlayerState.Shooting || _newState == PlayerState.Reloading || _newState == PlayerState.Dodging))
        {
            currentState = _newState;
        }
        else
        {
            return;
        }
        Debug.Log($"Switched to {_newState} state");

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
                playerAnimator.Play("Reloading", 0, 0);
                    playerAnimator.SetTrigger("Reloading");
                StartCoroutine(HandleReloadState());
                break;
            case PlayerState.Dodging:
                    playerAnimator.Play("Dodging", 0, 0);
                playerAnimator.SetTrigger("Dodging");
                StartCoroutine(HandleDodgeState());
                break;
            case PlayerState.Dead:
                playerAnimator.Play("Player_Die", 0, 0);
                playerAnimator.SetTrigger("Dead");
                break;
            default:
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
}
