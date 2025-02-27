using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerInputActions playerControls;

    private PlayerInput playerInput;
    private int playerIndex;
    private Transform playerTransform;
    private InputAction aim, fire, reload, dodge;
    private Vector2 aimDirection;
    private PlayerStateMachine playerStateMachine;
    private PlayerShoot playerShoot;
    private Animator playerAnimator;

    private enum FacingDirection { Up, Left, Right, Down }
    private FacingDirection facingDirection = FacingDirection.Up;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        playerTransform = transform;
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerShoot = GetComponent<PlayerShoot>();
        playerAnimator = GetComponent<Animator>();

        playerInput = GetComponent<PlayerInput>();
        playerIndex = playerInput.playerIndex;

        playerAnimator.SetInteger("PlayerIndex", (int)playerIndex);

        Debug.Log($"{gameObject.name} assigned Player Index {playerIndex}");

        AssignControls();
    }

    private void AssignControls()
    {
        var playerMap = playerControls.Player;
        aim = playerIndex == 0 ? playerMap.Aim_P1 : playerMap.Aim_P2;
        fire = playerIndex == 0 ? playerMap.Attack_P1 : playerMap.Attack_P2;
        reload = playerIndex == 0 ? playerMap.Reload_P1 : playerMap.Reload_P2;
        dodge = playerIndex == 0 ? playerMap.Dodge_P1 : playerMap.Dodge_P2;
        facingDirection = playerIndex == 0 ? FacingDirection.Up : FacingDirection.Down;

        aim.Enable();
        fire.Enable();
        reload.Enable();
        dodge.Enable();

        fire.performed += Fire;
        reload.performed += Reload;
        dodge.performed += Dodge;
    }

    private void OnDisable()
    {
        aim.Disable();
        fire.Disable();
        reload.Disable();
        dodge.Disable();

        fire.performed -= Fire;
        reload.performed -= Reload;
        dodge.performed -= Dodge;
    }

    private void Update()
    {
        aimDirection = aim.ReadValue<Vector2>();

        playerAnimator.SetInteger("Direction", (int)facingDirection);
        playerTransform.localScale = new Vector3(facingDirection == FacingDirection.Left ? -1 : 1, 1, 1);

        // Optimized direction setting
        facingDirection = aimDirection switch
        {
            { y: > 0f } when playerIndex == 0 => FacingDirection.Up,
            { y: < 0f } when playerIndex == 1 => FacingDirection.Down,
            { x: > 0f } => FacingDirection.Right,
            { x: < 0f } => FacingDirection.Left,
            _ => facingDirection
        };

        // Optimized fire point positioning
        Vector3 firePointPos = facingDirection switch
        {
            FacingDirection.Up => new Vector3(0, 1, 0),
            FacingDirection.Down => new Vector3(0, -1, 0),
            FacingDirection.Right => new Vector3(0.86f, 0.13f, 0f),
            FacingDirection.Left => new Vector3(0.86f, 0.13f, 0f),
            _ => playerShoot.firePoint.localPosition
        };

        Quaternion firePointRot = facingDirection switch
        {
            FacingDirection.Up => Quaternion.identity,
            FacingDirection.Down => Quaternion.Euler(0, 0, 180),
            FacingDirection.Left => playerIndex == 0 ? Quaternion.Euler(0, 0, -75) : Quaternion.Euler(0, 0, 255),
            FacingDirection.Right => playerIndex == 0 ? Quaternion.Euler(0, 0, -75) : Quaternion.Euler(0, 0, 255),
            _ => Quaternion.identity
        };

        playerShoot.firePoint.localPosition = firePointPos;
        playerShoot.firePoint.localRotation = firePointRot;
    }

    private void Fire(InputAction.CallbackContext context) =>
        playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Shooting);

    private void Reload(InputAction.CallbackContext context) =>
        playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Reloading);

    private void Dodge(InputAction.CallbackContext context) =>
        playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Dodging);
}
