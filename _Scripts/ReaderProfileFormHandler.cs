using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReaderProfileFormHandler : MonoBehaviour
{
    [Header("Input fields")]
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private TMP_InputField _emailField;
    [SerializeField] private Toggle _isSuperUserToggle;

    [Header("Message Fields")]
    [SerializeField] private TMP_Text _messageText;

    public void HandleInputData()
    {
        _messageText.text = "";

        if (_nameField.text == null || _nameField.text == "")
        {
            _messageText.text = "Name can't be empty";
            return;
        }

        if (_emailField.text == null || _emailField.text == "")
        {
            _messageText.text = "Email can't be empty";
            return;
        }

        ReaderProfileCreator.Instance.CreateReaderProfile(_nameField.text, _emailField.text, _isSuperUserToggle.isOn);
    }
}
