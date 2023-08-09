using PaintSystem;
using UnityEngine;

public abstract class Painter : MonoBehaviour
{
    [SerializeField] protected PaintRequesterObject paintManagerObject;

    protected virtual void Awake()
    {
        paintManagerObject.Initialize();
    }
}
