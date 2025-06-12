using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ShortTile"))
        {
            ShortTile tile = collision.GetComponent<ShortTile>();
            if (tile != null && tile.IsTapped)
            {
                return;
            }
            Debug.Log("Death zone");
            GameController.Instance.GameOver();
        }
    }
}
