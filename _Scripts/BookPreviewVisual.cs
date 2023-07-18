using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookPreviewVisual : MonoBehaviour ,IInitializable
{
    [SerializeField] private GameObject _previewPanel;

    public void Initialize()
    {
        _previewPanel.SetActive(false);
        GlobalEvents.Instance.OnBookSelected += ShowBookPreview;
        GlobalEvents.Instance.OnBookDeselected += HideBookPreview;
    }

    public void Deinitialize()
    {
        GlobalEvents.Instance.OnBookSelected -= ShowBookPreview;
        GlobalEvents.Instance.OnBookDeselected -= HideBookPreview;
    }

    private void ShowBookPreview(Book book)
    {
        _previewPanel.SetActive(true);
    }

    private void HideBookPreview(Book book)
    {
        _previewPanel.SetActive(false);
    }
}
