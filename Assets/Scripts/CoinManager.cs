using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("Coin Settings")]
    public GameObject coinPrefab;  // Префаб монеты
    public int rows = 2;  // Количество строк
    public int columns = 3;  // Количество колонок
    public float xSpacing = 2.0f;  // Расстояние между позициями по оси X
    public float ySpacing = 2.0f;  // Расстояние между позициями по оси Y
    public float spawnDistance = 10f;  // Расстояние перед кораблём, где будет генерироваться монета

    public Transform player;  // Ссылка на объект игрока
    public float respawnThreshold = 20f;  // Расстояние, при достижении которого генерируется новая монета
    public float spawnInterval = 5f;  // Время между генерациями монет
    public float offScreenThreshold = 15f;  // Расстояние за игроком, при котором монета будет удалена
    public float collisionCheckRadius = 1.0f;  // Радиус проверки коллизий

    private Vector3[] spawnPositions;  // Все возможные позиции для монет
    private float spawnTimer;
    private GameObject activeCoin;  // Активная монета

    void Start()
    {
        if (player == null)
        {
            return;
        }

        CalculateSpawnPositions();  // Вычисление всех возможных позиций спавна
        spawnTimer = 0f;  // Инициализация таймера
        SpawnCoin();  // Изначальный спавн монеты
    }

    void Update()
    {
        if (player == null)
            return;

        // Обновление таймера
        spawnTimer += Time.deltaTime;

        // Генерация монеты, когда игрок проходит определённое расстояние и прошло достаточно времени
        if (Vector3.Distance(player.position, transform.position) >= respawnThreshold &&
            activeCoin == null &&  // Если на сцене нет активной монеты
            spawnTimer >= spawnInterval)
        {
            SpawnCoin();
            spawnTimer = 0f;  // Сброс таймера после генерации
        }

        // Удаление монеты, которая находится за игроком
        RemoveOffScreenCoin();
    }

    void CalculateSpawnPositions()
    {
        // Вычисляем начальную позицию спавна относительно игрока
        Vector3 startPos = player.position + player.forward * spawnDistance;

        // Определяем возможные позиции спавна монет относительно этой начальной позиции
        spawnPositions = new Vector3[rows * columns];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Расчёт позиции для каждой монеты, с фиксированными X и Y
                Vector3 position = startPos + new Vector3(col * xSpacing - (columns - 1) * xSpacing / 2, row * ySpacing, 0);
                spawnPositions[row * columns + col] = position;
            }
        }
    }

    void SpawnCoin()
{
    for (int attempt = 0; attempt < spawnPositions.Length; attempt++)
    {
        int spawnIndex = Random.Range(0, spawnPositions.Length);
        Vector3 basePosition = spawnPositions[spawnIndex];
        Vector3 spawnPosition = basePosition + new Vector3(0, 0, player.position.z + spawnDistance);

        if (!IsCollidingWithAsteroid(spawnPosition))
        {
            // Установка правильного вращения для монеты
            Quaternion coinRotation = Quaternion.Euler(0, 0, 0);  // Или другой нужный угол
            activeCoin = Instantiate(coinPrefab, spawnPosition, coinRotation);
            return;
        }
    }
}

    bool IsCollidingWithAsteroid(Vector3 position)
    {
        // Проверяем, находится ли в указанной позиции коллайдер астероида
        Collider[] hitColliders = Physics.OverlapSphere(position, collisionCheckRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("obstacle"))  // Убедитесь, что астероиды имеют тег "Asteroid"
            {
                return true;  // Если астероид слишком близко, возвращаем true
            }
        }
        return false;  // Если поблизости нет астероидов, возвращаем false
    }

    void RemoveOffScreenCoin()
    {
        if (activeCoin != null)
        {
            // Проверяем, находится ли монета за игроком на определённом расстоянии
            if (activeCoin.transform.position.z < player.position.z - offScreenThreshold)
            {
                Destroy(activeCoin);
                activeCoin = null;  // Удаляем ссылку на активную монету
            }
        }
    }
}
