using UnityEngine;
using BoxColliderPlus;


public class OverlapTest : MonoBehaviour
{
    private Vector2 boxSize = new Vector2(1f, 1f); 
    public float overlapThreshold = 0.1f;         
    public LayerMask layerMask;                   
    private void Start() {
        boxSize = new Vector2(transform.localScale.x,transform.localScale.y);
        
    }
    void Update()
    {
        Collider2D[] significantOverlaps = OverlapUtility.GetSignificantOverlaps(transform.position, boxSize, layerMask, overlapThreshold);

        foreach (Collider2D collider in significantOverlaps)
        {
            Debug.Log("Significant overlap with: " + collider.name);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}

