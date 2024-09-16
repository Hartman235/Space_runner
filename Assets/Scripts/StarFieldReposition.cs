using UnityEngine;

public class StarFieldReposition : MonoBehaviour
{
    public Transform player; // Ссылка на игрока
    public float fieldRadius = 50f; // Радиус зоны, в которой располагаются звезды
    public float repositionThreshold = 60f; // Радиус от игрока, при котором звезда перемещается

    void Update()
    {
        foreach (Transform star in transform) // Перебираем все звезды в звёздном поле
        {
            // Если звезда находится слишком далеко от игрока
            if (Vector3.Distance(player.position, star.position) > repositionThreshold)
            {
                // Перемещаем звезду в новую случайную позицию перед игроком
                Vector3 newPos = player.position + Random.onUnitSphere * fieldRadius;
                star.position = newPos;
            }
        }
    }
}
