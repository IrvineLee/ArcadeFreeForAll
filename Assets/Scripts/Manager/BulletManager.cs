using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager sSingleton { get { return _sSingleton; } }
    static BulletManager _sSingleton;

    [Header("Data")]
    [SerializeField] BulletsScriptableObject m_BulletData;

    // Bullet list.
    List<Transform> mPlayerBulletList = new List<Transform>();
    List<Transform> mPlayerRocketList = new List<Transform>();
    List<Transform> mEnemyBulletList = new List<Transform>();

    // The current index of bullets in list.
    int mPlayerBulletIndex, mPlayerRocketIndex, mEnemyBulletIndex;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        CacheBullets();
    }

    /// ---------------------------------------------------------------------------------------------
    /// --------------------------------- PUBLIC FUNCTIONS ------------------------------------------
    /// ---------------------------------------------------------------------------------------------

    public void DisableAllBullets()
    {
        DisableBullets(ref mPlayerBulletList);
        DisableBullets(ref mPlayerRocketList);
        DisableBullets(ref mEnemyBulletList);
    }

    // ----------------------------------------------------------------------------------------------
    // --------------------------------------- GETTER -----------------------------------------------
    // ----------------------------------------------------------------------------------------------

    // Get inactive bullets.
    public Transform GetPlayerBullet()  { return GetBullet(ref mPlayerBulletList, ref mPlayerBulletIndex); }
    public Transform GetPlayerRocket()  { return GetBullet(ref mPlayerRocketList, ref mPlayerRocketIndex); }
    public Transform GetEnemyBullet()   { return GetBullet(ref mEnemyBulletList, ref mEnemyBulletIndex); }

    /// ----------------------------------------------------------------------------------------------
    /// ---------------------------------- PRIVATE FUNCTIONS -----------------------------------------
    /// ----------------------------------------------------------------------------------------------

    Transform GetBullet(ref List<Transform> bulletList, ref int bulletIndex)
    {
        int total = bulletList.Count - 1;

        if (bulletIndex + 1 > total) bulletIndex = 0;
        else bulletIndex++;

        return bulletList[bulletIndex];
    }

    void DisableBullets(ref List<Transform> bulletList)
    {
        for (int i = 0; i < bulletList.Count; i++)
        {
            bulletList[i].gameObject.SetActive(false);
        }
    }

    // Instantiate the bullets at the start of the game.
    void CacheBullets()
    {
        for (int i = 0; i < m_BulletData.prefabList.Count; i++)
        {
            BulletsScriptableObject.Bullet currBullet = m_BulletData.prefabList[i];

            if (currBullet.prefab.CompareTag("PlayerBullet")) AddToList(currBullet, ref mPlayerBulletList);
            else if (currBullet.prefab.CompareTag("PlayerRocket")) AddToList(currBullet, ref mPlayerRocketList);
            else if (currBullet.prefab.CompareTag("EnemyBullet")) AddToList(currBullet, ref mEnemyBulletList);
        }
    }

    void AddToList(BulletsScriptableObject.Bullet currBullet, ref List<Transform> currList)
    {
        GameObject parent = new GameObject();
        parent.name = "Parent_" + currBullet.prefab.name;

        for (int j = 0; j < currBullet.count; j++)
        {
            Transform trans = Instantiate(currBullet.prefab, Vector3.zero, Quaternion.identity);
            trans.SetParent(parent.transform);
            trans.gameObject.SetActive(false);

            currList.Add(trans);
        }
    }
}
