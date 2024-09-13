using UnityEngine;

[ExecuteInEditMode]
public class ShowBones : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        DrawBones(transform);
    }

    private void DrawBones(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Gizmos.color = Color.green; // You can change the color as you like
            Gizmos.DrawSphere(child.position, 0.05f); // Sphere at bone position

            if (parent != transform)
            {
                Gizmos.color = Color.red; // You can change the color as you like
                Gizmos.DrawLine(parent.position, child.position); // Line connecting parent to child
            }

            DrawBones(child); // Recursively draw bones for child transforms
        }
    }
}