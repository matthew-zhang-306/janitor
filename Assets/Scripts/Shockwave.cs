using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float expandSpeed;
    public float maxSize;

    private LineRenderer lineRenderer;
    public new Collider2D collider;


    private void Start() {
        transform.localScale = Vector3.zero;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate() {
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;

        float lineAlpha = Mathf.Lerp(0f, 1f, maxSize - transform.localScale.x);
        lineRenderer.startColor = lineRenderer.startColor.WithAlpha(lineAlpha);
        lineRenderer.endColor = lineRenderer.endColor.WithAlpha(lineAlpha);

        collider.enabled = lineAlpha > 0.5f;

        if (transform.localScale.x >= maxSize) {
            Destroy(gameObject);
        }
    }


    [ContextMenu("Create Circle")]
    public void CreateCircle() {
        int numPoints = 51;

        var line = GetComponent<LineRenderer>();
        var edge = GetComponent<EdgeCollider2D>();
        
        Vector2[] positions2D = new Vector2[numPoints + 1];
        Vector3[] positions = new Vector3[numPoints];
        int i = 0;
        for (float angle = 0; angle < 360f; angle += 360f / numPoints) {
            positions2D[i] = Quaternion.Euler(0, 0, angle) * Vector2.up;
            positions[i] = positions2D[i].ToVector3();
            i++;
        }
        positions2D[numPoints] = positions2D[0];

        line.positionCount = numPoints;
        line.SetPositions(positions);
        edge.points = positions2D;
    }
}
