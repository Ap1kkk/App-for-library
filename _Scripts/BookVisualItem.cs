using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BookVisualItem : MonoBehaviour
{
    [Header("Displaying fields")]
    [SerializeField] private TMP_Text _idField;
    [SerializeField] private TMP_Text _titleField;
    [SerializeField] private TMP_Text _authorField;
    [SerializeField] private TMP_Text _genreField;

    private Book _book;
    private Button _button;

    public void SetBookData(Book book)
    {
        _book = book;
        _idField.text = book.BookId.ToString();
        _titleField.text = book.Title;
        _authorField.text = book.Author;
        _genreField.text = book.Genre;
    }

    public void InitializeButton()
    {
        _button = GetComponent<Button>();
        //_button.onClick.AddListener(SelectBook);
    }

    public void SelectBook()
    {
        GlobalEvents.Instance.OnBookSelected?.Invoke(_book);
        Debug.Log($"book selection with id: {_book.BookId}");
    }

    public void EnableSelection() 
    {
        _button.interactable = true;
    }
    public void DisableSelection() 
    {
        _button.interactable = false;
    }
}
