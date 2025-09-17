using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public string RequiredTag;
    public UnityEvent<Collider2D> triggerEvent;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag(RequiredTag))
        {
            triggerEvent?.Invoke(collision);
        }
    }
}
