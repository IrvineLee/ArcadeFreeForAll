using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterSec : MonoBehaviour
{
    public float time;
    public bool isDestroy = false;

    float mTimer;

    void OnEnable()
    {
        mTimer = time;
    }

    void Update()
    {
        if (!GameManager.sSingleton.IsBattle()) return;

        if (mTimer > 0)
        {
            mTimer -= Time.deltaTime;
            if (mTimer <= 0)
            {
                mTimer = time;

                if (isDestroy) Destroy(gameObject);
                else gameObject.SetActive(false);
            }
        }
    }
}
