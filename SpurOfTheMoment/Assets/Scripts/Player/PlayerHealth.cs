using UnityEngine;

public class PlayerHealth : Health
{
    private bool isDodging = false;
    public float dodgeDuration = 1;

    protected override void Awake()
    {
        base.Awake();
        // You can extend functionality here if needed
    }

    public void Dodge(bool currentlyDodging)
    {
        Debug.Log($"Dodging: {(isDodging ? "yes" : "no")}");
        isDodging = currentlyDodging;
    }

    public override void TakeDamage(int damage)
    {
        if (isDodging) return;
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        Debug.Log($"{gameObject.name} died!");

        // Change player state to Dead
        GetComponent<PlayerStateMachine>().ExecuteStateChange(PlayerStateMachine.PlayerState.Dead);
    }
}
