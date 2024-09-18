using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MicroscopeHolder : MonoBehaviour
{
    [Range(1, 2)]
    [SerializeField] protected int micNumber;
    [SerializeField] protected GameObject button;
    [SerializeField] protected Transform micPoint, lensPoint;
    protected bool mic = false, micLens = false;
    public void InsertedObject(Collider collider)
    {
        if (mic)
        {
            if (micLens)
            {
                if (collider.transform.tag == "FungiHolder")
                {
                    if (button.activeSelf == false)
                    {
                        collider.transform.GetComponent<XRGrabInteractable>().enabled = false;
                        var rb = collider.transform.GetComponent<Rigidbody>();
                        rb.isKinematic = true;
                        rb.useGravity = false;
                        collider.enabled = false;
                        collider.transform.GetComponent<MeshRenderer>().enabled = false;
                        transform.GetComponent<MeshRenderer>().enabled = false;
                        button.SetActive(true);
                    }
                }
            }
            else
            {
                //we need the lens
                if (collider.gameObject.tag == "Lens" + micNumber.ToString())
                {
                    micLens = true;
                    collider.transform.GetComponent<XRGrabInteractable>().enabled = false;
                    var rb = collider.transform.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    collider.enabled = false;
                    collider.transform.position = lensPoint.position;
                    collider.transform.rotation = lensPoint.rotation;
                    collider.transform.localScale = lensPoint.localScale;
                }
            }
        }
        else
        {
            if (collider.gameObject.tag == "Mic" + micNumber.ToString())
            {
                mic = true;
                collider.transform.GetComponent<XRGrabInteractable>().enabled = false;
                var rb = collider.transform.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
                collider.enabled = false;
                collider.transform.position = micPoint.position;
                collider.transform.rotation = micPoint.rotation;
                collider.transform.localScale = micPoint.localScale;
            }
        }
    }
}
