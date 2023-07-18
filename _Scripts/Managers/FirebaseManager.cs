using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class FirebaseManager : MonoBehaviour, IInitializable
{
    public static FirebaseManager Instance { get; private set; }

    [Header("Firebase")]
    [SerializeField] private DependencyStatus _dependencyStatus;
    [SerializeField] private FirebaseAuth _auth;
    [SerializeField] private FirebaseUser _user;
    public DatabaseReference DatabaseReference { get; private set; }

    [Header("Login")]
    [SerializeField] private TMP_InputField _emailLoginField;
    [SerializeField] private TMP_InputField _passwordLoginField;
    [SerializeField] private TMP_Text _warningLoginText;
    [SerializeField] private TMP_Text _confirmLoginText;

    [Header("Register")]
    [SerializeField] private TMP_InputField _usernameRegisterField;
    [SerializeField] private TMP_InputField _emailRegisterField;
    [SerializeField] private TMP_InputField _passwordRegisterField;
    [SerializeField] private TMP_InputField _passwordRegisterVerifyField;
    [SerializeField] private TMP_Text _warningRegisterText;

    //public Action OnDatabaseInitialized;
    private bool _isDatabaseInitialized = false;

    public void Initialize()
    {
        StartCoroutine(Notify());
        if (Instance == null)
        {
            Instance = this;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                _dependencyStatus = task.Result;
                if (_dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();

                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + _dependencyStatus);
                    StopCoroutine(Notify());
                }
            });
        }
        else
        {
            Debug.LogError("Cannot create Firebase Manager.\nInstance is already exists");
        }
    }

    public void Deinitialize()
    {
        
    }

    private IEnumerator Notify()
    {
        yield return new WaitUntil(predicate: () => _isDatabaseInitialized);
        GlobalEvents.Instance.OnDatabaseInitialized?.Invoke();
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase");
        _auth = FirebaseAuth.DefaultInstance;
        DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        _isDatabaseInitialized = true;
        Debug.Log("Setting up Firebase finished");

    }

    public void LoginButton()
    {
        StartCoroutine(Login(_emailLoginField.text, _passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(_emailRegisterField.text, _passwordRegisterField.text, _usernameRegisterField.text));
    }

    private IEnumerator Login(string email, string password)
    {
        var loginTask = _auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {loginTask.Exception}");
            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseException.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "User Not Found";
                    break;
            }
            _warningLoginText.text = message;
        }
        else
        {
            _user = loginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", _user.DisplayName, _user.Email);
            _warningLoginText.text = "";

            var databaseTask = DatabaseReference.Child("users").Child(_user.UserId).GetValueAsync();
            yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

            if (databaseTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
                _warningLoginText.text = "Failed to login user\nPlease make sure you have put your data correctly";

            }
            else
            {
                Debug.Log("Reader profile successfully logged in");
                _warningLoginText.text = "";

                string jsonString = databaseTask.Result.GetRawJsonValue();

                if(jsonString != null )
                {
                    ReaderProfileCreator.Instance.TrySetActiveReader(uint.Parse(jsonString));
                    _confirmLoginText.text = "Logged In";
                    UiManager.Instance.ReaderProfileLayout();
                }
                else
                {
                    _warningLoginText.text = $"Failed to login user\nThere is no existing reader profile"; 
                }
            }

        }
    }

    private IEnumerator Register(string email, string password, string username)
    {
        bool isSuperUser = false;

        if(username == "")
        {
            _warningRegisterText.text = "Missing Username";
        }
        else if(_passwordRegisterField.text != _passwordRegisterVerifyField.text)
        {
            _warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            var registerTask = _auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(predicate: ()=> registerTask.IsCompleted);

            if(registerTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {registerTask.Exception}");
                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseException.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                _warningRegisterText.text = message;
            }
            else
            {
                _user = registerTask.Result.User;

                if(_user != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = username };

                    var profileTask = _user.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

                    if(profileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {profileTask.Exception.Message}");
                        FirebaseException firebaseException = profileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseException.ErrorCode;
                        _warningRegisterText.text = "Username Set Failed";
                    }
                    else
                    {

                        if (username.StartsWith("su_"))
                        {
                            isSuperUser = true;
                        }

                        ReaderProfile newProfile = ReaderProfileCreator.Instance.CreateReaderProfile(username, email, isSuperUser);

                        var databaseTask = DatabaseReference.Child("users").Child(_user.UserId).SetValueAsync(newProfile.ReaderId);
                        yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

                        if (databaseTask.Exception != null)
                        {
                            Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
                            _warningRegisterText.text = "Failed to register user\nPlease make sure you have put your data correctly";

                        }
                        else
                        {
                            Debug.Log("Reader profile successfully registered");
                            _warningRegisterText.text = "";
                            UiManager.Instance.LoginLayout();
                        }

                    }
                }
            }
        }
    }
}
