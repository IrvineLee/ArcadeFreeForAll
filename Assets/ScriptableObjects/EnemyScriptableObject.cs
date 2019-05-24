using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemy", order = 1)]
public class EnemyScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class Enemy
    {
        public Transform prefab;
        public int count;

        public Enemy(Transform prefab, int count)
        {
            this.prefab = prefab;
            this.count = count;
        }
    }

    public List<Enemy> prefabList = new List<Enemy>();
}