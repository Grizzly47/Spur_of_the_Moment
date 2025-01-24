using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInputActions playerControls;

    private InputAction aim;
    private InputAction fire;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        Debug.Log("Enabling Player Controls");
        aim = playerControls.Player.Move;
        aim.Enable();

        fire = playerControls.Player.Attack;
        fire.Enable();
        fire.performed += Fire;
    }

    private void OnDisable()
    {
        aim.Disable();
        fire.Disable();
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("Pew");
    }
}
