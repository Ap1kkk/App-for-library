using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEvents
{
    public static GlobalEvents Instance { get; private set; }

    public Action<ReaderProfile> OnReaderProfileLoggedIn;
    public Action OnDatabaseInitialized;
    public Action<Book> OnBookGotByReader;
    public Action<Book> OnBookSelected;
    public Action<Book> OnBookDeselected;

    public Action OnAllBooksWereInitialized;
    public Action OnAllReaderProfilesWereInitialized;

    public Action<List<uint>> OnActiveBooksRecognized;
    public GlobalEvents() 
    { 
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Cannot create Global Events.\nInstance is already exists");
        }
    }
}
