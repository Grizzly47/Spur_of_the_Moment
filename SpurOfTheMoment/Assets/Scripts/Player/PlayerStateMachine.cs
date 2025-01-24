using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerStateMachine : MonoBehaviour
{
    [HideInInspector]public enum PlayerState { Idle, Shooting, Reloading, Dodging, Dead };

    private PlayerState currentState = PlayerState.Idle;

    private void Update()
    {
        UpdateState();
    }

    public void ChangeState(PlayerState _newState)
    {
        if (currentState == _newState) return; // Prevent switching to the same state
        currentState = _newState;
        Debug.Log($"Switched to {_newState} state");
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                
                break;
            case PlayerState.Shooting:
                
                break;
            case PlayerState.Reloading:
                
                break;
            case PlayerState.Dodging:
                
                break;
            case PlayerState.Dead:
                
                break;
            default:
                break;
        }
    }
}
