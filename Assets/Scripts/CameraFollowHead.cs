using UnityEngine;

public class CameraFollowHead : MonoBehaviour
{
    [SerializeField] private Animator characterAnimator;
    
    [Tooltip("Optionnel : Assigne un Transform (ex: un GameObject vide) si ton perso n'est pas Humanoïde")]
    [SerializeField] private Transform fallbackHeadTransform; 
    
    [SerializeField] private Vector3 offset = new Vector3(0, 0.15f, 0.1f);

    void LateUpdate()
    {
        if (characterAnimator == null) return;

        Transform head = null;

        // 1. On vérifie si l'Animator a bien un Avatar Humanoïde valide
        if (characterAnimator.avatar != null && characterAnimator.isHuman)
        {
            head = characterAnimator.GetBoneTransform(HumanBodyBones.Head);
        }
        // 2. Sinon, on utilise le point manuel de secours si tu en as glissé un
        else if (fallbackHeadTransform != null)
        {
            head = fallbackHeadTransform;
        }

        // 3. On applique la position si on a trouvé la tête
        if (head != null)
        {
            transform.position = head.position + head.rotation * offset;
            //transform.rotation = head.rotation;
        }
    }
}
