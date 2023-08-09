using PaintSystem;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Paint Settings Collection Object", menuName = "Scriptable Objects/Paint Settings Collection Object")]
public class PaintBrushCollectionObject : ScriptableObject, IPaintBrushProvider
{
    [field: SerializeField] public PaintBrushObject[] PaintBrushes { get; private set; } = new PaintBrushObject[0];
    [field: SerializeField] public bool UseRandomBrush { get; private set; } = false;
    public PaintBrushObject GetRandomSettings() => PaintBrushes.Length == 0 ? null : PaintBrushes[Random.Range(0, PaintBrushes.Length)];
    [field: SerializeField] public int SelectedPaintBrushesIndex { get; set; } = 0;
    public IEnumerable<IPaintBrush> Brushes => UseRandomBrush ? GetRandomSettings().Brushes : PaintBrushes[SelectedPaintBrushesIndex].Brushes;
}
