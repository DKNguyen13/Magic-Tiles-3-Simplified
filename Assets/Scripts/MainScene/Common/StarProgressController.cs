using UnityEngine;

public class StarProgressController : MonoBehaviour
{
    private StarState state = StarState.Idle;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimation(StarState newState)
    {
        if (animator != null && state != newState)
        {
            state = newState;
            animator.Play(newState.ToString().ToLower()); // "idle", "half", "full"
        }
    }
}
