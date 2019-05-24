using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager sSingleton { get { return _sSingleton; } }
    static ShopManager _sSingleton;

    #region Price
    [Header("Price")]
    public int lifePrice = 100;
    public int rechargeRocketPrice = 50;
    public int addRocketsPrice = 50;
    public int upgrShieldPrice = 30;
    public int upgrRechargeRatePrice = 30;
    #endregion

    #region Button
    [Header("Button")]
    [SerializeField] Button m_AddLifeBtn;
    [SerializeField] Button m_RechargeRocketsBtn;
    [SerializeField] Button m_AddMaxRocketsBtn;
    [SerializeField] Button m_UpgrShieldBtn;
    [SerializeField] Button m_UpgrShieldRechargeBtn;
    #endregion

    #region Text
    [Header("Text")]
    [SerializeField] Text m_RechargeTimeText;
    [SerializeField] Text m_InsufficientCoins;
    #endregion

    PlayerController mPlayerController;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        mPlayerController = GameManager.sSingleton.m_PlayerController;

        SetPrice(ref m_AddLifeBtn, lifePrice.ToString());
        SetPrice(ref m_RechargeRocketsBtn, rechargeRocketPrice.ToString());
        SetPrice(ref m_AddMaxRocketsBtn, addRocketsPrice.ToString());
        SetPrice(ref m_UpgrShieldBtn, upgrShieldPrice.ToString());
        SetPrice(ref m_UpgrShieldRechargeBtn, upgrRechargeRatePrice.ToString());
    }

    /// ---------------------------------------------------------------------------------------------
    /// --------------------------------- PUBLIC FUNCTIONS ------------------------------------------
    /// ---------------------------------------------------------------------------------------------

    public void SetupButtons()
    {
        if (mPlayerController.GetLife < GameManager.sSingleton.maxLifes) m_AddLifeBtn.interactable = true;
        else m_AddLifeBtn.interactable = false;

        if (!mPlayerController.IsRocketsFull) m_RechargeRocketsBtn.interactable = true;
        else m_RechargeRocketsBtn.interactable = false;

        if (mPlayerController.GetMaxRockets < GameManager.sSingleton.maxRockets) m_AddMaxRocketsBtn.interactable = true;
        if (mPlayerController.GetMaxShield < GameManager.sSingleton.maxShield) m_UpgrShieldBtn.interactable = true;
        if (mPlayerController.GetShieldRechargeTime > GameManager.sSingleton.minShieldRechargeTime) m_UpgrShieldRechargeBtn.interactable = true;
    }

    public void AddLife()
    {
        if (!IsSufficient(lifePrice)) return;

        mPlayerController.AddLife();
        m_AddLifeBtn.interactable = false;
    }

    public void RechargeRockets()
    {
        if (!IsSufficient(rechargeRocketPrice)) return;

        mPlayerController.RechargeRockets();
        if (mPlayerController.IsRocketsFull) m_RechargeRocketsBtn.interactable = false;
    }

    public void AddMaxRockets()
    {
        if (!IsSufficient(addRocketsPrice)) return;
        mPlayerController.AddMaxRockets();

        float rockets = mPlayerController.GetMaxRockets;
        if (rockets >= GameManager.sSingleton.maxRockets) m_AddMaxRocketsBtn.interactable = false;
    }

    public void UpgradeShield()
    {
        if (!IsSufficient(upgrShieldPrice)) return;
        mPlayerController.UpgradeShield();

        float shield = mPlayerController.GetMaxShield;
        if (shield >= GameManager.sSingleton.maxShield) m_UpgrShieldBtn.interactable = false;
    }

    public void UpgradeShieldRechargeRate()
    {
        if (!IsSufficient(upgrRechargeRatePrice)) return;
        mPlayerController.UpgradeShieldRechargeRate();

        float rechargeDur = mPlayerController.GetShieldRechargeTime;
        m_RechargeTimeText.text = "[" + rechargeDur.ToString() + "s]";

        if (rechargeDur <= GameManager.sSingleton.minShieldRechargeTime) m_UpgrShieldRechargeBtn.interactable = false;
    }

    /// ---------------------------------------------------------------------------------------------
    ///-------------------------------------- PRIVATE FUNCTIONS -------------------------------------
    /// ---------------------------------------------------------------------------------------------

    void SetPrice(ref Button button, string price)
    {
        button.transform.GetChild(button.transform.childCount - 1).GetComponent<Text>().text = price;
    }

    bool IsSufficient(int price)
    {
        if (mPlayerController.GetCoin >= price)
        {
            mPlayerController.RemoveCoin(price);
            return true;
        }
        else
        {
            m_InsufficientCoins.gameObject.SetActive(true);
            return false;
        }
    }
}
