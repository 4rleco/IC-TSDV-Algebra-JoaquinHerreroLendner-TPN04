using UnityEngine;

public class FrustumVertexCheck : MonoBehaviour
{
    public Camera cam;
    private Plane[] planes;
    private Renderer[] rend;

    void Start()
    {
        rend = GetComponentsInChildren<Renderer>();
    }

    void LateUpdate()
    {
        if (cam == null)
            cam = Camera.main;

        Debug.DrawRay(cam.transform.position, cam.transform.forward * 5, Color.cyan);

        planes = GeometryUtility.CalculateFrustumPlanes(cam);

        // --- PRIMERA ETAPA: Chequeo rápido con AABB ---
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (var r in rend)
            bounds.Encapsulate(r.bounds);

        bool aabbInside = GeometryUtility.TestPlanesAABB(planes, bounds);

        if (!aabbInside)
        {
            // Si la AABB está completamente fuera, ni seguimos
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
            foreach (var r in rend)
            {
                r.enabled = true;
            }
        }
        else
        {
            Debug.Log($"{name}: El objeto está completamente fuera del frustum (aunque su AABB estaba dentro)");
            foreach (var r in rend)
            {
                r.enabled = false;
            }
        }
    }
}