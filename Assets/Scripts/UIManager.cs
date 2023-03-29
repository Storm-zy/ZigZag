using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;



public class UIManager : MonoBehaviour
{

    public static UIManager instance;


    public GameObject StartingPanel;
    public GameObject GameOverPanel;
    public GameObject TapText;
    public GameObject Score1;
    public GameObject highScoreView;
    public TMP_Text Score;
    public TMP_Text HighScore1;
    public TMP_Text HighScore2;
    public Button seeHighScore;

    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

    }

    
    // Start is called before the first frame update
    void Start()
    {

        HighScore1.text = "High Score: " + PlayerPrefs.GetInt("highscore");
        highScoreView.SetActive(false);
        seeHighScore.onClick.AddListener(viewHighScore);
    }

    public void GameStart() 
    {
        Score1.SetActive(true);
        TapText.SetActive(false);
        StartingPanel.SetActive(false);
        highScoreView.SetActive(false);
    }

    public void GameOver()
    {
        Score1.SetActive(false);
        Score.text = PlayerPrefs.GetInt("score").ToString();
        HighScore2.text = PlayerPrefs.GetInt("highscore").ToString();
        GameOverPanel.SetActive(true);
        highScoreView.SetActive(true);
    }

    public void Reset()
    {
        
        SceneManager.LoadScene (0);
        
    }

    public void viewHighScore(){
         SceneManager.LoadScene (1);
    }

    // Update is called once per frame
    void Update()
    {

    }




}
