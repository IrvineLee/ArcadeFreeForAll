using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveHiScoreName : MonoBehaviour
{
    [SerializeField] Transform m_SavedTrans;
    TMPro.TMP_InputField mInputField;

    bool mIsPressed = false;

    void Start()
    {
        mInputField = GetComponent<TMPro.TMP_InputField>();
    }

    void Update()
    {
        if (mIsPressed)
        {
            UIManager.sSingleton.AddNameToHighScore(mInputField.text);
            mInputField.text = "";
            mInputField.gameObject.SetActive(false);
            m_SavedTrans.gameObject.SetActive(true);
            mIsPressed = false;
        }
    }

    void OnGUI()
    {
        if (mInputField.isFocused && mInputField.text != "" && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            mIsPressed = true;
        }
    }
}
