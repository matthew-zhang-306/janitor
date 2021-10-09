using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SnapToGrid : MonoBehaviour
{
    public Vector2 gridSize = new Vector2(0.5f, 0.5f);

    public void SnapCollider() {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        PrepareForSnap();
        if (transform.lossyScale.x == 0 || transform.lossyScale.y == 0) {
            Debug.LogError("Cannot snap collider of " + gameObject.name + " because it has scale 0 in at least one dimension!");
            return;
        }

        // get old min/max using math
        Vector2 lossyScale = transform.lossyScale;
        Vector2 center = transform.position.ToVector2() + boxCollider.offset * lossyScale;
        Vector2 min = center - boxCollider.size / 2f * lossyScale;
        Vector2 max = center + boxCollider.size / 2f * lossyScale;

        min = min.RoundToNearest(gridSize);
        max = max.RoundToNearest(gridSize);
        if (min == max) {
            // ensure the collider is at least 1 tile large
            max += Vector2.one * gridSize;
        }
        center = (min + max) / 2f;

        Debug.DrawLine(center, min, Color.white, 5f);
        Debug.DrawLine(center, max, Color.white, 5f);

        // set new offset/size values using math
        boxCollider.offset = (center - transform.position.ToVector2()) / lossyScale;
        boxCollider.size = (max - min) / lossyScale;
    }

    public void SnapPosition() {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        PrepareForSnap();

        Vector2 lossyScale = transform.lossyScale;
        Vector2 size = boxCollider.size * lossyScale;
        size = size.RoundToNearest(size);
        int sizeX = Mathf.RoundToInt(size.x);
        int sizeY = Mathf.RoundToInt(size.y);

        // snap position to the grid, keeping in mind approximately how big the collider is
        Vector2 positionOffset = new Vector2(
            (sizeX % 2) * (gridSize.x / 2),
            (sizeY % 2) * (gridSize.y / 2)
        );
        transform.position = transform.position.ToVector2().RoundToNearest(gridSize, positionOffset).ToVector3();
    }

    public void SnapPositionAndCollider() {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        PrepareForSnap();
        if (transform.lossyScale.x == 0 || transform.lossyScale.y == 0) {
            Debug.LogError("Cannot snap collider of " + gameObject.name + " because it has scale 0 in at least one dimension!");
            return;
        }

        // get old min/max using math
        Vector2 lossyScale = transform.lossyScale;
        Vector2 center = transform.position.ToVector2() + boxCollider.offset * lossyScale;
        Vector2 min = center - boxCollider.size / 2f * lossyScale;
        Vector2 max = center + boxCollider.size / 2f * lossyScale;

        min = min.RoundToNearest(gridSize);
        max = max.RoundToNearest(gridSize);
        if (min == max) {
            // ensure the collider is at least 1 tile large
            max += Vector2.one * gridSize;
        }
        center = (min + max) / 2f;

        Debug.DrawLine(center, min, Color.white, 5f);
        Debug.DrawLine(center, max, Color.white, 5f);

        // set new offset/size values using math
        transform.position = center;
        boxCollider.offset = Vector2.zero;
        boxCollider.size = (max - min) / lossyScale;
    }


    private void PrepareForSnap() {
        // this object needs to have zero rotation and non-zero scale for snapping to work
        transform.rotation = Quaternion.identity;

        // the object should also have a non-zero scale in both directions
        var scale = transform.localScale;
        if (scale.x == 0)
            scale.x = 1;
        if (scale.y == 0)
            scale.y = 1;
        transform.localScale = scale;
    }
}
