using UnityEngine;
[System.Serializable]
public class PropData
{
    [field: SerializeField] public GameObject Prefab { get; protected set; }
    [field: SerializeField] public Transform[] SpawnPoints { get; protected set; }
}
