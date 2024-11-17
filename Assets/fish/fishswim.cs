using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float changeDirectionInterval = 3f;

    [Header("Water Boundaries")]
    [SerializeField] private float waterSurfaceY = 0f;
    [SerializeField] private float waterDepth = 20f;
    [SerializeField] private float minDistanceFromSurface = 1f;
    
    [Header("Swimming Zone")]
    [SerializeField] private Vector3 waterCenter = Vector3.zero;
    [SerializeField] private float waterRadius = 50f;
    [SerializeField] private float islandRadius = 15f;

    private Vector3 targetPosition;
    private float nextDirectionChange;
    private bool needNewTarget = true;

    private void Start()
    {
        // Set initial rotation
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        transform.position = GetRandomWaterPosition();
        SetNewRandomTarget();
    }

    private void Update()
    {
        if (Time.time >= nextDirectionChange || needNewTarget)
        {
            SetNewRandomTarget();
        }

        MoveTowardsTarget();
        RotateTowardsTarget();
        EnforceWaterBoundaries();
    }

    private Vector3 GetRandomWaterPosition()
    {
        int maxAttempts = 30;
        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(islandRadius, waterRadius);
            
            float x = waterCenter.x + distance * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = waterCenter.z + distance * Mathf.Sin(angle * Mathf.Deg2Rad);
            float y = Random.Range(waterSurfaceY - waterDepth, waterSurfaceY - minDistanceFromSurface);

            Vector3 position = new Vector3(x, y, z);
            
            if (IsValidWaterPosition(position))
            {
                return position;
            }
        }

        return new Vector3(
            waterCenter.x + islandRadius * 1.5f, 
            waterSurfaceY - minDistanceFromSurface - 1f,
            waterCenter.z
        );
    }

    private void SetNewRandomTarget()
    {
        targetPosition = GetRandomWaterPosition();
        nextDirectionChange = Time.time + changeDirectionInterval + Random.Range(-1f, 1f);
        needNewTarget = false;
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;

        if (IsValidWaterPosition(newPosition))
        {
            transform.position = newPosition;
        }
        else
        {
            needNewTarget = true;
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        if (direction != Vector3.zero)
        {
            // Calculate the rotation to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            
            // Maintain the -90 degrees X rotation while only rotating in Y and Z
            Vector3 currentEuler = transform.rotation.eulerAngles;
            Vector3 targetEuler = lookRotation.eulerAngles;
            
            // Create final rotation with fixed X and interpolated Y
            Quaternion finalRotation = Quaternion.Euler(
                -90f,  // Keep X at -90 degrees
                targetEuler.y,  // Use target Y rotation
                0f    // Reset Z rotation
            );
            
            // Smoothly rotate to the final rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                finalRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private bool IsValidWaterPosition(Vector3 position)
    {
        if (position.y > waterSurfaceY - minDistanceFromSurface || 
            position.y < waterSurfaceY - waterDepth)
        {
            return false;
        }

        float distanceFromCenter = Vector2.Distance(
            new Vector2(position.x, position.z),
            new Vector2(waterCenter.x, waterCenter.z)
        );

        return distanceFromCenter <= waterRadius && distanceFromCenter >= islandRadius;
    }

    private void EnforceWaterBoundaries()
    {
        Vector3 position = transform.position;
        bool needsCorrection = false;

        if (position.y > waterSurfaceY - minDistanceFromSurface)
        {
            position.y = waterSurfaceY - minDistanceFromSurface;
            needsCorrection = true;
        }
        else if (position.y < waterSurfaceY - waterDepth)
        {
            position.y = waterSurfaceY - waterDepth;
            needsCorrection = true;
        }

        Vector2 positionXZ = new Vector2(position.x - waterCenter.x, position.z - waterCenter.z);
        float distanceFromCenter = positionXZ.magnitude;

        if (distanceFromCenter < islandRadius)
        {
            positionXZ = positionXZ.normalized * islandRadius;
            needsCorrection = true;
        }
        else if (distanceFromCenter > waterRadius)
        {
            positionXZ = positionXZ.normalized * waterRadius;
            needsCorrection = true;
        }

        if (needsCorrection)
        {
            position.x = positionXZ.x + waterCenter.x;
            position.z = positionXZ.y + waterCenter.z;
            transform.position = position;
            needNewTarget = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        DrawCircle(waterCenter, waterRadius, waterSurfaceY);
        DrawCircle(waterCenter, waterRadius, waterSurfaceY - waterDepth);
        
        Gizmos.color = Color.red;
        DrawCircle(waterCenter, islandRadius, waterSurfaceY);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(waterCenter, 1f);
    }

    private void DrawCircle(Vector3 center, float radius, float y)
    {
        int segments = 32;
        Vector3 previousPoint = center + new Vector3(radius, y, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 nextPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                y,
                Mathf.Sin(angle) * radius
            );
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}