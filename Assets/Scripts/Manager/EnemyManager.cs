using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager sSingleton { get { return _sSingleton; } }
    static EnemyManager _sSingleton;

    // The enemy data.
    [Header("Data")]
    [SerializeField] EnemyScriptableObject m_EnemyData;

    #region Enemy
    [Header("Enemy")]
    public int enemy1StartHealth = 3;
    [SerializeField] int m_StartEnemy = 4;
    [SerializeField] int m_MaxIncPerRound = 2;
    [SerializeField] float m_RespawnCD = 1.5f;
    [SerializeField] float m_CDReducePerRound = 0.1f;
    #endregion

    // To display the current spawned enemies and the maximum enemies for current round.
    [ReadOnly][SerializeField] int m_CurrEnemy, m_CurrRoundMaxEnemy;

    // Instantiated enemies.
    List<Transform> mEnemyist = new List<Transform>();
    int mEnemyIndex;

    // To count the time to respawn killed enemy.
    float mRespawnTimer;

    // The size of enemy to create the in-game boundaries.
    Vector3 mEnemySize;
    GameManager.Border mBorder = new GameManager.Border();

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        float distance = (transform.position - Camera.main.transform.position).z;

        // TODO: I'm assuming all enemies have the same size. Change this if there are different enemy sizes.
        mEnemySize = m_EnemyData.prefabList[0].prefab.GetComponent<Renderer>().bounds.size;

        // Set the playable boudaries.
        mBorder.left = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0, distance)).x + (mEnemySize.x / 2);
        mBorder.right = Camera.main.ViewportToWorldPoint(new Vector3(0.75f, 0, distance)).x - (mEnemySize.x / 2);
        mBorder.top = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, distance)).y - (mEnemySize.y / 2);
        mBorder.bottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).y + (mEnemySize.y / 2);

        CacheEnemy();
        m_CurrRoundMaxEnemy = m_StartEnemy;
    }

    void Update()
    {
        if (!GameManager.sSingleton.IsBattle()) return;

        // Respawn enemy when current enemy is lower than the max amount.
        if (m_CurrEnemy < m_CurrRoundMaxEnemy)
        {
            mRespawnTimer += Time.deltaTime;
            if (mRespawnTimer >= m_RespawnCD)
            {
                SpawnEnemy();
                mRespawnTimer = 0;
            }
        }
    }

    /// ---------------------------------------------------------------------------------------------
    /// --------------------------------- PUBLIC FUNCTIONS ------------------------------------------
    /// ---------------------------------------------------------------------------------------------

    public void NextRound()
    {
        m_CurrEnemy = 0;
        m_CurrRoundMaxEnemy = m_StartEnemy + (GameManager.sSingleton.currRound - 1) * m_MaxIncPerRound;
        GameManager.sSingleton.currRound++;
        SetEnemyForTheRound();
    }

    public void DisableAllEnemies()
    {
        for (int i = 0; i < mEnemyist.Count; i++)
        {
            mEnemyist[i].gameObject.SetActive(false);
        }
    }

    public void MinusEnemy() { m_CurrEnemy--; }

    // Return the nearest enemy.
    public Transform GetNearestEnemy(int currIndex)
    {
        float minDistance = Mathf.Infinity;
        int closestIndex = currIndex;

        // Scan all enemy to see which are nearer.
        for (int i = 0; i < mEnemyist.Count; i++)
        {
            if (i == currIndex || !mEnemyist[i].gameObject.activeSelf) continue;

            float dist = (mEnemyist[i].position - mEnemyist[currIndex].position).sqrMagnitude;

            if (dist < minDistance)
            {
                minDistance = dist;
                closestIndex = i;
            }
        }

        return mEnemyist[closestIndex];
    }

    public void ReduceSpawnCD()
    {
        m_RespawnCD -= m_CDReducePerRound;
        if (m_RespawnCD < 0) m_RespawnCD = 0;
    }

    // ----------------------------------------------------------------------------------------------
    // --------------------------------------- GETTER -----------------------------------------------
    // ----------------------------------------------------------------------------------------------

    public GameManager.Border GetBorder { get { return mBorder; } }
    public Vector3 GetEnemySize { get { return mEnemySize; } }

    /// ---------------------------------------------------------------------------------------------
    ///----------------------------------- PRIVATE FUNCTIONS ----------------------------------------
    /// ---------------------------------------------------------------------------------------------

    // Set the enemy at the start of round.
    void SetEnemyForTheRound()
    {
        for (int i = 0; i < m_CurrRoundMaxEnemy; i++)
        {
            SpawnEnemy();
        }
    }

    // Spawn the enemy.
    void SpawnEnemy()
    {
        m_CurrEnemy++;
        Transform trans = GetEnemy();
        trans.position = GetRandomPos();
        trans.gameObject.SetActive(true);
    }

    // Get inactive enemy from the List.
    Transform GetEnemy()
    {
        int total = mEnemyist.Count - 1;

        if (mEnemyIndex + 1 > total) mEnemyIndex = 0;
        else mEnemyIndex++;

        return mEnemyist[mEnemyIndex];
    }

    // Get random position within the border.
    Vector3 GetRandomPos()
    {
        float x = Random.Range(mBorder.left, mBorder.right);
        float y = Random.Range(mBorder.top, mBorder.bottom);

        return new Vector3(x, y, 0);
    }

    // Instantiate all enemeies and disabled them.
    void CacheEnemy()
    {
        for (int i = 0; i < m_EnemyData.prefabList.Count; i++)
        {
            EnemyScriptableObject.Enemy currEnemy = m_EnemyData.prefabList[i];
            GameObject parent = new GameObject();
            parent.name = "Parent_" + currEnemy.prefab.name;
            
            for (int j = 0; j < currEnemy.count; j++)
            {
                Transform trans = Instantiate(currEnemy.prefab, Vector3.zero, Quaternion.identity);
                trans.SetParent(parent.transform);
                trans.GetComponent<Enemy>().enemyID = j;
                trans.gameObject.SetActive(false);

                mEnemyist.Add(trans);
            }
        }
    }
}
