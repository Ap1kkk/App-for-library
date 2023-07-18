using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;

public class BookFormHandler : MonoBehaviour
{
    [Header("Input fields")]
    [SerializeField] private TMP_InputField _titleField;
    [SerializeField] private TMP_InputField _authorField;
    [SerializeField] private TMP_InputField _genreField;

    [Header("Message Fields")]
    [SerializeField] private TMP_Text _messageText;

    public void HandleInputData()
    {
        _messageText.text = "";

        if (_titleField.text == null || _titleField.text == "")
        {
            _messageText.text = "Title can't be empty";
            return;
        }

        if (_authorField.text == null || _authorField.text == "")
        {
            _messageText.text = "Author can't be empty";
            return;
        }

        if (_genreField.text == null || _genreField.text == "")
        {
            _messageText.text = "Genre can't be empty";
            return;
        }

        BookCreator.Instance.CreateBook(_titleField.text, _authorField.text , _genreField.text);
    }
}
