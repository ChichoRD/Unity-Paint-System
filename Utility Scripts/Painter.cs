using PaintSystem;
using UnityEngine;

public abstract class Painter : MonoBehaviour
{
    [SerializeField] protected PaintManagerObject paintManagerObject;

    protected virtual void Awake()
    {
        paintManagerObject.Initialize();
    }
}
