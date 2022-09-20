using System;

public class Registration
{
    private event Action Action = delegate { };

    public void Invoke() => Action.Invoke();
    public void RemoveListener(Action listener) => Action -= listener;
    public void AddListener(Action listener)
    {
        Action -= listener;
        Action += listener;
    }
}

public class Registration<T>
{
    private event Action<T> Action = delegate { };

    public void Invoke(T param) => Action.Invoke(param);
    public void RemoveListener(Action<T> listener) => Action -= listener;
    public void AddListener(Action<T> listener)
    {
        Action -= listener;
        Action += listener;
    }
}

public class Registration<T1, T2>
{
    private event Action<T1, T2> Action = delegate { };

    public void Invoke(T1 param1, T2 param2) => Action.Invoke(param1, param2);
    public void RemoveListener(Action<T1, T2> listener) => Action -= listener;
    public void AddListener(Action<T1, T2> listener)
    {
        Action -= listener;
        Action += listener;
    }
}

public class Registration<T1, T2, T3>
{
    private event Action<T1, T2, T3> Action = delegate { };

    public void Invoke(T1 param1, T2 param2, T3 param3) => Action.Invoke(param1, param2, param3);
    public void RemoveListener(Action<T1, T2, T3> listener) => Action -= listener;
    public void AddListener(Action<T1, T2, T3> listener)
    {
        Action -= listener;
        Action += listener;
    }
 }

public class Registration<T1, T2, T3, T4>
{
    private event Action<T1, T2, T3, T4> Action = delegate { };

    public void Invoke(T1 param1, T2 param2, T3 param3, T4 param4) => Action.Invoke(param1, param2, param3, param4);
    public void RemoveListener(Action<T1, T2, T3, T4> listener) => Action -= listener;
    public void AddListener(Action<T1, T2, T3, T4> listener)
    {
        Action -= listener;
        Action += listener;
    }
}

