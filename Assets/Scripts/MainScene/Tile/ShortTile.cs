using System.Collections;
using UnityEngine;

public class ShortTile : TileBase
{
    protected override void Start()
    {
        bpm = 144f;
        demoTime = 7f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!isTapped)
        {
            Vector2? tapPos = GetTapPositionIfTapped();
            if (tapPos.HasValue)
            {
                if (GameController.Instance.IsFirstTile(this.gameObject, IsLeftSide))
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
        {
            fallSpeed *= 0.3f;
        }
        else if (Time.timeSinceLevelLoad >= demoTime && Time.timeSinceLevelLoad < 60f)
        {
            fallSpeed *= 0.5f;
        }
        else if (Time.timeSinceLevelLoad >= 60f && Time.timeSinceLevelLoad < 100f)
        {
            fallSpeed *= 0.7f;
        }
        else if (Time.timeSinceLevelLoad >= 100f && Time.timeSinceLevelLoad < 114f)
        {
            fallSpeed *= 0.4f;//Sau 1:40s thi tiet tau nhac giam
        }
        else // 1:54s thi nhanh lai
        {
            fallSpeed *= 0.8f;
        }

        //Debug.Log("Fall speed : " + fallSpeed);
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);//Lien tuc roi xuong
    }

    IEnumerator TappedAnimation(float tapY)
    {
        if (animator != null)
        {
            animator.Play("tapped");
            GameController.Instance.OnTileDestroyed(tapY);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
        //Destroy(gameObject);
        TilePoolManager.Instance.ReturnTileToPool(this.gameObject);
    }

    public override void ResetTile()
    {
        isTapped = false;
        transform.position = Vector3.zero;
        //animator.Play("idle");
        gameObject.SetActive(false);
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

    //Getter, setter
    public bool IsTapped { get => isTapped; set => isTapped = value; }
    public float SpawnTime { set => spawnTime = value; }
    public bool IsLeftSide { get => isLeftSide; set => isLeftSide = value;}
}
