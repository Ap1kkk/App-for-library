using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private FirebaseManager _firebaseManager;
    [SerializeField] private UiManager _uiManager;

    [Header("Creators")]
    [SerializeField] private BookCreator _bookCreator;
    [SerializeField] private ReaderProfileCreator _readerProfileCreator;

    [Header("Visuals")]
    [SerializeField] private ReaderProfileVisual _readerProfileVisual;
    [SerializeField] private HeaderVisual _headerVisual;
    [SerializeField] private BookPreviewVisual _bookPreviewVisual;
    [SerializeField] private BookListVisual _bookListVisual;

    [Header("Form handlers")]
    [SerializeField] private GetBookFormHandler _getBookFormHandler;

    private GlobalEvents _globalEvents;

    private int _callbackCounter = 0;
    private const int MAX_CALLBACK_COUNT = 2;

    private void Start()
    {
        _globalEvents = new GlobalEvents();

        _firebaseManager.Initialize();

        _bookCreator.Initialize();
        _readerProfileCreator.Initialize();

        _uiManager.Initialize();

        _readerProfileVisual.Initialize();
        _headerVisual.Initialize();
        _bookPreviewVisual.Initialize();
        _bookListVisual.Initialize();

        _getBookFormHandler.Initialize();
    }

    private void OnDestroy()
    {
        _getBookFormHandler.Deinitialize();

        _bookListVisual.Deinitialize();
        _bookPreviewVisual.Deinitialize();
        _headerVisual.Deinitialize();
        _readerProfileVisual.Deinitialize();

        _uiManager.Deinitialize();

        _readerProfileCreator.Deinitialize();
        _bookCreator.Deinitialize();

        _firebaseManager.Deinitialize();
    }

    public void ExitApp()
    {
        _bookCreator.OnDataSaved(SavingCallbackEncounter);
        _readerProfileCreator.OnDataSaved(SavingCallbackEncounter);
    }

    private void SavingCallbackEncounter()
    {
        ++_callbackCounter;
        if(_callbackCounter >= MAX_CALLBACK_COUNT)
        {
            Application.Quit();
        }
    }
}
