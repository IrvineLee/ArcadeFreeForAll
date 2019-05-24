using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BulletData", menuName = "ScriptableObjects/Bullets", order = 1)]
public class BulletsScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class Bullet
    {
        public Transform prefab;
        public int count;

        public Bullet(Transform prefab, int count)
        {
            this.prefab = prefab;
            this.count = count;
        }
    }

    public List<Bullet> prefabList = new List<Bullet>();
}