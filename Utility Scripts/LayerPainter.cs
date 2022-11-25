using UnityEngine;

public abstract class LayerPainter : Painter
{
    [SerializeField] protected LayerMask paintableLayer;
}
