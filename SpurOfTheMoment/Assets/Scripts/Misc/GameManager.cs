using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isBulletTimeActive = false;
    private List<Bullet> activeBullets = new List<Bullet>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleBulletTime(bool isActive)
    {
        isBulletTimeActive = isActive;
        foreach (Bullet bullet in activeBullets)
        {
            if (bullet != null)
            {
                bullet.SetFrozen(isBulletTimeActive);
            }
        }
    }

    public void RegisterBullet(Bullet bullet)
    {
        activeBullets.Add(bullet);
    }

    public void UnregisterBullet(Bullet bullet)
    {
        activeBullets.Remove(bullet);
    }
}
