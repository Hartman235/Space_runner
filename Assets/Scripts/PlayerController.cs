using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 dir;
    private Score score;
    [SerializeField] private float speed;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private int coins;
    [SerializeField] private Text coinsText;
    [SerializeField] private Score scoreScript;

    private int lineToMove = 1;
    public float lineDistance = 3;
    public float verticalDistance = 3; // Расстояние для вертикального перемещения
    private int verticalLine = 0; // Переменная для контроля вертикального перемещения (0 или 1)
    private float initialYPosition; // Начальная высота персонажа
    private float maxSpeed = 120;
    private bool isCoinCollectingEnabled = true; // Флаг, который управляет возможностью сбора монет
    private bool isImmortal;

    void Start()
    {
        isImmortal = false;
        score = scoreText.GetComponent<Score>();
        Time.timeScale = 1;
        coinsText.text = "Coins : 0";
        score.scoreMultiplier = 1;
        controller = GetComponent<CharacterController>();
        initialYPosition = transform.position.y; // Сохраняем начальную высоту
        coins = PlayerPrefs.GetInt("coins");
        coinsText.text = "Coins : " + coins.ToString();
        StartCoroutine(SpeedIncrease());
    }

    private void Update()
    {
        // Перемещение по горизонтали
        if (SwipeController.swipeRight)
        {
            if (lineToMove < 2)
            {
                lineToMove++;
            }
        }

        if (SwipeController.swipeLeft)
        {
            if (lineToMove > 0)
            {
                lineToMove--;
            }
        }

        // Перемещение вверх
        if (SwipeController.swipeUp)
        {
            if (verticalLine < 1) // Позволяет перемещаться только на одну линию вверх
            {
                verticalLine++;
            }
        }

        // Перемещение вниз
        if (SwipeController.swipeDown)
        {
            if (verticalLine > 0) // Позволяет перемещаться только на одну линию вниз
            {
                verticalLine--;
            }
        }

        // Определяем целевую позицию
        Vector3 targetPosition = transform.position.z * transform.forward;

        if (lineToMove == 0)
        {
            targetPosition += Vector3.left * lineDistance;
        }
        else if (lineToMove == 2)
        {
            targetPosition += Vector3.right * lineDistance;
        }

        // Добавляем вертикальное смещение
        targetPosition.y = initialYPosition + verticalLine * verticalDistance;

        // Обновляем позицию
        //transform.position = targetPosition;
        if(transform.position == targetPosition){
            return;
        }
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if(moveDir.sqrMagnitude < diff.sqrMagnitude){
            controller.Move(moveDir);
        } else{
            controller.Move(diff);
        }
    }

    void FixedUpdate()
    {
        // Перемещение вперед по оси z
        dir.z = speed;
        controller.Move(dir * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) 
{
    if (hit.gameObject.tag == "obstacle")
    {
        if(isImmortal)
        {
            Destroy(hit.gameObject);
        }
        else
        {
            int lastRunScore;
            string scoreText = scoreScript.scoreText.text.Split(':')[1].Trim();
            bool isParsed = int.TryParse(scoreText, out lastRunScore);

            if (isParsed)
            {
                PlayerPrefs.SetInt("lastRunScore", lastRunScore);

                // Проверяем и обновляем рекорд
                int recordScore = PlayerPrefs.GetInt("recordScore");
                if (lastRunScore > recordScore)
                {
                    recordScore = lastRunScore;
                    PlayerPrefs.SetInt("recordScore", recordScore);
                }
        }
        else
        {
            Debug.LogError("Ошибка: не удалось преобразовать значение результата. Текущее значение: " + scoreScript.scoreText.text);
        }

        // Останавливаем время и показываем панель поражения
        losePanel.SetActive(true);
        Time.timeScale = 0;
        }
        
    }
}


private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("coin") && isCoinCollectingEnabled)
    {
        Destroy(other.gameObject); // Уничтожаем монету
        coins++; // Увеличиваем счетчик монет
        PlayerPrefs.SetInt("coins", coins);
        coinsText.text = "Coins : " + coins.ToString();
        //scoreText.text = "Score : " + ((int)(player.position.z / 2)).ToString();
        StartCoroutine(DisableCoinCollectingTemporarily()); // Запускаем корутину
    }

    if (other.CompareTag("BonusStar") && isCoinCollectingEnabled)
    
    {
        Destroy(other.gameObject);
        score.scoreMultiplier = 2;
        StartCoroutine(StarBonus());
        StartCoroutine(DisableCoinCollectingTemporarily());
    }

    if (other.CompareTag("BonusShield") && isCoinCollectingEnabled)
    {
        Destroy(other.gameObject);
        StartCoroutine(ShieldBonus());
        StartCoroutine(DisableCoinCollectingTemporarily());
    }
}

private IEnumerator DisableCoinCollectingTemporarily()
{
    isCoinCollectingEnabled = false; // Отключаем возможность сбора монет
    yield return new WaitForSeconds(0.1f); // Время задержки перед повторным включением
    isCoinCollectingEnabled = true; // Включаем возможность сбора монет
}

    private IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(3);
        if(speed < maxSpeed){
            speed += 1;
            StartCoroutine(SpeedIncrease());
        }
        
    }

    private IEnumerator StarBonus()
    {
        score.scoreMultiplier = 2;

        yield return new WaitForSeconds(10);

        score.scoreMultiplier = 1;
    }

    private IEnumerator ShieldBonus()
    {
        isImmortal = true;

        yield return new WaitForSeconds(10);

        isImmortal = false;
    }
}
