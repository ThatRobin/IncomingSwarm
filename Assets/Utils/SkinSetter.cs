using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class SkinSetter : MonoBehaviour {
    
    public void SetSkin(GameObject nameObject) {
        TMP_InputField input = GameObject.FindGameObjectWithTag("NameInput").GetComponent<TMP_InputField>();

        PlayerPrefs.SetString("username", input.text);
        PlayerPrefs.Save();
        nameObject.SetActive(false);
    }
}
