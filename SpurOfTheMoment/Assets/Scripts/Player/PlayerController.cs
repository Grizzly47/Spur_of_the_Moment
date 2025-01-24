using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private PlayerInputActions playerControls;
    [SerializeField] private Transform firePoint;

    private Transform playerTransform;
    private InputAction aim;
    private InputAction fire;
    private Vector2 aimDirection;

    private enum FacingDirection { Up, Left, Right }
    private FacingDirection facingDirection = FacingDirection.Up;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        playerTransform = transform;
    }

    private void OnEnable()
    {
        Debug.Log("Enabling Player Controls");
        aim = playerControls.Player.Aim;
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
                firePoint.localPosition = new Vector3(0, 1, 0);
                firePoint.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case FacingDirection.Right:
                firePoint.localPosition = new Vector3(-0.866f, 0.5f, 0); // Adjust relative to player
                firePoint.localRotation = Quaternion.Euler(0, 0, 30); // 30 degrees up from right
                break;
            case FacingDirection.Left:
                firePoint.localPosition = new Vector3(-0.866f, 0.5f, 0); // Adjust relative to player
                firePoint.localRotation = Quaternion.Euler(0, 0, 150); // 30 degrees up from left
                break;
            default:
                break;
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("Pew");
    }
}
