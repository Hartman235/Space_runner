using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Если уже существует экземпляр, который не должен уничтожаться, уничтожаем текущий
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            // Назначаем текущий объект как instance и предотвращаем его уничтожение
            instance = this;
            DontDestroyOnLoad(gameObject); // Не уничтожать при загрузке новой сцены
        }
    }
}
