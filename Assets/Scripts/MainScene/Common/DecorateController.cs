using UnityEngine;

public class DecorateController : MonoBehaviour
{
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void Highlight()
    {
        animator.Play("light");
    }

    public void SpeedUpAnimation()
    {
        animator.Play("speed_up");
    }
}
