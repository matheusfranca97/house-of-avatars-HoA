using UnityEngine;

public abstract class DataDrivenBehaviour<T> : MonoBehaviour
{
    private readonly EventVariable<DataDrivenBehaviour<T>, T> _data;
    public T data
    {
        set { _data.value = value; }
        get { return _data.eventStackValue; }
    }

    protected DataDrivenBehaviour()
    {
        _data = new EventVariable<DataDrivenBehaviour<T>, T>(this, default(T));
    }

    protected virtual void OnEnable()
    {
        _data.onValueChangeImmediate += OnValueChanged_Data;
    }

    protected virtual void OnDisable()
    {
        _data.onValueChangeImmediate -= OnValueChanged_Data;
    }

    protected abstract void OnValueChanged_Data(T oldValue, T newValue);
}