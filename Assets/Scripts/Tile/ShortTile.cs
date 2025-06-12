using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShortTile : MonoBehaviour
{
    //Speed
    private float fallSpeed;
    private float baseSpeed = 5f;

    //BMP and time
    private float bpm = 144f;
    private float demoTime = 7f;
    private Animator animator;
    private float spawnTime;

    //Perfect zone
    [SerializeField] private PerfectZone perfectZone;

    //Effect score type
    [SerializeField] private ParticleSystem perfectEffect;
    [SerializeField] private ParticleSystem goodEffect;

    //Check tapped
    private bool isTapped = false;

    //Side left/right
    private bool isLeftSide;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTapped)
        {
            Vector2? tapPos = GetTapPositionIfTapped();
            if (tapPos.HasValue)
            {
                if (GameController.Instance.IsFirstTile(this.gameObject, IsLeftSide)) ;
                {
                    isTapped = true;
                    GameController.Instance.RemoveFirstTile(IsLeftSide);
                    StartCoroutine(TappedAnimation(tapPos.Value.y));
                }
            }
        }
        SpeedUpOverTime();
    }

    public void SpeedUpOverTime()
    {
        fallSpeed = baseSpeed * (bpm / 60f);

        if (Time.timeSinceLevelLoad < demoTime)
            fallSpeed *= 0.3f;
        else if (Time.timeSinceLevelLoad >= demoTime && Time.timeSinceLevelLoad < 60f)
            fallSpeed *= 0.5f;
        else
            fallSpeed *= 0.8f;

        //Debug.Log("Fall speed : " + fallSpeed);
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);//Lien tuc roi xuong
    }

    IEnumerator TappedAnimation(float tapY)
    {
        if (animator != null)
        {
            animator.Play("tapped");
            GameController.Instance.OnTileDestroyed(tapY);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.2f);
        }
        Destroy(gameObject);
    }

    public Vector2? GetTapPositionIfTapped()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                return worldPoint;
            }
        }
#else
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            return worldPoint;
        }
    }
#endif
        return null;
    }

    public bool IsTapped {  get { return isTapped; } }
    public float SpawnTime { set => spawnTime = value; }
    public bool IsLeftSide { get => isLeftSide; set => isLeftSide = value;}
}
