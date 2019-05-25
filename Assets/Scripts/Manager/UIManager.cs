using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager sSingleton { get { return _sSingleton; } }
    static UIManager _sSingleton;

    #region PlayerUI
    [Header("Player UI")]
    [SerializeField] Transform m_LifeParent;
    [SerializeField] Transform m_HealthParent;
    [SerializeField] Transform m_ShieldParent;
    [SerializeField] Transform m_RocketParent;
    #endregion

    #region PageUI
    [Header("Page UI")]
    [SerializeField] Transform m_PauseParent;
    [SerializeField] Transform m_ShopParent;
    [SerializeField] Transform m_HighScoreMenu;
    [SerializeField] Transform m_HighScoreParent;
    [SerializeField] Transform m_RoundStart;
    [SerializeField] Transform m_RoundOver;
    [SerializeField] Transform m_GameOver;
    #endregion

    #region Current
    [Header("Current")]
    [SerializeField] Text m_RoundStartText;
    [SerializeField] Text m_CurrRoundText;
    [SerializeField] Text m_CurrScoreText;
    [SerializeField] Text m_CurrCoinText;
    [SerializeField] Text m_CurrKillText;
    [SerializeField] CountdownTimer m_RoundTimerCD;
    #endregion

    #region HighScore
    [Header("HighScore")]
    [SerializeField] TMPro.TMP_InputField m_InputNameField;
    [SerializeField] Button m_EndButton;
    #endregion

    int mHighScoreRank;
    PlayerController mPlayerController;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        UI_Initialize();
        mPlayerController = GameManager.sSingleton.playerController;
    }

    void Update()
    {
        if (!GameManager.sSingleton.IsBattle()) return;

        // Pause menu.
        if (Input.GetKeyDown(KeyCode.Escape)) PauseUnPauseMenu();
    }

    /// ------------------------------------------------------------------------------------
    /// --------------------------------- PUBLIC FUNCTIONS ---------------------------------
    /// ------------------------------------------------------------------------------------
    
    // Update the life and rockets UI.
    public void UpdateLife(int toVal)    { UpdateImageUI(ref m_LifeParent, toVal); }
    public void UpdateRockets(int toVal) { UpdateImageUI(ref m_RocketParent, toVal); }

    // Update the health and shield UI.
    public void UpdateHealth(int toVal)  { UpdateBarUI(ref m_HealthParent, toVal); }
    public void UpdateShield(int toVal)  { UpdateBarUI(ref m_ShieldParent, toVal); }

    // Update score UI.
    public void UpdateScore(int toVal)   { m_CurrScoreText.text = toVal.ToString(); }
    public void UpdateCoins(int toVal)   { m_CurrCoinText.text = toVal.ToString(); }
    public void UpdateKills(int toVal)   { m_CurrKillText.text = toVal.ToString(); }

    // Pause and unpause menu. Public for button event press.
    public void PauseUnPauseMenu()
    {
        if (GameManager.sSingleton.currState == GameManager.State.PAUSE)
        {
            m_PauseParent.gameObject.SetActive(false);
            GameManager.sSingleton.currState = GameManager.State.BATTLE;
        }
        else
        {
            m_PauseParent.gameObject.SetActive(true);
            GameManager.sSingleton.currState = GameManager.State.PAUSE;
        }
    }

    // Quit back to title screen. Public for button event press.
    public void QuitToTitleScreen() { SceneManager.LoadScene("TitleScreen"); }
    
    // Round start.
    public void RoundStart()
    {
        m_RoundStart.gameObject.SetActive(true);
        m_ShopParent.gameObject.SetActive(false);

        // Display new round to player.
        int currRound = GameManager.sSingleton.currRound;
        m_RoundStartText.text = "Round " + currRound + " Start !!";

        // Update string to new round on the sidebar.
        string str = "";
        if (currRound < 10) str = "Round 0" + currRound.ToString();
        else if (currRound < 100) str = "Round " + currRound.ToString();
        m_CurrRoundText.text = str;

        GameManager.sSingleton.currState = GameManager.State.TRANSITION;
        m_RoundTimerCD.Initialize();

        StartCoroutine(WaitThenDo(2, StartBattle));
    }

    // Round end after timer finished.
    public void RoundEnd()
    {
        m_RoundOver.gameObject.SetActive(true);
        GameManager.sSingleton.currState = GameManager.State.TRANSITION;
        ShopManager.sSingleton.SetupButtons();
        EnemyManager.sSingleton.ReduceSpawnCD();

        StartCoroutine(WaitThenDo(2, SwitchToShop));
    }

    // Game Over.
    public void ShowGameOver()
    {
        m_GameOver.gameObject.SetActive(true);
        StartCoroutine(WaitThenDo(2f, ShowHighScoreMenu));
    }

    // Add the name to high score.
    public void AddNameToHighScore(string name)
    {
        Transform currTrans = m_HighScoreParent.GetChild(mHighScoreRank - 1);
        Text nameText = currTrans.GetChild(1).GetComponent<Text>();
        nameText.text = name;

        HighScoreManager.sSingleton.SaveHighScore(m_HighScoreParent);

        StartCoroutine(WaitThenDo(2, QuitToTitleScreen));
    }

    /// ------------------------------------------------------------------------------------
    /// -------------------------------- PRIVATE FUNCTIONS ---------------------------------
    /// ------------------------------------------------------------------------------------

    // Enable the child gameobject (which has an image on them).
    void UpdateImageUI(ref Transform trans, int toVal)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject currChild = trans.GetChild(i).gameObject;
            if (i < toVal) currChild.SetActive(true);
            else currChild.SetActive(false);
        }
    }

    // Fill the bar according to the value (0 ~ 1).
    void UpdateBarUI(ref Transform trans, int toVal)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            trans.GetChild(i).GetComponent<Image>().fillAmount = toVal * 0.01f;
        }
    }

    void StartBattle()
    {
        m_RoundStart.gameObject.SetActive(false);
        EnemyManager.sSingleton.NextRound();
        GameManager.sSingleton.currState = GameManager.State.BATTLE;
    }

    void SwitchToShop()
    {
        m_RoundOver.gameObject.SetActive(false);
        m_ShopParent.gameObject.SetActive(true);

        // Restore the health and shield of player.
        mPlayerController.RestoreHealthAndShield();
        UpdateHealth(GameManager.sSingleton.startHealth);
        UpdateShield(mPlayerController.GetMaxShield);

        // Reset the scene back to default.
        EnemyManager.sSingleton.DisableAllEnemies();
        BulletManager.sSingleton.DisableAllBullets();
        EnvironmentalObjManager.sSingleton.DisableAllPickUps();
        mPlayerController.ResetPosition();

        GameManager.sSingleton.currState = GameManager.State.SHOP;
    }

    void ShowHighScoreMenu()
    {
        m_GameOver.gameObject.SetActive(false);

        // Load the data into the high score UI
        if (HighScoreManager.sSingleton != null)
        {
            int currScore = mPlayerController.GetScore;
            currScore += mPlayerController.GetCoin * GameManager.sSingleton.coinToScore;
            mHighScoreRank = HighScoreManager.sSingleton.LoadHighScore(m_HighScoreParent, currScore);
        }

        GameManager.sSingleton.currState = GameManager.State.HISCORE;

        if (mHighScoreRank == 0)
        {
            m_InputNameField.gameObject.SetActive(false);
            m_EndButton.gameObject.SetActive(true);
        }
        else
        {
            m_InputNameField.gameObject.SetActive(true);
            m_EndButton.gameObject.SetActive(false);
        }
        m_HighScoreMenu.gameObject.SetActive(true);
    }

    // Initialize the player HUD.
    void UI_Initialize()
    {
        // Set the starting life UI.
        for (int i = 0; i < GameManager.sSingleton.startLifes; i++)
        {
            m_LifeParent.GetChild(i).gameObject.SetActive(true);
        }

        // Set the starting health UI.
        Image[] imgArray = m_HealthParent.GetComponentsInChildren<Image>();
        for (int i = 0; i < imgArray.Length; i++)
        {
            imgArray[i].fillAmount = GameManager.sSingleton.startHealth * 0.01f;
        }

        // Set the starting shield UI.
        imgArray = m_ShieldParent.GetComponentsInChildren<Image>();
        for (int i = 0; i < imgArray.Length; i++)
        {
            imgArray[i].fillAmount = GameManager.sSingleton.startShield * 0.01f;
        }

        // Set the starting rockets UI.
        for (int i = 0; i < GameManager.sSingleton.startRockets; i++)
        {
            m_RocketParent.GetChild(i).gameObject.SetActive(true);
        }
    }

    IEnumerator WaitThenDo(float waitTime, Action act)
    {
        yield return new WaitForSeconds(waitTime);
        act();
    }
}
