using UnityEngine;
using PaintSystem;

public class CursorPainter : LayerPainter
{
    [SerializeField] private PaintSettingsObject _paintSettingsObject;
    [field: SerializeField] public Camera Camera { get; set; }

    public bool TryPaintAtScreenPosition(Vector2 position, out Paintable paintable)
    {
        paintable = null;
        if (Camera == null) return false;
        
        var ray = Camera.ScreenPointToRay(position);
        if (!Physics.Raycast(ray, out var hit, float.MaxValue, paintableLayer)) return false;
        
        paintable = hit.collider.transform.root.gameObject.GetComponentInChildren<Paintable>();
        if (paintable == null) return false;
        
        paintManagerObject.Paint(paintable, hit.point, _paintSettingsObject);
        return true;
    }
}
