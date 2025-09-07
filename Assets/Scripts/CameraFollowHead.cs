using UnityEngine;

public class CameraFollowHead : MonoBehaviour
{
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.15f, 0.1f);

    void LateUpdate()
    {
        if (characterAnimator == null) return;

        Transform head = characterAnimator.GetBoneTransform(HumanBodyBones.Head);
        if (head != null)
        {
            transform.position = head.position + head.rotation * offset;
            transform.rotation = head.rotation;
        }
    }
}
