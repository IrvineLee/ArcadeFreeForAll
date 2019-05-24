using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager sSingleton { get { return _sSingleton; } }
    static MainMenuManager _sSingleton;

    [SerializeField] Transform m_MainMenu, m_HighScoreMenu, m_HighScoreParent;

    bool mIsHiScore = false;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    // Start in game scene.
    public void StartGame()
    {
        SceneManager.LoadScene("InGameScene");
    }

    // Toggle high score.
    public void ToggleHiScore()
    {
        if (!mIsHiScore)
        {
            m_HighScoreMenu.gameObject.SetActive(true);
            m_MainMenu.gameObject.SetActive(false);
            HighScoreManager.sSingleton.LoadHighScore(m_HighScoreParent);
            mIsHiScore = true;
        }
        else
        {
            m_HighScoreMenu.gameObject.SetActive(false);
            m_MainMenu.gameObject.SetActive(true);
            mIsHiScore = false;
        }
    }
}
