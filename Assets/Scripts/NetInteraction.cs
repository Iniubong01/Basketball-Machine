using UnityEngine;

public class NetInteraction : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private float forceMultiplier = 10f;  // Intensity of deformation
    [SerializeField] private float influenceRadius = 2f;   // Radius of deformation
    [SerializeField] private float shakeDuration = 0.5f;   // Duration of the shake
    [SerializeField] private float shakeFrequency = 10f;   // Frequency of the shake

    private Mesh netMesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;
    private bool isShaking = false;

    private void Start()
    {
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("Skinned Mesh Renderer is not assigned!");
            return;
        }

        netMesh = skinnedMeshRenderer.sharedMesh;
        netMesh = Instantiate(netMesh);
        skinnedMeshRenderer.sharedMesh = netMesh;

        if (netMesh == null)
        {
            Debug.LogError("Net Mesh is missing or invalid!");
            return;
        }

        originalVertices = netMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        System.Array.Copy(originalVertices, displacedVertices, originalVertices.Length);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball") && !isShaking)
        {
            Vector3 collisionPoint = other.contacts[0].point;
            Vector3 localCollisionPoint = skinnedMeshRenderer.transform.InverseTransformPoint(collisionPoint);

            StartCoroutine(ShakeNet(localCollisionPoint));
        }
    }

    private System.Collections.IEnumerator ShakeNet(Vector3 collisionPoint)
    {
        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            for (int i = 0; i < displacedVertices.Length; i++)
            {
                float distance = Vector3.Distance(originalVertices[i], collisionPoint);

                if (distance < influenceRadius)
                {
                    float offset = Mathf.Sin(elapsedTime * shakeFrequency) * (forceMultiplier / Mathf.Max(distance, 0.1f));
                    Vector3 direction = (originalVertices[i] - collisionPoint).normalized;
                    displacedVertices[i] = originalVertices[i] + direction * offset;
                }
                else
                {
                    displacedVertices[i] = originalVertices[i]; // Reset unaffected vertices
                }
            }

            netMesh.vertices = displacedVertices;
            netMesh.RecalculateNormals();
            netMesh.RecalculateBounds();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the vertices to their original positions
        System.Array.Copy(originalVertices, displacedVertices, originalVertices.Length);
        netMesh.vertices = displacedVertices;
        netMesh.RecalculateNormals();
        netMesh.RecalculateBounds();

        isShaking = false;
    }

    private void OnDrawGizmos()
    {
        if (skinnedMeshRenderer != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, influenceRadius);
        }
    }
}
