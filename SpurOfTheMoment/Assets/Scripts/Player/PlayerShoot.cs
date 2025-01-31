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
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
            bulletRB.AddForce(firePoint.up * shootSpeed, ForceMode2D.Impulse);
        }
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Reloaded");
    }
}
