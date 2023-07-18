using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HeaderVisual : MonoBehaviour, IInitializable
{
    [Header("Buttons")]
    [SerializeField] private TMP_Text _userProfileButtonText;
    [SerializeField] private Button _addBooksButton;
    [SerializeField] private Button _addReaderProfilesButton;

    private bool isSuperUser = false;

    public void Initialize()
    {
        GlobalEvents.Instance.OnReaderProfileLoggedIn += ApplyReaderProfileData;
        //ReaderProfileCreator.Instance.OnReaderProfileLoggedIn += ApplyReaderProfileData;
    }

    public void Deinitialize()
    {
        GlobalEvents.Instance.OnReaderProfileLoggedIn -= ApplyReaderProfileData;
        //ReaderProfileCreator.Instance.OnReaderProfileLoggedIn -= ApplyReaderProfileData;
    }

    public void ApplyReaderProfileData(ReaderProfile readerProfile)
    {
        _userProfileButtonText.text = readerProfile.ReaderName;
        isSuperUser = readerProfile.IsSuperUser;
    }

    public void OnEnable()
    {
        if(isSuperUser)
        {
            _addBooksButton.gameObject.SetActive(true);
            _addReaderProfilesButton.gameObject.SetActive(true);
        }
    }

    public void OnDisable()
    {
        _addBooksButton.gameObject.SetActive(false);
        _addReaderProfilesButton.gameObject.SetActive(false);
    }
}
