using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int score;
    public int highscore;
    public TMP_Text score1;


    

    void Awake()
    {
    if(instance == null)
    {
        instance = this;
        print("highscore");
    }
    }
    // Start is called before the first frame update
    void Start()
    {
        score = 0;  
        highscore = 0; 
        PlayerPrefs.SetInt("score", score);
    }

        void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Diamond")
        {
            score += 1;
            
        }
    }
    


    public void IncrementScore()
    {
        score += 1;
        score1.text = score.ToString();
        

    }

        // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetInt("score", score);
    }

    public void StartScore()
    {
        InvokeRepeating("IncrementScore", 0.1f, 0.5f);
    }

    public void StopScore()
    {

        CancelInvoke("IncrementScore");

        PlayerPrefs.SetInt("score", score); 
        

        if(PlayerPrefs.HasKey("highscore"))
        {
            if(score > PlayerPrefs.GetInt("highscore"))
            {
                PlayerPrefs.SetInt("highscore", score);
                
            }
        }
        else
        {
            PlayerPrefs.SetInt("highscore", highscore);
        }
    }
}
