using PaintSystem;
using UnityEngine;

public static class PaintRequesterExtensions
{
    public static bool TryPaintAtScreenPosition(this PaintRequesterObject paintRequesterObject, Vector2 screenPosition, Camera camera, IPaintBrushProvider paintBrushProvider, float maxDistance, LayerMask layerMask)
    {
        Ray ray = camera.ScreenPointToRay(screenPosition);
        return paintRequesterObject.TryPaintWithRay(ray, paintBrushProvider, maxDistance, layerMask);
    }

    public static bool TryPaintWithRay(this PaintRequesterObject paintRequesterObject, Ray ray, IPaintBrushProvider paintBrushProvider, float maxDistance, LayerMask layerMask)
    {
        if (!Physics.Raycast(ray, out var hit, maxDistance, layerMask)) return false;

        Paintable paintable = hit.collider.gameObject.GetComponentInChildren<Paintable>();
        if (paintable == null) return false;

        paintRequesterObject.Paint(paintable, hit.point, paintBrushProvider);
        return true;
    }

    public static bool TryPaintAtContact(this PaintRequesterObject paintRequesterObject, Collision collision, IPaintBrushProvider paintBrushProvider, LayerMask layerMask)
    {
        if ((layerMask & (1 << collision.gameObject.layer)) == 0) return false;

        Paintable paintable = collision.gameObject.GetComponentInChildren<Paintable>();
        if (paintable == null) return false;

        ContactPoint contact = collision.GetContact(0);
        paintRequesterObject.Paint(paintable, contact.point, paintBrushProvider);
        return true;
    }
}