using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipLoader : MonoBehaviour
{
    public Transform player; // объект, в котором находятся все префабы кораблей
    private int chosenIndex;

    void Start()
    {
        // Загружаем индекс выбранного корабля из PlayerPrefs
        chosenIndex = PlayerPrefs.GetInt("chosenSkin", 0); // по умолчанию загружается корабль 0

        // Активируем выбранный корабль
        for (int i = 0; i < player.childCount; i++)
        {
            if (i == chosenIndex)
            {
                player.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                player.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
