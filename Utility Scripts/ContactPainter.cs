using PaintSystem;
using UnityEngine;

public class ContactPainter : LayerPainter
{
    private const int REDUCED_MAX_CONTACTS = 1;
    private const int DEFAULT_MAX_CONTACTS = 5;
    private const int AUGMENTED_MAX_CONTACTS = 10;

    [SerializeField] private PaintSettingsCollectionObject _paintSettingsCollectionObject;

    private void OnCollisionStay(Collision collision)
    {
        if ((paintableLayer & (1 << collision.gameObject.layer)) == 0) return;

        Paintable paintable = collision.gameObject.GetComponentInChildren<Paintable>();
        if (paintable == null) return;

        //Vector3 contact = collision.contacts[0].point;
        //paintManagerObject.Paint(paintable, contact, _paintSettingsCollectionObject);

        //TODO - Introduce tiny cooldown to prevent multiple paint calls per frame
        const int MAX_CONTACTS = REDUCED_MAX_CONTACTS;
        var contacts = new ContactPoint[MAX_CONTACTS];
        var count = collision.GetContacts(contacts);

        for (var i = 0; i < count; i++)
        {
            var contact = contacts[i];
            paintManagerObject.Paint(paintable, contact.point, _paintSettingsCollectionObject);
        }
    }
}
