using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;

public class UiManager : MonoBehaviour, IInitializable
{
    public static UiManager Instance {get; private set;}

    [Header("Layouts")]
    [SerializeField] private Canvas _loginLayout;
    [SerializeField] private Canvas _registerLayout;
    [SerializeField] private Canvas _booksLayout;
    [SerializeField] private Canvas _addBookLayout;
    [SerializeField] private Canvas _addReaderProfileLayout;
    [SerializeField] private Canvas _readerProfileLayout;
    [SerializeField] private Canvas _headerLayout;
    public void Initialize()
    {
        if(Instance ==  null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Cannot create UI Manager.\nInstance is already exists");
        }
    }

    public void Deinitialize()
    {

    }

    public void ClearLayouts()
    {
        _loginLayout.gameObject.SetActive(false);
        _registerLayout.gameObject.SetActive(false);
        _booksLayout.gameObject.SetActive(false);
        _addBookLayout.gameObject.SetActive(false);
        _addReaderProfileLayout.gameObject.SetActive(false);
        _readerProfileLayout.gameObject.SetActive(false);
        _headerLayout.gameObject.SetActive(false);
    }

    public void LoginLayout()
    {
        ClearLayouts();
        _loginLayout.gameObject.SetActive(true);
    }

    public void RegisterLayout()
    {
        ClearLayouts();
        _registerLayout.gameObject.SetActive(true);
    }

    public void BooksLayout()
    {
        ClearLayouts();
        _booksLayout.gameObject.SetActive(true);
        _headerLayout.gameObject.SetActive(true);
    }

    public void AddBookLayout()
    {
        ClearLayouts();
        _addBookLayout.gameObject.SetActive(true);
        _headerLayout.gameObject.SetActive(true);
    }

    public void AddReaderProfileLayout()
    {
        ClearLayouts();
        _addReaderProfileLayout.gameObject.SetActive(true);
        _headerLayout.gameObject.SetActive(true);
    }

    public void ReaderProfileLayout()
    {
        ClearLayouts();
        _readerProfileLayout.gameObject.SetActive(true);
        _headerLayout.gameObject.SetActive(true);
    }
}
