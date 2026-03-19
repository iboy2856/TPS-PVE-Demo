using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("预制体")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;

    [Header("池大小")]
    public int poolSize = 30;

    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    private Queue<GameObject> casingPool = new Queue<GameObject>();

    void Start()
    {
        // 预先创建子弹
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }

        // 预先创建弹壳
        for (int i = 0; i < poolSize; i++)
        {
            GameObject casing = Instantiate(casingPrefab);
            casing.SetActive(false);
            casingPool.Enqueue(casing);
        }
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count == 0)
        {
            // 动态扩容
            GameObject newBullet = Instantiate(bulletPrefab);
            bulletPool.Enqueue(newBullet);
        }
        GameObject bullet = bulletPool.Dequeue();
        bullet.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    public GameObject GetCasing()
    {
        if (casingPool.Count == 0)
        {
            GameObject newCasing = Instantiate(casingPrefab);
            casingPool.Enqueue(newCasing);
        }
        GameObject casing = casingPool.Dequeue();
        casing.SetActive(true);
        return casing;
    }

    public void ReturnCasing(GameObject casing)
    {
        casing.SetActive(false);
        casingPool.Enqueue(casing);
    }
}