using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ResponsiveTileScaler : MonoBehaviour
{
    [Range(1, 10)]
    public int tilesPerRow = 4;

    void Start()
    {
        ResizeWidthToFitSlot();
    }

    void ResizeWidthToFitSlot()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float screenHeight = 2f * cam.orthographicSize;// Tinh chieu cao man hinh
        float screenWidth = screenHeight * cam.aspect;// Tinh chieu rong man hinh theo ty le khung hinh
        float targetWidth = screenWidth / tilesPerRow;// Chia deu chieu rong man hinh cho so luong tile

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float currentWidth = sr.bounds.size.x;// Lay chieu rong thuc te cua sprite trong world space

        if (currentWidth <= 0f) return;

        float scaleFactor = targetWidth / currentWidth;

        Vector3 newScale = transform.localScale;
        newScale.x *= scaleFactor;
        transform.localScale = newScale;
    }
}
