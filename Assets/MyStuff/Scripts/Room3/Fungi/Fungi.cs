using UnityEngine;
public class Fungi : MonoBehaviour
{
    protected FungiData fungiData;
    public FungiData FungiData => fungiData;
    [SerializeField] protected MeshRenderer mainBody;
    [SerializeField] protected Material sporeBodyOutlinedMaterial; // Reference to the shared material

    // Use MaterialPropertyBlock to avoid creating material instances
    private static MaterialPropertyBlock materialPropertyBlock;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    [SerializeField] protected GameObject sporeRef;

    [SerializeField] protected Rigidbody rb;

    // Sinusoidal movement variables
    private Vector3 initialPosition;
    private Vector3 forwardDirection;
    private Vector3 rightDirection;
    private float timeElapsed = 0f;
    private bool isMoving = false;

    [SerializeField] private float amplitude = 2f;
    [SerializeField] private float frequency = 1f;
    private float phaseOffset = 0f;

    // Timeout settings
    [SerializeField] private float lifeTime = 4f;
    private float timeAlive = 0f;

    public void SetMovementParameters(float newAmplitude, float newFrequency, float newPhaseOffset = 0f)
    {
        amplitude = newAmplitude;
        frequency = newFrequency;
        phaseOffset = newPhaseOffset;
    }
    public void Init(FungiData data)
    {
        fungiData = data;

        initialPosition = transform.position;
        forwardDirection = transform.forward.normalized;
        rightDirection = transform.right.normalized;

        isMoving = true;
        timeElapsed = 0f;
        timeAlive = 0f;  // Reset lifetime timer

        // Use MaterialPropertyBlock to set color without creating material instances
        if (sporeBodyOutlinedMaterial != null && mainBody != null)
        {
            // Initialize static MaterialPropertyBlock if needed
            if (materialPropertyBlock == null)
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }

            // Set the material if not already set
            if (mainBody.sharedMaterial != sporeBodyOutlinedMaterial)
            {
                mainBody.sharedMaterial = sporeBodyOutlinedMaterial;
            }

            // Clear previous properties and set color
            materialPropertyBlock.Clear();

            // Try setting color properties (checking which one the shader uses)
            materialPropertyBlock.SetColor(BaseColorID, data.FungiColorBody);
            materialPropertyBlock.SetColor(ColorID, data.FungiColorBody);

            // Apply the property block to the renderer
            mainBody.SetPropertyBlock(materialPropertyBlock);
        }

        rb.isKinematic = false; // Allow physics interactions for collision detection
        rb.useGravity = false;  // Disable gravity since we want controlled movement
        rb.linearDamping = 0f;  // No drag for smooth movement
    }

    private void Update()
    {
        if (isMoving && fungiData != null)
        {
            timeElapsed += Time.deltaTime;
            timeAlive += Time.deltaTime;

            // Check if fungi has exceeded its lifetime (4 seconds)
            if (timeAlive >= lifeTime)
            {
                isMoving = false;
                gameObject.SetActive(false);
                return;
            }

            float forwardDistance = fungiData.Speed * timeElapsed;

            // Calculate sinusoidal offset with phase offset for variety
            float sineOffset = amplitude * Mathf.Sin(frequency * timeElapsed + phaseOffset);

            Vector3 newPosition = initialPosition +
                                 (forwardDirection * forwardDistance) +
                                 (rightDirection * sineOffset);

            // Use MovePosition for physics-based movement that can detect collisions
            rb.MovePosition(newPosition);
        }
    }
    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerScoreManager.Instance.Contact(fungiData);
        }

        isMoving = false;
        gameObject.SetActive(false);
    }

    // Add trigger detection as backup
    protected void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            PlayerScoreManager.Instance.Contact(fungiData);
            isMoving = false;
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        FungiManager.Instance.HasDied(this);
    }
}
