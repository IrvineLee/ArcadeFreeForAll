using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float primaryShootDelay = 0.25f;
    public float rocketyShootDelay = 0.25f;

    [SerializeField] Transform m_ExplodeTrans;
    [ReadOnly] [SerializeField] int m_CurrLife, m_CurrHealth, m_CurrShield, m_MaxShield, m_CurrRockets, m_MaxRockets, m_CurrScore, m_CurrCoins, m_CurrKills;
    [ReadOnly] [SerializeField] float m_ShieldRechargeTime;

    public class Border
    {
        public float top, bottom, left, right;

        public Border()
        {
            this.top = 0;
            this.bottom = 0;
            this.left = 0;
            this.right = 0;
        }

        public bool IsZero()
        {
            if (top == 0 && bottom == 0 && left == 0 && right == 0) return true;
            return false;
        }
    }
    Border border = new Border();

    enum BulletType
    {
        PRIMARY = 0,
        ROCKET
    }

    Vector3 mPlayerSize;
    bool mIsPlayerPrimary = false, mIsPlayerRocket = false, mIsRecharge = false;
    float mShieldRechargeTimer;

    void Start()
    {
        m_CurrLife = GameManager.sSingleton.startLifes;
        m_CurrHealth = GameManager.sSingleton.startHealth;
        m_CurrShield = GameManager.sSingleton.startShield;
        m_MaxShield = m_CurrShield;
        m_CurrRockets = GameManager.sSingleton.startRockets;
        m_MaxRockets = m_CurrRockets;
        m_ShieldRechargeTime = GameManager.sSingleton.startShieldRechargeTime;

        ResetPosition();

        float distance = (transform.position - Camera.main.transform.position).z;
        mPlayerSize = GetComponent<Renderer>().bounds.size;

        // Set the playable boudaries.
        border.left = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0, distance)).x + (mPlayerSize.x / 2);
        border.right = Camera.main.ViewportToWorldPoint(new Vector3(0.75f, 0, distance)).x - (mPlayerSize.x / 2);
        border.top = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, distance)).y - (mPlayerSize.y / 2);
        border.bottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).y + (mPlayerSize.y / 2);
    }

    void Update()
    {
        if (!GameManager.sSingleton.IsBattle()) return;

        // Movement.
        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);

        // Prevent player from moving out of screen.
        transform.position = (new Vector3(
            Mathf.Clamp(transform.position.x, border.left, border.right),
            Mathf.Clamp(transform.position.y, border.bottom, border.top),
            transform.position.z)
        );

        // Rotate the player based on mouse position.
        Vector2 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, transform.position.z - Camera.main.transform.position.z));

        Quaternion rot = transform.rotation;
        float z = Mathf.Atan2((worldPos.y - transform.position.y), (worldPos.x - transform.position.x)) * Mathf.Rad2Deg - 90;
        rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, z);
        transform.rotation = rot;

        // Player primary fire and rocket.
        if (Input.GetMouseButton(0) && !mIsPlayerPrimary) StartCoroutine(ShootPrimary(primaryShootDelay));
        if (m_CurrRockets > 0 && Input.GetMouseButton(1) && !mIsPlayerRocket) StartCoroutine(ShootRocket(rocketyShootDelay));

        // Shield recharge.
        if (mIsRecharge && mShieldRechargeTimer > 0)
        {
            mShieldRechargeTimer -= Time.deltaTime;
            if (mShieldRechargeTimer <= 0) StartCoroutine(FillUpShield());
        }
    }

    void LateUpdate()
    {
        // Re-enable recharge of shield.
        if (!mIsRecharge) mIsRecharge = true;
    }

    /// ---------------------------------------------------------------------------------------------
    /// --------------------------------- PUBLIC FUNCTIONS ------------------------------------------
    /// ---------------------------------------------------------------------------------------------

    public void AddLife()
    {
        if (m_CurrLife < GameManager.sSingleton.maxLifes)
        {
            m_CurrLife++;
            UIManager.sSingleton.UpdateLife(m_CurrLife);
        }
    }

    public void RechargeRockets()
    {
        if (m_CurrRockets < m_MaxRockets)
        {
            m_CurrRockets = m_MaxRockets;
            UIManager.sSingleton.UpdateRockets(m_CurrRockets);
        }
    }

    public void AddMaxRockets()
    {
        if (m_MaxRockets < GameManager.sSingleton.maxRockets)
        {
            m_MaxRockets++;
            m_CurrRockets++;
            UIManager.sSingleton.UpdateRockets(m_CurrRockets);
        }
    }

    public void UpgradeShield()
    {
        if (m_MaxShield < GameManager.sSingleton.maxShield)
        {
            m_MaxShield += 10;
            m_CurrShield = m_MaxShield;
            UIManager.sSingleton.UpdateShield(m_MaxShield);
        }
    }

    public void UpgradeShieldRechargeRate()
    {
        if (m_ShieldRechargeTime > GameManager.sSingleton.minShieldRechargeTime)
        {
            m_ShieldRechargeTime -= 0.5f;
        }
    }

    public void RestoreHealthAndShield()
    {
        m_CurrHealth = GameManager.sSingleton.startHealth;
        m_CurrShield = m_MaxShield;
    }

    public void AddScore(int val)
    {
        m_CurrScore += val;
        UIManager.sSingleton.UpdateScore(m_CurrScore);
    }

    public void AddKill()
    {
        m_CurrKills++;
        UIManager.sSingleton.UpdateKills(m_CurrKills);
    }

    public void AddCoin()
    {
        m_CurrCoins++;
        UIManager.sSingleton.UpdateCoins(m_CurrKills);
    }

    public void RemoveCoin(int amount)
    {
        m_CurrCoins -= amount;
        UIManager.sSingleton.UpdateCoins(m_CurrCoins);
    }

    public void ResetPosition()
    {
        transform.position = GameManager.sSingleton.startPosTrans.position;
        transform.rotation = Quaternion.identity;
    }

    // ----------------------------------------------------------------------------------------------
    // ------------------------------------------- GETTER -------------------------------------------
    // ----------------------------------------------------------------------------------------------

    public int GetLife { get { return m_CurrLife; } }
    public int GetHealth { get { return m_CurrHealth; } }
    public bool IsRocketsFull { get { return (m_CurrRockets == m_MaxRockets) ? true : false;  } }
    public int GetMaxRockets { get { return m_MaxRockets; } }
    public int GetMaxShield { get { return m_MaxShield; } }
    public float GetShieldRechargeTime { get { return m_ShieldRechargeTime; } }
    public int GetCoin { get { return m_CurrCoins; } }
    public int GetScore { get { return m_CurrScore; } }

    // ----------------------------------------------------------------------------------------------

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            // Handle damage.
            int damage = collision.GetComponent<BulletMove>().damage;
            mIsRecharge = false;

            if (m_CurrShield != 0)
            {
                m_CurrShield -= damage;
                if (m_CurrShield < 0)
                {
                    damage = Mathf.Abs(m_CurrShield);
                    m_CurrHealth -= damage;
                    m_CurrShield = 0;
                }
                mShieldRechargeTimer = m_ShieldRechargeTime;
            }
            else m_CurrHealth -= damage;

            // Update the health and shield UI.
            UIManager.sSingleton.UpdateHealth(m_CurrHealth);
            UIManager.sSingleton.UpdateShield(m_CurrShield);

            if (m_CurrHealth <= 0)
            {
                // Create an explosion where the player died.
                Instantiate(m_ExplodeTrans, transform.position, Quaternion.identity);

                if (m_CurrLife > 0)
                {
                    // Respawn the player back at the start position.
                    transform.position = GameManager.sSingleton.startPosTrans.position;

                    // Minus life and fully restore the health and shield.
                    m_CurrLife--;
                    m_CurrHealth = GameManager.sSingleton.startHealth;
                    m_CurrShield = m_MaxShield;

                    // Update the UI.
                    UIManager.sSingleton.UpdateHealth(m_CurrHealth);
                    UIManager.sSingleton.UpdateShield(m_CurrShield);
                    UIManager.sSingleton.UpdateLife(m_CurrLife);
                }
                else
                {
                    // Game over.
                    GameManager.sSingleton.currState = GameManager.State.GAME_OVER;
                    UIManager.sSingleton.ShowGameOver();
                }
            }

            collision.gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Coin"))
        {
            m_CurrCoins++;
            UIManager.sSingleton.UpdateCoins(m_CurrCoins);

            collision.gameObject.SetActive(false);
        }
    }

    IEnumerator ShootPrimary(float delay)
    {
        mIsPlayerPrimary = true;

        ActivateBullet(BulletType.PRIMARY);
        yield return new WaitForSeconds(delay);

        mIsPlayerPrimary = false;
    }

    IEnumerator ShootRocket(float delay)
    {
        mIsPlayerRocket = true;

        ActivateBullet(BulletType.ROCKET);
        m_CurrRockets--;
        UIManager.sSingleton.UpdateRockets(m_CurrRockets);
        yield return new WaitForSeconds(delay);

        mIsPlayerRocket = false;
    }

    IEnumerator FillUpShield()
    {
        float timer = 0, duration = GameManager.sSingleton.shieldRechargeDuration;
        float startShield = m_CurrShield;

        // If gotten damaged, mIsRecharge will be false, which will then exit from the loop.
        while (timer < duration && mIsRecharge)
        {
            timer += Time.deltaTime;
            if (timer > duration) timer = duration;

            float diff = (float)(m_MaxShield - startShield);
            float currValForCurrTime = (timer / duration) * diff;
            int shield = (int)Mathf.Ceil((float)startShield + currValForCurrTime);

            if (m_CurrShield < shield)
            {
                m_CurrShield = shield;
                UIManager.sSingleton.UpdateShield(m_CurrShield);
            }
            yield return null;
        }
    }

    void ActivateBullet(BulletType bulletType)
    {
        Transform trans = null;
        if (bulletType == BulletType.PRIMARY) trans = BulletManager.sSingleton.GetPlayerBullet();
        else trans = BulletManager.sSingleton.GetPlayerRocket();

        trans.position = transform.position + transform.up * (mPlayerSize.y / 2);
        trans.rotation = transform.rotation;
        trans.gameObject.SetActive(true);
    }
}
