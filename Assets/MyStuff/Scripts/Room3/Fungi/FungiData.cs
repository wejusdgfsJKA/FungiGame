using UnityEngine;
[System.Serializable]
public class FungiData
{
    [field: SerializeField]
    public bool Beneficial { get; protected set; }
    [field: SerializeField]
    public float Speed { get; protected set; }
    //how much score will be lost or gained on contact
    [field: SerializeField]
    public int Score { get; protected set; }
    [field: SerializeField]
    public float Weight { get; protected set; }
    [field: SerializeField]
    public Material FungiMaterial { get; protected set; }
}