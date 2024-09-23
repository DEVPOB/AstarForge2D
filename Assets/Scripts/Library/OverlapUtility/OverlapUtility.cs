using UnityEngine;
namespace BoxColliderPlus
{
   
    public static class OverlapUtility
    {
        /// <summary>
        /// Checks if the overlap between the hit object and the box at the given world point is smaller than the threshold.
        /// </summary>
        /// <param name="hit">The collider that was hit by the OverlapBox.</param>
        /// <param name="worldPoint">The center point of the OverlapBox in world coordinates.</param>
        /// <param name="boxSize">The size of the OverlapBox.</param>
        /// <param name="overlapThreshold">The maximum overlap size to consider as "small" (and ignorable).</param>
        /// <returns>True if the overlap is smaller than the threshold, otherwise false.</returns>
        public static bool IsSmallOverlap(Collider2D hit, Vector2 worldPoint, Vector2 boxSize, float overlapThreshold)
        {
            if (hit == null) return true; // No hit means there's no overlap at all

            // Get the bounds of the hit object and the OverlapBox
            Bounds hitBounds = hit.bounds;
            Bounds boxBounds = new Bounds(worldPoint, boxSize);

            // Calculate the overlap size
            Bounds overlapBounds = boxBounds;
            overlapBounds.Encapsulate(hitBounds);

            float overlapWidth = boxBounds.size.x + hitBounds.size.x - overlapBounds.size.x;
            float overlapHeight = boxBounds.size.y + hitBounds.size.y - overlapBounds.size.y;

            // Return true if the overlap is smaller than the threshold
            return overlapWidth >= overlapThreshold && overlapHeight >= overlapThreshold;
        }


        /// <summary>
        /// Checks for significant overlap between two colliders, ignoring small overlaps.
        /// </summary>
        /// <param name="collider1">The first collider to check.</param>
        /// <param name="collider2">The second collider to check.</param>
        /// <param name="overlapThreshold">The minimum overlap size to consider.</param>
        /// <returns>True if the overlap is larger than the threshold, otherwise false.</returns>
        public static bool IsSignificantOverlap(Collider2D collider1, Collider2D collider2, float overlapThreshold)
        {
            if (collider1 == null || collider2 == null) return false;

            Bounds bounds1 = collider1.bounds;
            Bounds bounds2 = collider2.bounds;

            // Calculate the overlapping bounds
            Bounds overlapBounds = bounds1;
            overlapBounds.Encapsulate(bounds2);

            float overlapWidth = bounds1.size.x + bounds2.size.x - overlapBounds.size.x;
            float overlapHeight = bounds1.size.y + bounds2.size.y - overlapBounds.size.y;

            // Check if the overlap exceeds the threshold in both width and height
            return overlapWidth > overlapThreshold && overlapHeight > overlapThreshold;
        }

        /// <summary>
        /// Checks if there are any objects overlapping significantly with a given object.
        /// </summary>
        /// <param name="position">The position to check for overlap.</param>
        /// <param name="boxSize">The size of the overlap box.</param>
        /// <param name="layerMask">The layer mask to filter objects.</param>
        /// <param name="overlapThreshold">The minimum overlap size to consider.</param>
        /// <returns>Array of colliders that have significant overlap.</returns>
        public static Collider2D[] GetSignificantOverlaps(Vector2 position, Vector2 boxSize, LayerMask layerMask, float overlapThreshold)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(position, boxSize, 0f, layerMask);
            Collider2D myCollider = Physics2D.OverlapBox(position, boxSize, 0f, layerMask);

            // Filter the colliders to only return those with significant overlap
            if (myCollider != null)
            {
                colliders = System.Array.FindAll(colliders, collider =>
                    IsSignificantOverlap(myCollider, collider, overlapThreshold));
            }

            return colliders;
        }
        /// <summary>
    


    }
}
