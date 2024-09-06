using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MicroscopeHolder : MonoBehaviour
{
    [Range(1, 2)]
    [SerializeField] protected int micNumber;
    [SerializeField] protected GameObject button;
    protected bool mic, micLens, fungiHolder;
    public void InsertedObject(Collider collider)
    {
        if (mic)
        {
            if (micLens)
            {
                if (collider.transform.tag == "FungiHolder")
                {
                    fungiHolder = true;
                    collider.transform.GetComponent<XRGrabInteractable>().enabled = false;
                    var rb = collider.transform.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;

                    if (micNumber == 1)
                    {
                        RoomManager.Instance.LoadRoom2();
                    }
                    else
                    {
                        RoomManager.Instance.LoadRoom3();
                    }
                }
            }
            else
            {
                //we need the lens
                if (collider.transform.gameObject.name == "Lens" + micNumber.ToString())
                {
                    micLens = true;
                    collider.transform.GetComponent<XRGrabInteractable>().enabled = false;
                    var rb = collider.transform.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    collider.enabled = false;
                }
            }
        }
        else
        {
            if (collider.transform.gameObject.name == "Mic" + micNumber.ToString())
            {
                mic = true;
                collider.transform.GetComponent<XRGrabInteractable>().enabled = false;
                var rb = collider.transform.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
                collider.enabled = false;
            }
        }
    }
}
