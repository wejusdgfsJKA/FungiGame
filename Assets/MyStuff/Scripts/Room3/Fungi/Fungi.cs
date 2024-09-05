using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/FungiData")]
public class FungiData : ScriptableObject
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
public class Fungi : MonoBehaviour
{
    protected FungiData fungiData;
    protected MeshRenderer meshRenderer;
    protected Rigidbody rb;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }
    public void Init(FungiData data)
    {
        fungiData = data;
        rb.velocity = transform.forward * fungiData.Speed;
        meshRenderer.material = data.FungiMaterial;
    }
    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.gameObject.tag == "Player")
        {
            PlayerScoreManager.Instance.Contact(fungiData);
        }
        Die();
    }
    protected void Die()
    {
        gameObject.SetActive(false);
        FungiManager.Instance.HasDied(this);
    }
}
