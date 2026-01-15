using UnityEngine;

namespace ArrowGame.Hit
{
    public class HitZoneGizmos : MonoBehaviour
    {
        [SerializeField] private HitZoneConfig config;

        private void OnDrawGizmos()
        {
            if (config == null) return;
            if (Needle.NeedleController.Instance == null) return;

            Vector3 pos = Needle.NeedleController.Instance.transform.position;

            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            DrawCircle(pos, config.outerRadius);

            Gizmos.color = new Color(0f, 0.8f, 1f, 0.5f);
            DrawCircle(pos, config.middleRadius);

            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            DrawCircle(pos, config.innerRadius);

            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            DrawCircle(pos, config.coreRadius);
        }

        private void DrawCircle(Vector3 center, float radius)
        {
            int segments = 64;
            float angleStep = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * angleStep * Mathf.Deg2Rad;
                float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

                Vector3 p1 = center + new Vector3(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius, 0);
                Vector3 p2 = center + new Vector3(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius, 0);

                Gizmos.DrawLine(p1, p2);
            }
        }
    }
}