using UnityEngine;

namespace PaintSystem
{
    public interface IPaintBrush
    {
        float BrushRadius { get; }
        float BrushHardness { get; }
        float BrushStrength { get; }
        Color PaintColor { get; }
        PaintTarget PaintTarget { get; }
    }
}
