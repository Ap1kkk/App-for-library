using System;
using TMPro;
using UnityEngine;

public class GetBookFormHandler : MonoBehaviour, IInitializable
{
    [Header("Book Visual Fields")]
    [SerializeField] private TMP_Text _titleField;
    [SerializeField] private TMP_Text _authorField;
    [SerializeField] private TMP_Text _genreField;

    [Header("Input Data Fields")]
    [SerializeField] private TMP_InputField _dayField;
    [SerializeField] private TMP_InputField _monthField;
    [SerializeField] private TMP_InputField _yearField;

    [Header("Message Fields")]
    [SerializeField] private TMP_Text _messageField;

    [Range(1, Globals.MAX_DAY_VALUE)]
    private int _dayValue = 1;
    [Range(1, Globals.MAX_MONTH_VALUE)]
    private int _monthValue = 1;
    [Range(Globals.MIN_YEAR_VALUE, Globals.MAX_YEAR_VALUE)]
    private int _yearValue = 2023;

    private bool _isValidDayInput = false;
    private bool _isValidMonthInput = false;
    private bool _isValidYearInput = false;

    private bool _isValidDateInput => _isValidDayInput && _isValidMonthInput && _isValidYearInput;

    private ReaderProfile _activeReaderProfile;
    private Book _activeBook;

    public void Initialize()
    {
        GlobalEvents.Instance.OnReaderProfileLoggedIn += OnReaderProfileChanged;
        //ReaderProfileCreator.Instance.OnReaderProfileLoggedIn += OnReaderProfileChanged;
        GlobalEvents.Instance.OnBookSelected += OnActiveBookChanged;
    }

    public void Deinitialize()
    {
        GlobalEvents.Instance.OnReaderProfileLoggedIn -= OnReaderProfileChanged;
        //ReaderProfileCreator.Instance.OnReaderProfileLoggedIn -= OnReaderProfileChanged;
        GlobalEvents.Instance.OnBookSelected -= OnActiveBookChanged;
    }

    public void HandleInputData()
    {
        Debug.Log("Handling book getting input data");
        if(_isValidDateInput)
        {
            DateTime deadlineDate = new DateTime(_yearValue, _monthValue, _dayValue);
            _activeReaderProfile.AddActiveBook(_activeBook, deadlineDate);

            GlobalEvents.Instance.OnBookGotByReader(_activeBook);
        }
        GlobalEvents.Instance.OnBookDeselected?.Invoke(_activeBook);
    }

    private void OnReaderProfileChanged(ReaderProfile readerProfile)
    {
        Debug.Log($"Changed active Reader Profile to profile with id: {readerProfile.ReaderId}");
        _activeReaderProfile = readerProfile;
    }

    public void OnActiveBookChanged(Book book) 
    {
        Debug.Log($"Changed active Book to book with id: {book.BookId}");
        _activeBook = book;
        FillDisplayingBookFields();
    }

    private void FillDisplayingBookFields()
    {
        _titleField.text = _activeBook.Title;
        _authorField.text = _activeBook.Author;
        _genreField.text = _activeBook.Genre;
    }

    public void CancelAddingBook()
    {
        GlobalEvents.Instance.OnBookDeselected?.Invoke( _activeBook );
    }

    public void CheckFieldValidation()
    {
        if(_isValidDateInput)
        {
            _messageField.text = "";
        }
    }
    public void CheckDayInput()
    {
        string dayString = _dayField.text;
        if(string.IsNullOrEmpty(dayString))
        {
            _messageField.text = "Please fill day field";
            return;
        }

        if(int.TryParse(dayString, out int value))
        {
            if(value < 1 || value > Globals.MAX_DAY_VALUE) 
            {
                _messageField.text = "Invalid day format";
                _isValidDayInput = false;
            }
            else
            {
                _dayValue = value;
                _isValidDayInput = true;
            }
        }
        else
        {
            _messageField.text = "Invalid day format";
        }
        CheckFieldValidation();
    }
    public void CheckMonthInput()
    {
        string monthString = _monthField.text;
        if (string.IsNullOrEmpty(monthString))
        {
            _messageField.text = "Please fill month field";
            return;
        }

        if (int.TryParse(monthString, out int value))
        {
            if(value < 1 || value > Globals.MAX_MONTH_VALUE)
            {
                _messageField.text = "Invalid month format";
                _isValidMonthInput=false;
            }
            else
            {
                _monthValue = value;
                _isValidMonthInput = true;
            }
        }
        else
        {
            _messageField.text = "Invalid month format";
        }
        CheckFieldValidation();
    }
    public void CheckYearInput()
    {
        string year = _yearField.text;
        if (string.IsNullOrEmpty(year))
        {
            _messageField.text = "Please fill year field";
            return;
        }

        if (int.TryParse(year, out int value))
        {
            if(value < Globals.MIN_YEAR_VALUE || value > Globals.MAX_YEAR_VALUE)
            {
                _messageField.text = "Invalid year format";
                _isValidYearInput=false;
            }
            else
            {
                _yearValue = value;
                _isValidYearInput = true;
            }
        }
        else
        {
            _messageField.text = "Invalid year format";
        }
        CheckFieldValidation();
    }
}
