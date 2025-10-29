using UnityEngine;
using UnityEngine.UIElements;

public class AABB : MonoBehaviour
{
    private Vector3 localCenter;
    private Vector3 localExtents;

    void Start()
    {
        // Calcular una sola vez en local space
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Bounds localBounds = mesh.bounds;

        localCenter = localBounds.center;
        localExtents = localBounds.extents;
    }

    void OnDrawGizmos()
    {
        if (GetComponent<MeshFilter>() == null) return;

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null) return;

        if (localExtents == Vector3.zero)
        {
            Bounds localBounds = mesh.bounds;
            localCenter = localBounds.center;
            localExtents = localBounds.extents;
        }

        // Matriz de transformación actual
        Matrix4x4 localToWorld = transform.localToWorldMatrix;

        // Centro en mundo
        Vector3 worldCenter = localToWorld.MultiplyPoint3x4(localCenter);

        // Extraemos ejes
        Vector3 right = new Vector3(localToWorld.m00, localToWorld.m01, localToWorld.m02);
        Vector3 up = new Vector3(localToWorld.m10, localToWorld.m11, localToWorld.m12);
        Vector3 forward = new Vector3(localToWorld.m20, localToWorld.m21, localToWorld.m22);

        // Valor absoluto
        right = new Vector3(Mathf.Abs(right.x), Mathf.Abs(right.y), Mathf.Abs(right.z));
        up = new Vector3(Mathf.Abs(up.x), Mathf.Abs(up.y), Mathf.Abs(up.z));
        forward = new Vector3(Mathf.Abs(forward.x), Mathf.Abs(forward.y), Mathf.Abs(forward.z));

        // Extents en mundo
        Vector3 worldExtents =
            right * localExtents.x +
            up * localExtents.y +
            forward * localExtents.z;

        // Bounds en mundo
        Bounds worldBounds = new Bounds(worldCenter, worldExtents * 2f);

        // Dibujar
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
    }
}
