using UnityEngine;
using PaintSystem;
using System;
using Object = UnityEngine.Object;

[Obsolete]
public class CursorPainter : LayerPainter // TODO - Improve these sample scripts
{
    [RequireInterface(typeof(IPaintBrushProvider))]
    [SerializeField] private Object _paintSettingsCollectionObject;
    private IPaintBrushProvider PaintBrushProvider => _paintSettingsCollectionObject as IPaintBrushProvider;
    [field: SerializeField] public Camera Camera { get; set; }

    [Obsolete]
    private void Update()
    {
        TryPaintAtScreenPosition(Camera.ViewportToScreenPoint(Vector3.one * 0.5f), out _);
    }

    public bool TryPaintAtScreenPosition(Vector2 position, out Paintable paintable)
    {
        paintable = null;
        if (Camera == null) return false;
        
        var ray = Camera.ScreenPointToRay(position);
        if (!Physics.Raycast(ray, out var hit, float.MaxValue, paintableLayer)) return false;
        
        paintable = hit.collider.transform.root.gameObject.GetComponentInChildren<Paintable>();
        if (paintable == null) return false;
        
        paintManagerObject.Paint(paintable, hit.point, PaintBrushProvider);
        return true;
    }
}
