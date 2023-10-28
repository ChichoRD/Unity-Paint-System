using System;
using UnityEngine;

[Obsolete]
public abstract class LayerPainter : Painter
{
    [SerializeField] protected LayerMask paintableLayer;
}
