using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private int maxAmmo = 6;
    [SerializeField] private int currentAmmo;

    public Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    public float shootDuration;
    public float reloadDuration;
    [SerializeField] private float shootSpeed;

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    public void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("No Ammo. Reload");
        }
        else
        {
            Debug.Log("Projectile Fired!");
            currentAmmo--;
            // Ugh
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(firePoint.rotation.eulerAngles.x, firePoint.rotation.eulerAngles.y, firePoint.rotation.eulerAngles.z));
            Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
            bulletRB.linearVelocity = firePoint.right * shootSpeed;
            Bullet bulletScript = bulletRB.GetComponent<Bullet>();
            bulletScript.SetInitialSpeed(shootSpeed);
        }
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Reloaded");
    }
}
