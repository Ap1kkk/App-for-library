using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Accessibility;

public class ReaderProfileCreator : MonoBehaviour, IInitializable
{
    public static ReaderProfileCreator Instance { get; private set; }

    private uint _readerIdCounter = 0;

    private DatabaseReference _databaseReference;

    [SerializeField] private ReaderProfile _activeReader;

    [SerializeField] private Dictionary<uint, ReaderProfile> _readerProfiles = new Dictionary<uint, ReaderProfile>();

    //public Action<ReaderProfile> OnReaderProfileLoggedIn; 

    private struct InitData
    {
        public uint ReaderIdCounter;

        public InitData(uint readerIdCounter)
        {
            ReaderIdCounter = readerIdCounter;
        }
    }

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
            GlobalEvents.Instance.OnDatabaseInitialized += OnDatabaseInit;
            _readerProfiles.Clear();
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
        //FirebaseManager.Instance.OnDatabaseInitialized -= OnDatabaseInit;
    }

    public void OnDatabaseInit()
    {
        _databaseReference = FirebaseManager.Instance.DatabaseReference;
        RetrieveAllProfiles();
        //StartCoroutine(RetriveInitData(InitializeCreatedReaderProfiles));
    }

    public void OnDataSaved(Action onSavingFinished)
    {
        StartCoroutine(SaveReaderProfileToDatabase(_activeReader));
        StartCoroutine(SaveInitData(onSavingFinished));
    }

    public void InitializeCreatedReaderProfiles()
    {
        //for (int readerId = 0; readerId < _readerIdCounter; readerId++)
        //{
        //    StartCoroutine(RetrieveReaderProfileFromDatabase(readerId, OnReaderProfileDataRetrieved));
        //}
    }

    private void OnReaderProfileDataRetrieved(ReaderProfile readerProfile)
    {
        if(!_readerProfiles.ContainsKey(readerProfile.ReaderId))
        {
            Debug.Log($"Added reader profile with id:{readerProfile.ReaderId}");
            _readerProfiles.Add(readerProfile.ReaderId, readerProfile);
        }
        else
        {
            Debug.LogWarning($"Tried to rewrite reader profile with id:{readerProfile.ReaderId}");
        }
    }

    public ReaderProfile CreateReaderProfile(string readerName, string rearedEmail, bool isSuperUser)
    {
        if (readerName == null || readerName == "")
        {
            Debug.LogError($"Can't create reader profile\nReader name is inappropriate: {readerName}");
            return null;
        }

        if (rearedEmail == null || rearedEmail == "")
        {
            Debug.LogError($"Can't create reader profile\nReader email is inappropriate: {rearedEmail}");
            return null;
        }

        ReaderProfile newReaderProfile = new ReaderProfile(_readerIdCounter++, readerName, rearedEmail, Globals.MAX_READER_RATING, isSuperUser, new Dictionary<uint, BookListItem>());
        //ReaderProfile newReaderProfile = new ReaderProfile(_readerIdCounter++, readerName, rearedEmail, Globals.MAX_READER_RATING, isSuperUser, new List<BookListItem>());
        _readerProfiles.Add(newReaderProfile.ReaderId, newReaderProfile);
        Debug.Log(message: $"Reader profile with id:{newReaderProfile.ReaderId} was created");
        StartCoroutine(SaveReaderProfileToDatabase(newReaderProfile));
        return newReaderProfile;
    }

    public bool TrySetActiveReader(uint readerId)
    {
        if(_readerProfiles.ContainsKey(readerId))
        {
            _activeReader = _readerProfiles[readerId];
            GlobalEvents.Instance.OnReaderProfileLoggedIn(_activeReader);
            return true;
        }
        Debug.LogError($"Failed to set active reader profile\nNo existing profile with id: {readerId}");
        return false;
    }

    public IEnumerator SaveReaderProfileToDatabase(ReaderProfile readerProfile)
    {
        var jsonString = JsonUtility.ToJson(readerProfile.SaveData());
        Debug.Log(jsonString);
        var databaseTask = _databaseReference.Child("reader_profiles").Child(readerProfile.ReaderId.ToString()).SetRawJsonValueAsync(jsonString);

        yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);


        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            Debug.Log(message: $"Reader profile with id:{readerProfile.ReaderId} was saved to database");
        }
    }

    public IEnumerator RetrieveReaderProfileFromDatabase(int readerId, Action<ReaderProfile> onReaderProfileRetrieved)
    {
        var databaseTask = _databaseReference.Child("reader_profiles").Child(readerId.ToString()).GetValueAsync();
        yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            Debug.Log("Reader profile successfully retrieved");
            string jsonString = databaseTask.Result.GetRawJsonValue();
            ReaderProfileData retrievedReaderProfileData = JsonUtility.FromJson<ReaderProfileData>(jsonString);
            ReaderProfile retrievedReaderProfile = new ReaderProfile(retrievedReaderProfileData);
            onReaderProfileRetrieved(retrievedReaderProfile);
        }
    }

    private IEnumerator SaveInitData(Action onInitializingFinished)
    {
        Debug.Log("Saving init data..");
        InitData dataToSave = new InitData(_readerIdCounter);
        string jsonString = JsonUtility.ToJson(dataToSave);

        var databaseTask = _databaseReference.Child("system").Child("reader_profile_creator_data").SetRawJsonValueAsync(jsonString);
        yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            Debug.Log("Reader Profile Creator Init Data successfully saved");
        }
        onInitializingFinished();
    }

    private IEnumerator RetriveInitData(Action onRetrievingFinished)
    {
        Debug.Log("Retrieving init data..");
        var databaseTask = _databaseReference.Child("system").Child("reader_profile_creator_data").GetValueAsync();
        yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            string jsonString = databaseTask.Result.GetRawJsonValue();
            InitData retrievedInitData = JsonUtility.FromJson<InitData>(jsonString);
            _readerIdCounter = retrievedInitData.ReaderIdCounter;

            onRetrievingFinished();

            Debug.Log("Reader Profile Init Data successfully retrieved");
        }
    }

    private void RetrieveAllProfiles()
    {
        StartCoroutine(RetrievedAllProfilesFromDatabase());
    }

    private IEnumerator RetrievedAllProfilesFromDatabase()
    {
        var databaseTask = _databaseReference.Child("reader_profiles").OrderByChild("ReaderId").GetValueAsync();
        yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

        if (databaseTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = databaseTask.Result;

            List<uint> activeBookIds = new List<uint>();

            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                string jsonString = childSnapshot.GetRawJsonValue();
                //Debug.Log(jsonString);
                ReaderProfileData readerProfileData = JsonUtility.FromJson<ReaderProfileData>(jsonString);
                ReaderProfile readerProfile = new ReaderProfile(readerProfileData);

                foreach(var bookListItem in readerProfile.ActiveBooksDict.Values)
                {
                    activeBookIds.Add(bookListItem.BookId);
                }

                _readerProfiles.Add(readerProfile.ReaderId, readerProfile);

                _readerIdCounter = readerProfileData.ReaderId + 1;
            }

            Debug.Log($"All existing books were retrieved");
            Debug.Log($"Setting Reader Profile id Counter: {_readerIdCounter}");

            GlobalEvents.Instance.OnAllReaderProfilesWereInitialized?.Invoke();
            GlobalEvents.Instance.OnActiveBooksRecognized?.Invoke(activeBookIds);
        }
    }
}
