using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[ExecuteInEditMode]
public class DrawFrustumGizmo : MonoBehaviour
{
    [Header("Frustum Settings")]
    public float fov = 60f;
    public float aspect = 1.7777f; // 16:9
    public float near = 0.3f;
    public float far = 10f;
    public Color color = Color.cyan;

    [Header("Target To Check")]
    public Transform target;

    public Color frustumColor = Color.cyan;
    public Color insideColor = Color.green;
    public Color outsideColor = Color.red;

    void OnDrawGizmos()
    {
        DrawFrustrum();
    }

    void DrawFrustrum()
    {
        Gizmos.color = color;

        // Calcula alturas y anchuras de los planos
        float nearHeight = 2f * Mathf.Tan(Mathf.Deg2Rad * fov * 0.5f) * near;
        float nearWidth = nearHeight * aspect;
        float farHeight = 2f * Mathf.Tan(Mathf.Deg2Rad * fov * 0.5f) * far;
        float farWidth = farHeight * aspect;

        // Define los vértices en espacio local
        Vector3[] nearCorners = new Vector3[4];
        Vector3[] farCorners = new Vector3[4];

        // Near plane
        nearCorners[0] = new Vector3(-nearWidth / 2, -nearHeight / 2, near);
        nearCorners[1] = new Vector3(nearWidth / 2, -nearHeight / 2, near);
        nearCorners[2] = new Vector3(nearWidth / 2, nearHeight / 2, near);
        nearCorners[3] = new Vector3(-nearWidth / 2, nearHeight / 2, near);

        // Far plane
        farCorners[0] = new Vector3(-farWidth / 2, -farHeight / 2, far);
        farCorners[1] = new Vector3(farWidth / 2, -farHeight / 2, far);
        farCorners[2] = new Vector3(farWidth / 2, farHeight / 2, far);
        farCorners[3] = new Vector3(-farWidth / 2, farHeight / 2, far);

        // Aplica la transformación del objeto
        Matrix4x4 m = transform.localToWorldMatrix;

        // Dibuja los bordes
        for (int i = 0; i < 4; i++)
        {
            // Near rectangle
            Gizmos.DrawLine(m.MultiplyPoint(nearCorners[i]), m.MultiplyPoint(nearCorners[(i + 1) % 4]));
            // Far rectangle
            Gizmos.DrawLine(m.MultiplyPoint(farCorners[i]), m.MultiplyPoint(farCorners[(i + 1) % 4]));
            // Conexiones entre near y far
            Gizmos.DrawLine(m.MultiplyPoint(nearCorners[i]), m.MultiplyPoint(farCorners[i]));
        }
    }
}