using System.Collections;
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
    protected Coroutine coroutine;
    protected WaitForSeconds wait;
    [SerializeField] protected float moveFrequency = .01f;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        wait = new WaitForSeconds(moveFrequency);
    }
    private void OnEnable()
    {
        //begin moving forward
        coroutine = StartCoroutine(MoveForward());
    }
    IEnumerator MoveForward()
    {
        while (true)
        {
            yield return wait;
            transform.Translate(-transform.forward * fungiData.Speed);
        }
    }
    public void Init(FungiData data)
    {
        fungiData = data;
        meshRenderer.material = data.FungiMaterial;
    }
    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.root.gameObject.tag == "Player")
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
    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}
