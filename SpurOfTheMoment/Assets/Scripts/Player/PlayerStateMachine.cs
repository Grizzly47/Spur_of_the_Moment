using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerStateMachine : MonoBehaviour
{
    [HideInInspector]public enum PlayerState { Idle, Shooting, Reloading, Dodging, Dead };

    private PlayerState currentState = PlayerState.Idle;
    private PlayerShoot playerShoot;


    // Functions
    private void Awake()
    {
        playerShoot = GetComponent<PlayerShoot>();
    }

    // Add logic to make it for most states, you must be idle
    public void ChangeState(PlayerState _newState)
    {
        if (currentState == _newState) return; // Prevent switching to the same state
        currentState = _newState;
        Debug.Log($"Switched to {_newState} state");

        UpdateState();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                
                break;
            case PlayerState.Shooting:
                StartCoroutine(HandleShootingState());
                break;
            case PlayerState.Reloading:
                StartCoroutine(HandleReloadState());
                break;
            case PlayerState.Dodging:
                
                break;
            case PlayerState.Dead:
                
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
}
