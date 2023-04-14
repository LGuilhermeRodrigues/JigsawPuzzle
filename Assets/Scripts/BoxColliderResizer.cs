using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderResizer : MonoBehaviour
{
    public void ResizeBoxCollider()
    {
        // Get the box collider
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        // Get the size of the image
        Vector2 imageSize = GetComponent<RectTransform>().sizeDelta;
        // Set the size of the box collider
        boxCollider.size = new Vector2(imageSize.x, imageSize.y);
        // Set the center of the box collider
        boxCollider.offset = new Vector2(imageSize.x / 2, -imageSize.y / 2);
    }
}
