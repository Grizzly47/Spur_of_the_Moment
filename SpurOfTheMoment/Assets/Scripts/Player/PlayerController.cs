using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private PlayerInputActions playerControls;
    //[SerializeField] private Transform firePoint;

    private Transform playerTransform;
    private InputAction aim;
    private InputAction fire;
    private InputAction reload;
    private Vector2 aimDirection;
    private PlayerStateMachine playerStateMachine;
    private PlayerShoot playerShoot;

    private enum FacingDirection { Up, Left, Right }
    private FacingDirection facingDirection = FacingDirection.Up;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        playerControls.Enable();
        playerTransform = transform;
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerShoot = GetComponent<PlayerShoot>();
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
    }

    private void OnDisable()
    {
        aim.Disable();
        fire.Disable();
        reload.Disable();
    }

    private void Update()
    {
        aimDirection = aim.ReadValue<Vector2>();
        // Trashy
        if (aimDirection.y > 0f) // Aiming up
        {
            spriteRenderer.sprite = sprites[0];
            facingDirection = FacingDirection.Up;
        }
        else if (aimDirection.x > 0f) // Aiming right
        {
            spriteRenderer.sprite = sprites[1];
            playerTransform.localScale = new Vector3(-1, 1, 1);
            facingDirection = FacingDirection.Right;
        }
        else if (aimDirection.x < 0f) // Aiming left
        {
            spriteRenderer.sprite = sprites[1];
            playerTransform.localScale = new Vector3(1, 1, 1);
            facingDirection = FacingDirection.Left;
        }
        else
        {
            spriteRenderer.sprite = sprites[0];
            facingDirection = FacingDirection.Up;
        }

        switch (facingDirection) 
        {
            case FacingDirection.Up:
                playerShoot.firePoint.localPosition = new Vector3(0, 1, 0);
                playerShoot.firePoint.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case FacingDirection.Right:
                playerShoot.firePoint.localPosition = new Vector3(-0.707f, 0.707f, 0f); // Adjust relative to player
                playerShoot.firePoint.localRotation = Quaternion.Euler(0, 0, 45); // 45 degrees up from right
                break;
            case FacingDirection.Left:
                playerShoot.firePoint.localPosition = new Vector3(-0.707f, 0.707f, 0f); // Adjust relative to player
                playerShoot.firePoint.localRotation = Quaternion.Euler(0, 0, 135); // 45 degrees up from left
                break;
            default:
                break;
        }
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
}
