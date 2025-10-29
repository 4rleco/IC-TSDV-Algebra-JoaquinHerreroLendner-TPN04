using UnityEngine;

public class FrustumVertexCheck : MonoBehaviour
{
    public Camera cam;
    private Plane[] planes;

    void Update()
    {
        if (cam == null)
            cam = Camera.main; // usa la cámara principal si no se asignó

        planes = GeometryUtility.CalculateFrustumPlanes(cam);

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        Mesh mesh = mf.mesh;
        Transform t = transform;

        Vector3[] vertices = mesh.vertices;
        bool anyInside = false;

        // Recorremos los vértices del mesh
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldV = t.TransformPoint(vertices[i]);
            bool inside = true;

            // Comprobamos cada plano del frustum
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
            Debug.Log("Algún vértice del objeto está dentro del frustum");
        else
            Debug.Log("El objeto está completamente fuera del frustum");
    }
}
