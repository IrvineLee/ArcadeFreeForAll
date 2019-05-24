using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public int damage = 1;
    public float speed = 1f;

    void Update()
    {
        if (!GameManager.sSingleton.IsBattle()) return;

        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
    }
}
