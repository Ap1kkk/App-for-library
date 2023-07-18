using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ReaderProfileVisual : MonoBehaviour, IInitializable
{
    [Header("Text Fields")]
    [SerializeField] private TMP_Text _usernameField;
    [SerializeField] private TMP_Text _emailField;
    [SerializeField] private TMP_Text _idField;
    [SerializeField] private TMP_Text _ratingField;

    [Header("Prefab")]
    [SerializeField] private GameObject _activeBookItemPrefab;
    [SerializeField] private Transform _containerTransform;

    [SerializeField] private ReaderProfile _readerProfile;
    [SerializeField] private Dictionary<uint, ActiveBookItemVisual> _bookListItems = new Dictionary<uint, ActiveBookItemVisual>();

    public void Initialize()
    {
        _bookListItems.Clear();
        GlobalEvents.Instance.OnReaderProfileLoggedIn += DisplayReaderProfileData;
        GlobalEvents.Instance.OnBookGotByReader += UpdateDisplayingActiveBooks;
    }

    public void Deinitialize()
    {
        GlobalEvents.Instance.OnReaderProfileLoggedIn -= DisplayReaderProfileData;
        GlobalEvents.Instance.OnBookGotByReader -= UpdateDisplayingActiveBooks;
    }

    public void DisplayReaderProfileData(ReaderProfile readerProfile)
    {
        _readerProfile = readerProfile;
        _usernameField.text = _readerProfile.ReaderName;
        _emailField.text = _readerProfile.ReaderEmail;
        _idField.text = _readerProfile.ReaderId.ToString();
        _ratingField.text = _readerProfile.ReaderRating.ToString();

        DisplayActiveBoooks();
    }

    public void ClearAllDisplayingBooks()
    {
        foreach(var bookListItem in _bookListItems.Values)
        {
            Destroy(bookListItem.gameObject);
        }
        _bookListItems.Clear();
    }

    public void DisplayActiveBoooks()
    {
        ClearAllDisplayingBooks();
        foreach(var bookListItem in _readerProfile.ActiveBooksDict.Values)
        {
            var activeBookItem = Instantiate(_activeBookItemPrefab, _containerTransform);
            var activeBookItemVisual = activeBookItem.GetComponent<ActiveBookItemVisual>();
            activeBookItemVisual.SetBookData(bookListItem);
            _bookListItems.Add(bookListItem.BookId, activeBookItemVisual);
            activeBookItem.SetActive(true);
        }
    }

    public void UpdateDisplayingActiveBooks(Book book)
    {
        DisplayActiveBoooks();
    }
}
