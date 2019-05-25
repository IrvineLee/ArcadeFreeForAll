using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterSec : MonoBehaviour
{
    [SerializeField] float m_Time;                      // The time before disabling.
    [SerializeField] bool m_IsDestroy = false;          // Will the gameobject gets destroyed after the timer ended.
    [SerializeField] bool m_IsThisForBattle = false;    // If this is false, the timer will continue to run when the game is not in battle mode.

    float mTimer;

    void OnEnable()
    {
        mTimer = m_Time;
    }

    void Update()
    {
        if (m_IsThisForBattle && !GameManager.sSingleton.IsBattle()) return;

        if (mTimer > 0)
        {
            mTimer -= Time.deltaTime;
            if (mTimer <= 0)
            {
                mTimer = m_Time;

                if (m_IsDestroy) Destroy(gameObject);
                else gameObject.SetActive(false);
            }
        }
    }
}
