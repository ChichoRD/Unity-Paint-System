using PaintSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Paint Settings Collection Object", menuName = "Scriptable Objects/Paint Settings Collection Object")]
public class PaintSettingsCollectionObject : ScriptableObject
{
    [field: SerializeField] public PaintSettingsObject[] PaintSettings { get; private set; } = new PaintSettingsObject[0];
    [field: SerializeField] public bool UseRandomRotation { get; private set; } = false;
    [field: SerializeField] public bool UseRandomTexture { get; private set; } = false;
    [field: SerializeField] public bool UseRandomSettings { get; private set; } = false;
    public Vector3 GetRandomRotation() => Random.insideUnitSphere * Mathf.PI;
    public Texture2D GetRandomTextureFromSettings() => PaintSettings.Length == 0 ? null : PaintSettings[Random.Range(0, PaintSettings.Length)].PaintTexture;
    public PaintSettingsObject GetRandomSettings() => PaintSettings.Length == 0 ? null : PaintSettings[Random.Range(0, PaintSettings.Length)];
    public int SelectedPaintSettingsIndex { get; set; } = 0;
}
