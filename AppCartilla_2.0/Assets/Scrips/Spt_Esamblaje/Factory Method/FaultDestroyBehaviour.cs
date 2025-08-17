using UnityEngine;

public class FaultDestroyBehaviour : StateMachineBehaviour
{
    public AudioClip destroySound; // Asigna el AudioClip aquí en el Inspector

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioSource audioSource = animator.GetComponent<AudioSource>();
        if (audioSource != null && destroySound != null)
        {
            audioSource.PlayOneShot(destroySound);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject); // Destruye el enemigo después del sonido
    }
}