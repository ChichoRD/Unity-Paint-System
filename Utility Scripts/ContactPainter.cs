using PaintSystem;
using UnityEngine;

public class ContactPainter : LayerPainter
{
    [SerializeField] private PaintSettingsObject _paintSettingsObject;

    private void OnCollisionStay(Collision collision)
    {
        if ((paintableLayer & (1 << collision.gameObject.layer)) == 0) return;

        Paintable paintable = collision.gameObject.GetComponentInChildren<Paintable>();
        if (paintable == null) return;

        const int MAX_CONTACTS = 10;
        var contacts = new ContactPoint[MAX_CONTACTS];
        var count = collision.GetContacts(contacts);

        for (var i = 0; i < count; i++)
        {
            var contact = contacts[i];
            paintManagerObject.Paint(paintable, contact.point, _paintSettingsObject);
        }
    }
}
