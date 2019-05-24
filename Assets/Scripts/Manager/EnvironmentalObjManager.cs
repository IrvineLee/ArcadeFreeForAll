using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalObjManager : MonoBehaviour
{
    public static EnvironmentalObjManager sSingleton { get { return _sSingleton; } }
    static EnvironmentalObjManager _sSingleton;

    public EnvObjScriptableObject m_EnvObjData;

    List<Transform> mCoinList = new List<Transform>();
    List<Transform> mExplosionList = new List<Transform>();
    int mCoinIndex, mExplosionIndex;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        CacheCoins();
    }

    public Transform GetCoin() { return GetEnvObj(ref mCoinList, ref mCoinIndex); }
    public Transform GetExplosion() { return GetEnvObj(ref mExplosionList, ref mExplosionIndex); }

    public void DisableAllPickUps()
    {
        DisableEnvObj(ref mCoinList);
        DisableEnvObj(ref mExplosionList);
    }

    Transform GetEnvObj(ref List<Transform> envObjist, ref int envObjIndex)
    {
        int total = envObjist.Count - 1;

        if (envObjIndex + 1 > total) envObjIndex = 0;
        else envObjIndex++;

        return envObjist[envObjIndex];
    }

    void DisableEnvObj(ref List<Transform> enbObjList)
    {
        for (int i = 0; i < enbObjList.Count; i++)
        {
            enbObjList[i].gameObject.SetActive(false);
        }
    }

    void CacheCoins()
    {
        for (int i = 0; i < m_EnvObjData.prefabList.Count; i++)
        {
            EnvObjScriptableObject.PickUp currPickUp = m_EnvObjData.prefabList[i];
            GameObject parent = new GameObject();
            parent.name = "Parent_" + currPickUp.prefab.name;

            for (int j = 0; j < currPickUp.count; j++)
            {
                Transform trans = Instantiate(currPickUp.prefab, Vector3.zero, Quaternion.identity);
                trans.SetParent(parent.transform);
                trans.gameObject.SetActive(false);

                if (currPickUp.prefab.CompareTag("Coin")) mCoinList.Add(trans);
                if (currPickUp.prefab.CompareTag("Explosion")) mExplosionList.Add(trans);
            }
        }
    }
}
