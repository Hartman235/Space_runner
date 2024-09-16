using System.Collections.Generic;
using UnityEngine;

public class ShieldBonusManager : MonoBehaviour
{
    [Header("Shield Bonus Settings")]
    public GameObject shieldBonusPrefab;  // Префаб бонуса щита
    public int rows = 2;  // Количество строк
    public int columns = 3;  // Количество колонок
    public float xSpacing = 2.0f;  // Расстояние между клетками по оси X
    public float ySpacing = 2.0f;  // Расстояние между клетками по оси Y
    public float spawnDistance = 10f;  // Расстояние перед кораблём, где будет генерироваться бонус щита
    public Transform player;  // Ссылка на объект игрока
    public float respawnThreshold = 20f;  // Расстояние, при достижении которого генерируется бонус щита
    public float spawnInterval = 15f;  // Время между генерациями бонуса щита
    public float offScreenThreshold = 15f;  // Расстояние за игроком, при котором бонус будет удален

    private Vector3[] spawnPositions;  // Все возможные позиции для генерации бонуса
    private float spawnTimer;
    private GameObject activeShieldBonus;  // Активный бонус щита

    void Start()
    {
        if (player == null)
        {
            return;
        }

        CalculateSpawnPositions();  // Вычисление всех возможных позиций спавна
        spawnTimer = 0f;  // Инициализация таймера
    }

    void Update()
    {
        if (player == null)
            return;

        // Обновление таймера
        spawnTimer += Time.deltaTime;

        // Генерация бонуса щита, если его нет на сцене, прошло достаточно времени и игрок прошел респаун-зону
        if (activeShieldBonus == null && spawnTimer >= spawnInterval && Vector3.Distance(player.position, transform.position) >= respawnThreshold)
        {
            SpawnShieldBonus();
            spawnTimer = 0f;  // Сброс таймера после генерации
        }

        // Удаление бонуса щита, если он находится за игроком
        RemoveOffScreenShieldBonus();
    }

    void CalculateSpawnPositions()
    {
        // Вычисляем начальную позицию спавна относительно игрока
        Vector3 startPos = player.position + player.forward * spawnDistance;

        // Определяем возможные позиции спавна бонусов (аналогично астероидам)
        spawnPositions = new Vector3[rows * columns];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Расчёт позиции для каждой клетки
                Vector3 position = startPos + new Vector3(col * xSpacing - (columns - 1) * xSpacing / 2, row * ySpacing, 0);
                spawnPositions[row * columns + col] = position;
            }
        }
    }

    void SpawnShieldBonus()
    {
        // Выбираем случайную клетку для бонуса щита
        int randomIndex = Random.Range(0, spawnPositions.Length);

        // Рассчитываем позицию для бонуса щита с учётом оси Z и положения игрока
        Vector3 basePosition = spawnPositions[randomIndex];
        Vector3 spawnPosition = basePosition + new Vector3(0, 0, player.position.z + spawnDistance);

        // Создаем бонус щита на сцене
        activeShieldBonus = Instantiate(shieldBonusPrefab, spawnPosition, Quaternion.identity);
    }

    void RemoveOffScreenShieldBonus()
    {
        if (activeShieldBonus != null)
        {
            // Проверяем, находится ли бонус щита за игроком на определённом расстоянии
            if (activeShieldBonus.transform.position.z < player.position.z - offScreenThreshold)
            {
                Destroy(activeShieldBonus);
                activeShieldBonus = null;  // Сбрасываем ссылку на бонус щита
            }
        }
    }
}
