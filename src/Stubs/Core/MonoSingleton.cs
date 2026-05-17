public class MonoBehaviour { }

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance
    {
        get { return MonoSingleton<T>._instance; }
    }

    protected virtual void Awake() { }
    protected virtual void OnDestroy() { }

    private static T _instance;
}
