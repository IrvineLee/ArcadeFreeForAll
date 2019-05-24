using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager sSingleton { get { return _sSingleton; } }
    static HighScoreManager _sSingleton;

    [SerializeField] int m_HiScoreMaxNames = 5, m_HiScoreMaxDigit = 7;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        //SaveHighScore();
    }

    /// ------------------------------------------------------------------------------------
    /// --------------------------------- PUBLIC FUNCTIONS ---------------------------------
    /// ------------------------------------------------------------------------------------

    // Main menu highscore load.
    public void LoadHighScore(Transform hiScoreParent)
    {
        for (int i = 0; i < m_HiScoreMaxNames; i++)
        {
            Transform currTrans = hiScoreParent.GetChild(i);
            Text nameText = currTrans.GetChild(1).GetComponent<Text>();
            Text scoreText = currTrans.GetChild(2).GetComponent<Text>();

            string name = PlayerPrefs.GetString("Name_" + i);
            string score = PlayerPrefs.GetString("HiScore_" + i);

            if (int.Parse(score) == 0)
            {
                name = "Person_" + (i + 1);
                score = ((m_HiScoreMaxNames - i + 1) * 200).ToString();
            }

            nameText.text = name;
            scoreText.text = GetScoreWithZero(score);
        }
    }

    // Load high score and push it down based on current score. Return the rank.
    public int LoadHighScore(Transform hiScoreParent, int currScore)
    {
        List<int> scoreList = new List<int>();

        // Load everything accordingly.
        for (int i = 0; i < m_HiScoreMaxNames; i++)
        {
            Transform currTrans = hiScoreParent.GetChild(i);
            Text nameText = currTrans.GetChild(1).GetComponent<Text>();
            Text scoreText = currTrans.GetChild(2).GetComponent<Text>();

            string name = PlayerPrefs.GetString("Name_" + i);
            string score = PlayerPrefs.GetString("HiScore_" + i);

            nameText.text = name;
            scoreText.text = GetScoreWithZero(score);

            scoreList.Add(int.Parse(score));
        }

        // Get the current rank of the player.
        int rank = 0;
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (currScore > scoreList[i])
            {
                rank = i + 1;
                break;
            }
        }

        // If the player is within ranking...
        if (rank != 0)
        {
            // Push the high score down.
            for (int i = m_HiScoreMaxNames - 2; i >= rank - 1; i--)
            {
                Transform currTrans = hiScoreParent.GetChild(i);
                Text currNameText = currTrans.GetChild(1).GetComponent<Text>();
                Text currScoreText = currTrans.GetChild(2).GetComponent<Text>();

                Transform nextTrans = hiScoreParent.GetChild(i + 1);
                Text nextNameText = nextTrans.GetChild(1).GetComponent<Text>();
                Text nextScoreText = nextTrans.GetChild(2).GetComponent<Text>();

                nextNameText.text = currNameText.text;
                nextScoreText.text = currScoreText.text;
            }

            // Player name and score.
            Transform trans = hiScoreParent.GetChild(rank - 1);
            Text nameText = trans.GetChild(1).GetComponent<Text>();
            Text scoreText = trans.GetChild(2).GetComponent<Text>();

            nameText.text = "[----------------]";
            scoreText.text = GetScoreWithZero(currScore.ToString());
        }

        return rank;
    }

    // Save all the name and score in hiScoreParent.
    public void SaveHighScore(Transform hiScoreParent)
    {
        for (int i = 0; i < m_HiScoreMaxNames; i++)
        {
            Transform currTrans = hiScoreParent.GetChild(i);
            string currName = currTrans.GetChild(1).GetComponent<Text>().text;
            string currScore = currTrans.GetChild(2).GetComponent<Text>().text;

            PlayerPrefs.SetString("Name_" + i, currName);
            PlayerPrefs.SetString("HiScore_" + i, currScore);
        }
        PlayerPrefs.Save();
    }

    /// ------------------------------------------------------------------------------------
    /// -------------------------------- PRIVATE FUNCTIONS ---------------------------------
    /// ------------------------------------------------------------------------------------

    // Main menu highscore save. Inital test.
    //void SaveHighScore(Transform hiScoreParent)
    //{
    //    for (int i = 0; i < m_HiScoreMaxNames; i++)
    //    {
    //        Transform currTrans = hiScoreParent.GetChild(i);
    //        string currName = currTrans.GetChild(1).GetComponent<Text>().text;
    //        string currScore = currTrans.GetChild(2).GetComponent<Text>().text;

    //        PlayerPrefs.SetString("Name_" + i, currName);
    //        PlayerPrefs.SetString("HiScore_" + i, currScore);
    //    }
    //}

    string GetScoreWithZero(string score)
    {
        int addZero = m_HiScoreMaxDigit - score.Length;

        for (int i = 0; i < addZero; i++)
        { score = "0" + score; }

        return score;
    }
}
