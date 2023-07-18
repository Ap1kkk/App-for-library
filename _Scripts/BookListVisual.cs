using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BookListVisual : MonoBehaviour, IInitializable
{
    [SerializeField] private GameObject _listItemPrefab;
    [SerializeField] private Transform _containerTransform;

    [SerializeField] private Dictionary<uint, BookVisualItem> _freeBooks = new Dictionary<uint, BookVisualItem>();

    private DatabaseReference _databaseReference;

    public void Initialize()
    {
        _freeBooks.Clear();

        GlobalEvents.Instance.OnActiveBooksRecognized += OnActiveBooksRecognized;

        GlobalEvents.Instance.OnBookGotByReader += OnBookTakenByReader;
        GlobalEvents.Instance.OnBookSelected += OnBookSelected;
        GlobalEvents.Instance.OnBookDeselected += OnBookDeselected;
    }

    public void Deinitialize()
    {
        GlobalEvents.Instance.OnActiveBooksRecognized -= OnActiveBooksRecognized;
     
        GlobalEvents.Instance.OnBookGotByReader -= OnBookTakenByReader;
        GlobalEvents.Instance.OnBookSelected -= OnBookSelected;
        GlobalEvents.Instance.OnBookDeselected -= OnBookDeselected;
    }

    public void OnBookSelected(Book book)
    {
        UpdateDisplayingBooks();
        DisableSelection();
    }

    public void OnBookDeselected(Book book)
    {
        UpdateDisplayingBooks();
        EnableSelection();
    }

    public void OnActiveBooksRecognized(List<uint> bookIds)
    {
        UpdateDisplayingBooks();
    }

    private void DisableSelection()
    {
        foreach(BookVisualItem bookVisualItem in _freeBooks.Values)
        {
            bookVisualItem.DisableSelection();
        }
    }

    private void EnableSelection()
    {
        foreach (BookVisualItem bookVisualItem in _freeBooks.Values)
        {
            bookVisualItem.EnableSelection();
        }
    }

    public void OnBookTakenByReader(Book book)
    {
        if(_freeBooks.ContainsKey(book.BookId))
        {
            Destroy(_freeBooks[book.BookId].gameObject);
            _freeBooks.Remove(book.BookId);
            Debug.Log($"Book with id: {book.BookId} successfully removed from displaying container");
        }
        else
        {
            Debug.LogError($"Can't remove Book with id: {book.BookId}\nThere's no such book in displaying container");
        }
    }

    public void UpdateDisplayingBooks()
    {
        var books = BookCreator.Instance.GetAllInactiveBooks();
        ClearVisualItems();
        foreach (Book book in books)
        {
            UpdateVisualItem(book);
        }
    }

    private void UpdateVisualItem(Book book)
    {
        var item = Instantiate(_listItemPrefab, _containerTransform.transform);
        BookVisualItem visualItem = item.GetComponent<BookVisualItem>();

        _freeBooks.Add(book.BookId, visualItem);

        visualItem.SetBookData(book);
        visualItem.InitializeButton();

        item.SetActive(true);
    }

    private void ClearVisualItems()
    {
        foreach(BookVisualItem bookVisualItem in _freeBooks.Values)
        {
            Destroy(bookVisualItem.gameObject);
        }
        _freeBooks.Clear();
    }
}
