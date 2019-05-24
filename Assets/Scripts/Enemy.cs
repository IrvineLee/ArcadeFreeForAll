using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyIndex;
    public int damageScore = 100;
    public int killScore = 500;

    public float moveSpeed = 1;
    public float shootTime = 1;
    public bool isShoot = true;

    [ReadOnly][SerializeField]float currHealth;
    [ReadOnly][SerializeField]Transform target;

    PlayerController mPlayerController;
    PlayerController.Border mBorder = new PlayerController.Border();

    Vector3 mMoveDir;
    float mTimer;
    bool mIsDead = false;

    void Start()
    {
        mPlayerController = GameManager.sSingleton.m_PlayerController;
        mBorder = EnemyManager.sSingleton.GetBorder;

        currHealth = EnemyManager.sSingleton.enemy1StartHealth;
        SetMoveDirection();
    }

    void OnEnable()
    {
        // Set new direction after re-enabled.
        if (!mBorder.IsZero())
        {
            currHealth = EnemyManager.sSingleton.enemy1StartHealth;
            SetMoveDirection();
            mIsDead = false;
        }
    }

    void Update()
    {
        if (!GameManager.sSingleton.IsBattle()) return;

        // Movement.
        transform.Translate(mMoveDir * moveSpeed * Time.deltaTime, Space.World);

        // Attack.
        if (isShoot)
        {
            mTimer += Time.deltaTime;
            if (mTimer >= shootTime)
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
        if (mIsDead) return;

        if (collision.CompareTag("Explosion"))
        {
            GetDamaged(collision.GetComponent<BulletMove>().damage, true);
        }
    }

    void SetMoveDirection()
    {
        // Get the direction.
        Vector3 dir1 = mPlayerController.transform.position - transform.position;
        Vector3 dir2 = EnemyManager.sSingleton.GetNearestEnemy(enemyIndex).position - transform.position;

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
        currHealth -= dmg;

        if (currHealth <= 0)
        {
            mIsDead = true;
            gameObject.SetActive(false);
            EnemyManager.sSingleton.MinusEnemy();

            if (isAddToPlayer)
            {
                mPlayerController.AddScore(killScore);
                mPlayerController.AddKill();
            }

            Transform trans = EnvironmentalObjManager.sSingleton.GetCoin();
            trans.position = transform.position;
            trans.gameObject.SetActive(true);
        }
        else
        {
            if (isAddToPlayer) mPlayerController.AddScore(damageScore);
        }
    }
}
