using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class cube1script : MonoBehaviour
{
   private XRGrabInteractable grabInteractable;

    public bool grabbed = false;

    private void Awake()
    {
        // Get the XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Ensure that the component is available
        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable component is missing!");
        }
    }

    private void OnEnable()
    {
        // Register to the events for when the object is grabbed or released
        grabInteractable.onSelectEntered.AddListener(OnGrabbed);
        grabInteractable.onSelectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        // Unregister from the events when the object is disabled
        grabInteractable.onSelectEntered.RemoveListener(OnGrabbed);
        grabInteractable.onSelectExited.RemoveListener(OnReleased);
    }

    // This is called when the object is grabbed
    private void OnGrabbed(XRBaseInteractor interactor)
    {
        Debug.Log("Cube grabbed!");
    }

    // This is called when the object is released
    private void OnReleased(XRBaseInteractor interactor)
    {
        Debug.Log("Cube released!");
        grabbed = true; // Set grabbed to true when the cube is grabbed
    }
}
