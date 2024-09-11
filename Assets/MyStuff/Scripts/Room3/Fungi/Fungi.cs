using UnityEngine;
public class Fungi : MonoBehaviour
{
    protected FungiData fungiData;
    [SerializeField] protected MeshRenderer meshRenderer;
    [SerializeField] protected Rigidbody rb;
    public void Init(FungiData data)
    {
        fungiData = data;
        rb.WakeUp();
        rb.AddForce(transform.forward * data.Speed, ForceMode.Impulse);
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
