using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager sSingleton { get { return _sSingleton; } }
    static GameManager _sSingleton;

    #region StartValues
    [Header("Start Values")]
    public int startLifes = 3;
    public int startHealth = 40;
    public int startRockets = 3;
    public int startShield = 40;

    public float startShieldRechargeTime = 10;
    public float shieldRechargeDuration = 1;
    public float roundTime = 60;
    #endregion

    #region MaxValues
    [Header("Max Values")]
    public int maxLifes = 3;
    public int maxRockets = 10;
    public int maxShield = 100;
    public int minShieldRechargeTime = 2;
    #endregion

    #region Scene
    [Header("Scene")]
    public Transform startPosTrans;
    public PlayerController playerController;
    #endregion

    public enum State
    {
        BATTLE = 0,
        PAUSE,
        TRANSITION,
        SHOP,
        HISCORE,
        GAME_OVER
    }
    
    #region Scene
    [Header("Current")]
    public State currState = State.BATTLE;
    public int currRound = 1;
    public int coinToScore = 1000;
    #endregion

    public class Border
    {
        public float top, bottom, left, right;

        public Border()
        {
            this.top = 0;
            this.bottom = 0;
            this.left = 0;
            this.right = 0;
        }

        public bool IsZero()
        {
            if (top == 0 && bottom == 0 && left == 0 && right == 0) return true;
            return false;
        }
    }

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        // This gets called after transitioning from MainMenu scene to InGame scene.
        GameManager.sSingleton.currState = GameManager.State.TRANSITION;
        UIManager.sSingleton.RoundStart();
    }

    public bool IsBattle()
    {
        if (currState == State.BATTLE) return true;
        return false;
    }
}
