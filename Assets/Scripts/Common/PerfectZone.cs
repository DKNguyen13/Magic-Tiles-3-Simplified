using UnityEngine;

public class PerfectZone : MonoBehaviour
{
    private Animator zoneAnimator;

    //CurrentPosY check distance cho score
    private float currentPosY;

    // Thresholds
    [SerializeField] private float perfectThreshold = 0.3f;
    [SerializeField] private float greatThreshold = 0.6f;

    private void Awake()
    {
        zoneAnimator = GetComponent<Animator>();
    }

    public void UpdatePosition(Vector2 tapPosition, bool triggerAnimation)
    {
        Vector3 currentPosition = transform.position;

        currentPosition.y = tapPosition.y;
        transform.position = currentPosition;

        if (triggerAnimation && zoneAnimator != null)
        {
            zoneAnimator.Play("glow");
        }
    }

    public ScoreType CalculateScore(float tapY)
    {
        float distance = Mathf.Abs(tapY - currentPosY);

        if (distance <= perfectThreshold)
            return ScoreType.Perfect;
        else if (distance <= greatThreshold)//Greate type
        {
            currentPosY = tapY;
            return ScoreType.Great;
        }
        else//Cool type
        {
            currentPosY = tapY;
            return ScoreType.Cool;
        }
    }

    public ScoreType HandleTapAndScore(Vector2 tapPosition)
    {
        UpdatePosition(tapPosition, true);
        return CalculateScore(tapPosition.y);
    }
}
