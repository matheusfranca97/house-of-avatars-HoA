
using UnityEngine;
using UnityEngine.UI;

public abstract class DataDrivenSelectable<T> : Selectable
{
    private readonly EventVariable<DataDrivenSelectable<T>, T> _data;

    public Canvas canvas => GetComponentInParent<Canvas>();
    public RectTransform rectTransform => transform as RectTransform;

    public T data
    {
        set { _data.value = value; }
        get { return _data.eventStackValue; }
    }

    protected DataDrivenSelectable()
    {
        _data = new EventVariable<DataDrivenSelectable<T>, T>(this, default(T));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _data.onValueChangeImmediate += OnValueChanged_Data;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _data.onValueChangeImmediate -= OnValueChanged_Data;
    }

    protected abstract void OnValueChanged_Data(T oldValue, T newValue);
}