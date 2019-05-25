using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [ReadOnly] public int enemyID;
    [SerializeField] int m_DamageScore = 100;
    [SerializeField] int m_KillScore = 500;

    [SerializeField] float m_MoveSpeed = 3;
    [SerializeField] float m_ShootTime = 1;
    [SerializeField] bool m_IsShoot = true;

    [ReadOnly][SerializeField]float m_CurrHealth;

    PlayerController mPlayerController;
    GameManager.Border mBorder = new GameManager.Border();

    Vector3 mMoveDir;       // The move direction.
    float mTimer;           // Count the time to shoot.
    bool mIsDead = false;   // Check whether it's dead.

    void Start()
    {
        mPlayerController = GameManager.sSingleton.playerController;
        mBorder = EnemyManager.sSingleton.GetBorder;

        // Get the current health and set the move direction.
        m_CurrHealth = EnemyManager.sSingleton.enemy1StartHealth;
        SetMoveDirection();
    }

    void OnEnable()
    {
        // Set new direction after re-enabled.
        if (!mBorder.IsZero())
        {
            m_CurrHealth = EnemyManager.sSingleton.enemy1StartHealth;
            SetMoveDirection();
            mIsDead = false;
        }
    }

    void Update()
    {
        if (!GameManager.sSingleton.IsBattle()) return;

        // Movement.
        transform.Translate(mMoveDir * m_MoveSpeed * Time.deltaTime, Space.World);

        // Attack.
        if (m_IsShoot)
        {
            mTimer += Time.deltaTime;
            if (mTimer >= m_ShootTime)
            {
                ActivateBullet();
                mTimer = 0;
            }
        }

        // Prevent the enemy from moving out of screen.
        transform.position = (new Vector3(
            Mathf.Clamp(transform.position.x, mBorder.left, mBorder.right),
            Mathf.Clamp(transform.position.y, mBorder.bottom, mBorder.top),
            transform.position.z)
        );

        // Set a new direction after hitting the border.
        if (transform.position.x == mBorder.left || transform.position.x == mBorder.right ||
            transform.position.y == mBorder.top || transform.position.y == mBorder.bottom)
        {
            SetMoveDirection();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet") || collision.CompareTag("PlayerRocket") || collision.CompareTag("EnemyBullet"))
        {
            // Check whether it's the player or another enemy hit it.
            bool isAddToPlayer = collision.CompareTag("EnemyBullet") ? false : true;

            GetDamaged(collision.GetComponent<BulletMove>().damage, isAddToPlayer);
            collision.gameObject.SetActive(false);

            if (collision.CompareTag("PlayerRocket"))
            {
                Transform trans = EnvironmentalObjManager.sSingleton.GetExplosion();
                trans.position = collision.transform.position;
                trans.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        // To prevent killing the enemy multiple times because of trigger enter and trigger stay.
        if (mIsDead) return;

        if (collision.CompareTag("Explosion"))
        {
            GetDamaged(collision.GetComponent<BulletMove>().damage, true);
        }
    }

    /// ----------------------------------------------------------------------------------------------
    /// ---------------------------------- PRIVATE FUNCTIONS -----------------------------------------
    /// ----------------------------------------------------------------------------------------------

    void SetMoveDirection()
    {
        // Get the direction.
        Vector3 dir1 = mPlayerController.transform.position - transform.position;
        Vector3 dir2 = EnemyManager.sSingleton.GetNearestEnemy(enemyID).position - transform.position;

        // Get the distance.
        float dist1 = dir1.sqrMagnitude;
        float dist2 = dir2.sqrMagnitude;

        // Set the direction to whoever is nearer.
        if (dist1 < dist2 || dist2 == 0) mMoveDir = dir1.normalized;
        else mMoveDir = dir2.normalized;

        // Rotate to face the move direction.
        Quaternion rot = transform.rotation;
        float z = Mathf.Atan2(mMoveDir.y, mMoveDir.x) * Mathf.Rad2Deg - 90;
        rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, z);
        transform.rotation = rot;
    }

    void ActivateBullet()
    {
        Transform trans = BulletManager.sSingleton.GetEnemyBullet();

        trans.position = transform.position + transform.up * ((EnemyManager.sSingleton.GetEnemySize.y / 2) + 0.5f);
        trans.rotation = transform.rotation;
        trans.gameObject.SetActive(true);
    }

    void GetDamaged(int dmg, bool isAddToPlayer)
    {
        m_CurrHealth -= dmg;

        if (m_CurrHealth <= 0)
        {
            // Disable the enemy.
            mIsDead = true;
            gameObject.SetActive(false);
            EnemyManager.sSingleton.MinusEnemy();

            // Update the score and kill.
            if (isAddToPlayer)
            {
                mPlayerController.AddScore(m_KillScore);
                mPlayerController.AddKill();
            }

            // Drop the coin.
            Transform trans = EnvironmentalObjManager.sSingleton.GetCoin();
            trans.position = transform.position;
            trans.gameObject.SetActive(true);
        }
        else
        {
            if (isAddToPlayer) mPlayerController.AddScore(m_DamageScore);
        }
    }
}
