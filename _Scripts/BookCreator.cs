using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class BookCreator : MonoBehaviour, ISavingClass, IInitializable
{
    public static BookCreator Instance { get; private set; }

    private uint _bookIdCounter = 0;

    private DatabaseReference _databaseReference;

    //[SerializeField] private List<Book> _books = new List<Book>();
    [SerializeField] private Dictionary<uint, Book> _booksDict = new Dictionary<uint, Book>();
    [SerializeField] private Dictionary<uint, Book> _activeBooks = new Dictionary<uint, Book>();

    private struct InitData
    {
        public uint BookIdCounter;

        public InitData(uint bookIdCounter)
        {
            BookIdCounter = bookIdCounter;
        }
    }

    public void Initialize()
    {
        if (Instance == null)
        {
            _booksDict.Clear();
            _activeBooks.Clear();
            Instance = this;
            GlobalEvents.Instance.OnDatabaseInitialized += OnDatabaseInit;
            GlobalEvents.Instance.OnActiveBooksRecognized += HandleActiveBooks;

            GlobalEvents.Instance.OnBookGotByReader += OnBookGotbyReader;
            //FirebaseManager.Instance.OnDatabaseInitialized += OnDatabaseInit;
        }
        else
        {
            Debug.LogError("Cannot create Book Creator.\nInstance is already exists");
        }
    }

    public void Deinitialize()
    {
        GlobalEvents.Instance.OnDatabaseInitialized -= OnDatabaseInit;
        GlobalEvents.Instance.OnActiveBooksRecognized -= HandleActiveBooks;

        GlobalEvents.Instance.OnBookGotByReader -= OnBookGotbyReader;
        //FirebaseManager.Instance.OnDatabaseInitialized -= OnDatabaseInit;
    }

    private void OnDatabaseInit()
    {
        _databaseReference = FirebaseManager.Instance.DatabaseReference;
        InitializeExistingBooks();
        //StartCoroutine(RetriveInitData(InitializeCreatedBooks));
    }

    public void OnDataSaved(Action onSavingFinished)
    {
        //StartCoroutine(SaveInitData(onSavingFinished));
    }

    private void InitializeExistingBooks()
    {
        StartCoroutine(RetrieveAllBooks());
    }

    private void HandleActiveBooks(List<uint> activeBooks)
    {
        foreach (uint bookId in activeBooks)
        {
            _activeBooks.Add(bookId, _booksDict[bookId]);
        }
        Debug.Log($"All active books were handled");
    }

    public bool CheckActiveBook(Book book)
    {
        return _activeBooks.ContainsKey(book.BookId);
    }

    private void OnBookGotbyReader(Book book)
    {
        _activeBooks.Add(book.BookId, book);
    }

    public Book CreateBook(string title, string author, string genre)
    {
        if(title == null || title == "")
        {
            Debug.LogError($"Can't create book\nTitle is inappropriate: {title}");
            return null;
        }

        if (author == null || author == "")
        {
            Debug.LogError($"Can't create book\nAuthor is inappropriate: {author}");
            return null;
        }

        if (genre == null || genre == "")
        {
            Debug.LogError($"Can't create book\nGenre is inappropriate: {genre}");
            return null;
        }


        Book newBook = new Book(_bookIdCounter++, title, author, genre);
        Debug.Log(message: $"Book with id:{newBook.BookId} was created");
        StartCoroutine(SaveBookToDatabase(newBook));
        return newBook;
    }

    public IEnumerator SaveBookToDatabase(Book book)
    {
        var jsonString = JsonUtility.ToJson(book);
        var databaseTask = _databaseReference.Child("books").Child(book.BookId.ToString()).SetRawJsonValueAsync(jsonString);

        yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            Debug.Log(message: $"Book with id:{book.BookId} was saved to database");
        }
    }

    public IEnumerator RetrieveBookFromDatabase(int bookId, Action<Book> onBookRetrieved)
    {
        var databaseTask = _databaseReference.Child("books").Child(bookId.ToString()).GetValueAsync();
        yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            Debug.Log($"Book with id: {bookId} successfully retrieved");
            string jsonString = databaseTask.Result.GetRawJsonValue();
            Book retrievedBook = JsonUtility.FromJson<Book>(jsonString);
            onBookRetrieved(retrievedBook);
        }
    }

    //private IEnumerator SaveInitData(Action onInitializingFinished)
    //{
    //    Debug.Log("Saving init data..");
    //    InitData dataToSave = new InitData(_bookIdCounter);
    //    string jsonString = JsonUtility.ToJson(dataToSave);

    //    var databaseTask = _databaseReference.Child("system").Child("book_creator_data").SetRawJsonValueAsync(jsonString);
    //    yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

    //    if (databaseTask.Exception != null)
    //    {
    //        Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
    //    }
    //    else
    //    {
    //        Debug.Log("Book Creator Init Data successfully saved");
    //    }
    //    onInitializingFinished();
    //}

    //private IEnumerator RetriveInitData(Action onRetrievingFinished)
    //{
    //    Debug.Log("Retrieving init data..");
    //    var databaseTask = _databaseReference.Child("system").Child("book_creator_data").GetValueAsync();
    //    yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

    //    if (databaseTask.Exception != null)
    //    {
    //        Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
    //    }
    //    else
    //    {
    //        string jsonString = databaseTask.Result.GetRawJsonValue();
    //        InitData retrievedInitData = JsonUtility.FromJson<InitData>(jsonString);
    //        _bookIdCounter = retrievedInitData.BookIdCounter;

    //        onRetrievingFinished();

    //        Debug.Log("Book Creator Init Data successfully retrieved");
    //    }
    //}

    private IEnumerator RetrieveAllBooks()
    {
        var databaseTask = _databaseReference.Child("books").OrderByChild("BookId").GetValueAsync();
        yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = databaseTask.Result;

            foreach(DataSnapshot childSnapshot in snapshot.Children)
            {
                string jsonString = childSnapshot.GetRawJsonValue();
                Book retrievedBook = JsonUtility.FromJson<Book>(jsonString);
                _booksDict.Add(retrievedBook.BookId, retrievedBook);
                //_books.Add(retrievedBook);
                _bookIdCounter = retrievedBook.BookId + 1;
            }

            Debug.Log($"All existing books were retrieved");

            //_bookIdCounter = _books[_books.Count - 1].BookId + 1;
            Debug.Log($"Setting Book Id Counter: {_bookIdCounter}");

            GlobalEvents.Instance.OnAllBooksWereInitialized?.Invoke();
        }
    }

    public List<Book> GetAllBooks()
    {
        var list = new List<Book>();

        foreach (var book in _booksDict.Values)
        {
            list.Add(book);
        }
        return list;
    }

    public List<Book> GetAllActiveBooks()
    {
        var list = new List<Book>();

        foreach (var book in _activeBooks.Values)
        {
            list.Add(book);
        }
        return list;
    }

    public List<Book> GetAllInactiveBooks()
    {
        var list = new List<Book>();

        foreach (var book in _booksDict.Values)
        {
            if(!_activeBooks.ContainsKey(book.BookId))
            {
                list.Add(book);
            }
        }
        return list;
    }

    public Book GetBookById(uint bookId)
    {
        if(_booksDict.ContainsKey(bookId))
        {
            return _booksDict[bookId];
        }
        else
        {
            Debug.LogError($"Can't find book with id: {bookId}.\nThere's no such book in container");
            return null;
        }
    }
}
