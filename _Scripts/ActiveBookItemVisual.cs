using TMPro;
using UnityEngine;

public class ActiveBookItemVisual : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] private TMP_Text _titleField;
    [SerializeField] private TMP_Text _authorField;
    [SerializeField] private TMP_Text _genreField;
    [SerializeField] private TMP_Text _dateField;

    private Book _book;
    private BookListItem _bookListItem;

    public void SetBookData(BookListItem bookListItem)
    {
        _book = BookCreator.Instance.GetBookById(bookListItem.BookId);
        _bookListItem = bookListItem;

        _titleField.text = _book.Title;
        _authorField.text = _book.Author;
        _genreField.text = _book.Genre;

        _dateField.text = bookListItem.DeadlineTimeStr;
    }

}
