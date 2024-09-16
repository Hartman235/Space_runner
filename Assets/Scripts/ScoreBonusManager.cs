using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBonusManager : MonoBehaviour
{
    [Header("Bonus Settings")]
    public GameObject bonusPrefab;  // Префаб бонусной звезды
    public int rows = 2;  // Количество строк
    public int columns = 3;  // Количество колонок
    public float xSpacing = 2.0f;  // Расстояние между клетками по оси X
    public float ySpacing = 2.0f;  // Расстояние между клетками по оси Y
    public float spawnDistance = 10f;  // Расстояние перед кораблём, где будет генерироваться бонус
    public Transform player;  // Ссылка на объект игрока
    public float respawnThreshold = 20f;  // Расстояние, при достижении которого генерируется бонус
    public float spawnInterval = 10f;  // Время между генерациями бонуса
    public float offScreenThreshold = 15f;  // Расстояние за игроком, при котором бонус будет удален

    private Vector3[] spawnPositions;  // Все возможные позиции для генерации бонуса
    private float spawnTimer;
    private GameObject activeBonus;  // Активная бонусная звезда

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

        // Генерация бонуса, если его нет на сцене, прошло достаточно времени и игрок прошел респаун-зону
        if (activeBonus == null && spawnTimer >= spawnInterval && Vector3.Distance(player.position, transform.position) >= respawnThreshold)
        {
            SpawnBonus();
            spawnTimer = 0f;  // Сброс таймера после генерации
        }

        // Удаление бонуса, если он находится за игроком
        RemoveOffScreenBonus();
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

    void SpawnBonus()
    {
        // Выбираем случайную клетку для бонуса
        int randomIndex = Random.Range(0, spawnPositions.Length);

        // Рассчитываем позицию для бонуса с учётом оси Z и положения игрока
        Vector3 basePosition = spawnPositions[randomIndex];
        Vector3 spawnPosition = basePosition + new Vector3(0, 0, player.position.z + spawnDistance);

        // Создаем бонусную звезду на сцене
        activeBonus = Instantiate(bonusPrefab, spawnPosition, Quaternion.identity);
    }

    void RemoveOffScreenBonus()
    {
        if (activeBonus != null)
        {
            // Проверяем, находится ли бонус за игроком на определённом расстоянии
            if (activeBonus.transform.position.z < player.position.z - offScreenThreshold)
            {
                Destroy(activeBonus);
                activeBonus = null;  // Сбрасываем ссылку на бонус
            }
        }
    }
}
