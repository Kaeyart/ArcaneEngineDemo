using UnityEngine;
using UnityEngine.InputSystem;

namespace ArcaneEngine
{
    /// <summary>
    /// Single-purpose hardware mouse aim resolver. It deliberately does not use the
    /// IMGUI event cache, UI pointer fallbacks, sensitivity multipliers, viewport
    /// clamping, smoothing, retained dead zones, or movement input.
    /// </summary>
    [DefaultExecutionOrder(800)]
    public sealed class HardwareMouseAim : MonoBehaviour
    {
        public const float CombatPlaneHeight = 0.82f;

        public bool HasAim { get; private set; }
        public Vector2 ScreenPointer { get; private set; }
        public Vector3 WorldPoint { get; private set; }
        public Vector3 WorldDirection { get; private set; } = Vector3.forward;

        private void LateUpdate()
        {
            HasAim = false;
            Camera camera = Camera.main;
            Vector2 gamepadAim = ArcaneInput.GamepadAim;
            if (camera != null && gamepadAim.sqrMagnitude > 0.001f)
            {
                Vector3 forward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
                Vector3 right = Vector3.ProjectOnPlane(camera.transform.right, Vector3.up).normalized;
                Vector3 direction = right * gamepadAim.x + forward * gamepadAim.y;
                if (direction.sqrMagnitude > 0.001f)
                {
                    float response = ProfileManager.Current == null ? 1f : ProfileManager.Current.controls.controllerAimSensitivity;
                    float maxRadians = Mathf.Lerp(4f, 18f, Mathf.InverseLerp(0.25f, 2.5f, response)) * Time.unscaledDeltaTime;
                    WorldDirection = Vector3.RotateTowards(WorldDirection, direction.normalized, maxRadians, 0f).normalized;
                    WorldPoint = new Vector3(transform.position.x, CombatPlaneHeight, transform.position.z) + WorldDirection * 12f;
                    ScreenPointer = camera.WorldToScreenPoint(WorldPoint);
                    HasAim = true;
                    return;
                }
            }
            Mouse mouse = Mouse.current;
            if (camera == null || mouse == null) return;

            Vector2 pointer = mouse.position.ReadValue();
            if (!IsFinite(pointer.x) || !IsFinite(pointer.y)) return;

            Rect pixels = camera.pixelRect;
            if (pixels.width <= 1f || pixels.height <= 1f) return;

            Ray ray = camera.ScreenPointToRay(new Vector3(pointer.x, pointer.y, 0f));
            Plane plane = new Plane(Vector3.up, new Vector3(0f, CombatPlaneHeight, 0f));
            float enter;
            if (!plane.Raycast(ray, out enter) || enter <= 0f || !IsFinite(enter)) return;

            Vector3 point = ray.GetPoint(enter);
            point.y = CombatPlaneHeight;
            Vector3 origin = new Vector3(transform.position.x, CombatPlaneHeight, transform.position.z);
            Vector3 aimDirection = point - origin;
            aimDirection.y = 0f;
            if (aimDirection.sqrMagnitude <= 0.0001f) return;

            ScreenPointer = pointer;
            WorldPoint = point;
            WorldDirection = aimDirection.normalized;
            HasAim = true;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
