using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;

    public void OpenDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }
    }
}
