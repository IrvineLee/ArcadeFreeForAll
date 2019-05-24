using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnvObjData", menuName = "ScriptableObjects/PickUp", order = 1)]
public class EnvObjScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class PickUp
    {
        public Transform prefab;
        public int count;

        public PickUp(Transform prefab, int count)
        {
            this.prefab = prefab;
            this.count = count;
        }
    }

    public List<PickUp> prefabList = new List<PickUp>();
}