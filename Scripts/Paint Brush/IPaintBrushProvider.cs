using System.Collections.Generic;

namespace PaintSystem
{
    public interface IPaintBrushProvider
    {
        IEnumerable<IPaintBrush> Brushes { get; }
    }
}
