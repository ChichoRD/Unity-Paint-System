using PaintSystem;
using System;
using UnityEngine;

[Obsolete]
public abstract class Painter : MonoBehaviour
{
    [SerializeField] protected PaintRequesterObject paintManagerObject;

    protected virtual void Awake()
    {
        paintManagerObject.Initialize();
    }
}
