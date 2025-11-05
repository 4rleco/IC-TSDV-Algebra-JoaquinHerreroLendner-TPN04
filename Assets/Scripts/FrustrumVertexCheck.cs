using UnityEngine;

public class FrustumVertexCheck : MonoBehaviour
{
    public Camera cam;
    private Plane[] planes;
    private Renderer rend;

    private Color outsideColor = Color.red;
    private Color insideColor = Color.green;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void LateUpdate()
    {
        if (cam == null)
            cam = Camera.main;

        Debug.DrawRay(cam.transform.position, cam.transform.forward * 5, Color.cyan);

        planes = GeometryUtility.CalculateFrustumPlanes(cam);

        // --- PRIMERA ETAPA: Chequeo rápido con AABB ---
        Bounds bounds = rend.bounds; // bounds ya están en espacio mundo

        bool aabbInside = GeometryUtility.TestPlanesAABB(planes, bounds);

        if (!aabbInside)
        {
            // Si la AABB está completamente fuera, ni seguimos
            rend.material.color = outsideColor;
            return;
        }

        // --- SEGUNDA ETAPA: Chequeo detallado con vértices ---
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        Mesh mesh = mf.sharedMesh;
        Transform t = transform;

        Vector3[] vertices = mesh.vertices;
        bool anyInside = false;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldV = t.TransformPoint(vertices[i]);
            bool inside = true;

            foreach (var p in planes)
            {
                if (p.GetDistanceToPoint(worldV) < 0)
                {
                    inside = false;
                    break;
                }
            }

            if (inside)
            {
                anyInside = true;
                break;
            }
        }

        if (anyInside)
        {
            Debug.Log($"{name}: Algún vértice del objeto está dentro del frustum");
            rend.material.color = insideColor;
        }
        else
        {
            Debug.Log($"{name}: El objeto está completamente fuera del frustum (aunque su AABB estaba dentro)");
            rend.material.color = outsideColor;
        }
    }
}