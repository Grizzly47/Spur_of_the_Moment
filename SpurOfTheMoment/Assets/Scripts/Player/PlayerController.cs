using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private PlayerInputActions playerControls;

    private Transform playerTransform;
    private InputAction aim;
    private InputAction fire;
    private InputAction reload;
    private InputAction dodge;
    private Vector2 aimDirection;
    private PlayerStateMachine playerStateMachine;
    private PlayerShoot playerShoot;
    private Animator playerAnimator;

    private enum FacingDirection { Up, Left, Right }
    private FacingDirection facingDirection = FacingDirection.Up;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        playerControls.Enable();
        playerTransform = transform;
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerShoot = GetComponent<PlayerShoot>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Debug.Log("Enabling Player Controls");
        aim = playerControls.Player.Aim;
        aim.Enable();

        fire = playerControls.Player.Attack;
        fire.Enable();
        fire.performed += Fire;

        reload = playerControls.Player.Reload;
        reload.Enable();
        reload.performed += Reload;

        dodge = playerControls.Player.Dodge;
        dodge.Enable();
        dodge.performed += Dodge;
    }

    private void OnDisable()
    {
        aim.Disable();
        fire.Disable();
        reload.Disable();
        dodge.Disable();
    }

    private void Update()
    {
        aimDirection = aim.ReadValue<Vector2>();
        // Trashy
        if (aimDirection.y > 0f) // Aiming up
        {
            playerAnimator.SetInteger("Direction", 0);
            playerTransform.localScale = new Vector3(1, 1, 1);
            facingDirection = FacingDirection.Up;
        }
        else if (aimDirection.x > 0f) // Aiming right
        {
            playerAnimator.SetInteger("Direction", 2);
            playerTransform.localScale = new Vector3(1, 1, 1);
            facingDirection = FacingDirection.Right;
        }
        else if (aimDirection.x < 0f) // Aiming left
        {
            playerAnimator.SetInteger("Direction", 1);
            playerTransform.localScale = new Vector3(-1, 1, 1);
            facingDirection = FacingDirection.Left;
        }
        else
        {
            playerAnimator.SetInteger("Direction", 0);
            facingDirection = FacingDirection.Up;
        }

        // Can be removed later
        switch (facingDirection) 
        {
            case FacingDirection.Up:
                playerShoot.firePoint.localPosition = new Vector3(0, 1, 0);
                playerShoot.firePoint.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case FacingDirection.Right:
                playerShoot.firePoint.localPosition = new Vector3(0.86f, 0.13f, 0f); // Adjust relative to player
                playerShoot.firePoint.localRotation = Quaternion.Euler(0, 0, -75); // 45 degrees up from right
                break;
            case FacingDirection.Left:
                playerShoot.firePoint.localPosition = new Vector3(0.86f, 0.13f, 0f); // Adjust relative to player
                playerShoot.firePoint.localRotation = Quaternion.Euler(0, 0, -75); // 45 degrees up from left
                break;
            default:
                break;
        }
        // End of remove
    }

    private void Fire(InputAction.CallbackContext context)
    {
        playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Shooting);
    }

    private void Reload(InputAction.CallbackContext context)
    {
        Debug.Log("Reload Input Received");
        playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Reloading);
    }

    private void Dodge(InputAction.CallbackContext context)
    {
        Debug.Log("Dodging");
        playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Dodging);
    }
}
