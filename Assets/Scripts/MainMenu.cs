using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Text coinsText;
    // Start is called before the first frame update
    private void Start()
    {
        int coins = PlayerPrefs.GetInt("coins");
        coinsText.text = "Coins : " + coins.ToString();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Ships()
    {
        SceneManager.LoadScene(2);
    }
}
