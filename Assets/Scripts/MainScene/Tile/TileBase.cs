using UnityEngine;

public abstract class TileBase : MonoBehaviour
{
    //Speed
    [SerializeField] protected float fallSpeed;
    [SerializeField] protected float baseSpeed = 5f;

    //BMP and time
    protected float bpm;
    protected float demoTime = 7f;
    protected float spawnTime;

    //Check tapped
    [SerializeField] protected bool isTapped = false;

    //Side left/right
    protected bool isLeftSide;

    //Animator
    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    protected abstract void Start();

    // Update is called once per frame
    protected abstract void Update();

    public abstract void ResetTile();
}
