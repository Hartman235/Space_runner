using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    [Header("Asteroid Settings")]
    public GameObject[] asteroidPrefabs;  // Массив префабов астероидов
    public int rows = 2;  // Количество строк
    public int columns = 3;  // Количество колонок
    public float xSpacing = 2.0f;  // Расстояние между астероидами по оси X
    public float ySpacing = 2.0f;  // Расстояние между астероидами по оси Y
    public float spawnDistance = 10f;  // Расстояние перед кораблём, где будут генерироваться астероиды
    public Transform player;  // Ссылка на объект игрока
    public float respawnThreshold = 20f;  // Расстояние, при достижении которого генерируются новые астероиды
    public int maxAsteroids = 10;  // Максимальное количество астероидов на сцене
    public float spawnInterval = 5f;  // Время между генерациями астероидов
    public float offScreenThreshold = 15f;  // Расстояние за игроком, при котором астероид будет удален

    private Vector3[] spawnPositions;  // Все возможные позиции для астероидов
    private float spawnTimer;
    private int currentAsteroids;
    public List<GameObject> activeAsteroids = new List<GameObject>();  // Список активных астероидов

    void Start()
    {
        if (player == null)
        {
            return;
        }

        CalculateSpawnPositions();  // Вычисление всех возможных позиций спавна
        spawnTimer = 0f;  // Инициализация таймера
        currentAsteroids = 0;
        SpawnAsteroids();  // Изначальный спавн астероидов
    }

    void Update()
    {
        if (player == null)
            return;

        // Обновление таймера
        spawnTimer += Time.deltaTime;

        // Генерация астероидов, когда игрок проходит определённое расстояние,
        // количество астероидов меньше максимального, и прошло достаточно времени
        if (Vector3.Distance(player.position, transform.position) >= respawnThreshold &&
            currentAsteroids < maxAsteroids &&
            spawnTimer >= spawnInterval)
        {
            SpawnAsteroids();
            spawnTimer = 0f;  // Сброс таймера после генерации
        }

        // Удаление астероидов, которые находятся за игроком
        RemoveOffScreenAsteroids();
    }

    void CalculateSpawnPositions()
    {
        // Вычисляем начальную позицию спавна относительно игрока
        Vector3 startPos = player.position + player.forward * spawnDistance;

        // Определяем возможные позиции спавна астероидов относительно этой начальной позиции
        spawnPositions = new Vector3[rows * columns];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Расчёт позиции для каждого астероида, с фиксированными X и Y
                Vector3 position = startPos + new Vector3(col * xSpacing - (columns - 1) * xSpacing / 2, row * ySpacing, 0);
                spawnPositions[row * columns + col] = position;
            }
        }
    }

    void SpawnAsteroids()
    {
        // Определяем случайное количество астероидов для генерации (от 1 до 5)
        int numAsteroidsToSpawn = Random.Range(1, 6);

        // Выбираем случайный ряд и колонку, где не будет астероида, чтобы оставить путь для корабля
        int emptyIndex = Random.Range(0, spawnPositions.Length);

        int spawnedAsteroids = 0;

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            if (spawnedAsteroids >= numAsteroidsToSpawn)
                break;  // Прекращаем генерацию, если достигли нужного количества астероидов

            if (i == emptyIndex)
                continue;  // Пропускаем генерацию на выбранной позиции

            if (currentAsteroids >= maxAsteroids)
                return;  // Проверка лимита астероидов

            // Рассчитываем позицию для текущего астероида с учётом оси Z
            Vector3 basePosition = spawnPositions[i];
            Vector3 spawnPosition = basePosition + new Vector3(0, 0, player.position.z + spawnDistance);

            // Рандомно выбираем префаб астероида из массива
            GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];

            // Создаем астероид на сцене
            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
            activeAsteroids.Add(asteroid);  // Добавляем астероид в список активных
            currentAsteroids++;

            spawnedAsteroids++;  // Увеличиваем счётчик созданных астероидов
        }
    }

    void RemoveOffScreenAsteroids()
    {
        // Используем временный список для хранения астероидов, которые должны быть удалены
        List<GameObject> asteroidsToRemove = new List<GameObject>();

        foreach (GameObject asteroid in activeAsteroids)
        {
            if (asteroid != null)
            {
                // Проверяем, находится ли астероид за игроком на определённом расстоянии
                if (asteroid.transform.position.z < player.position.z - offScreenThreshold)
                {
                    asteroidsToRemove.Add(asteroid);
                }
            }
        }

        // Удаляем астероиды и обновляем список активных астероидов
        foreach (GameObject asteroid in asteroidsToRemove)
        {
            activeAsteroids.Remove(asteroid);
            Destroy(asteroid);
            currentAsteroids--;
        }
    }
}
