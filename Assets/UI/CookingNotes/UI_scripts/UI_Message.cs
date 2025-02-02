using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_Message : MonoBehaviour
{
    [SerializeField] private Image _imageMessage;
    [SerializeField] private TextMeshProUGUI _textMessage;

    IEnumerator Message(String message, int messageTime)
    {
        this._textMessage.text = message;
        this._imageMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(messageTime);
        this._imageMessage.gameObject.SetActive(false);
    }

    public void SpawnMessage(String s, int t)
    {
        StartCoroutine(Message(s, t));
    }
}
