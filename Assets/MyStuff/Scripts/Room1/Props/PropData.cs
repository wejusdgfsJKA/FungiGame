using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PropData")]
public class PropData : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; protected set; }
    [field: SerializeField] public Transform[] SpawnPoints { get; protected set; }
}
