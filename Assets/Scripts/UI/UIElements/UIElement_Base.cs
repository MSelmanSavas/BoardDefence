using UnityEngine;

public abstract class UIElement_Base : MonoBehaviour
{
    public virtual void OnEnable()
    {
        RefBook.Add(this);
        OnEnableInternal();
    }

    protected virtual void OnEnableInternal() { }

    protected void OnDisable()
    {
        RefBook.Remove(this);
        OnDisableInternal();
    }
    protected virtual void OnDisableInternal() { }

    public virtual void Initialize() { }
}
