using PaintSystem;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Obsolete]
public class ContactPainter : LayerPainter
{
    private const int REDUCED_MAX_CONTACTS = 1;
    private const int DEFAULT_MAX_CONTACTS = 5;
    private const int AUGMENTED_MAX_CONTACTS = 10;

    [RequireInterface(typeof(IPaintBrushProvider), typeof(ScriptableObject))]
    [SerializeField] private Object _paintBrushProviderObject;
    private IPaintBrushProvider PaintbrushProvider => _paintBrushProviderObject as IPaintBrushProvider; 

    private void OnCollisionStay(Collision collision)
    {
        if ((paintableLayer & (1 << collision.gameObject.layer)) == 0) return;

        Paintable paintable = collision.gameObject.GetComponentInChildren<Paintable>();
        if (paintable == null) return;

        const int MAX_CONTACTS = REDUCED_MAX_CONTACTS;
        var contacts = new ContactPoint[MAX_CONTACTS];
        var count = collision.GetContacts(contacts);

        for (var i = 0; i < count; i++)
        {
            var contact = contacts[i];
            paintManagerObject.Paint(paintable, contact.point, PaintbrushProvider);
        }
    }
}
