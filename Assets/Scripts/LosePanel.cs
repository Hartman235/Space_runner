using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LosePanel : MonoBehaviour
{
    [SerializeField] Text recordText;

    private void OnEnable() // Используем OnEnable, чтобы обновить текст при активации панели
    {
        UpdateRecordText(); // Обновляем текст при активации панели
    }

    public void UpdateRecordText()
    {
        int recordScore = PlayerPrefs.GetInt("recordScore");
        recordText.text = "Record : " + recordScore.ToString();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}