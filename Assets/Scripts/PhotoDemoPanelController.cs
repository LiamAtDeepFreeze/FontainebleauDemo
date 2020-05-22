using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class PhotoDemoPanelController : MonoBehaviour
{
    [Header("View Finder")]
    public RectTransform viewFinderContainer;
    public Button hideViewFinderButton;
    public Button showViewFinderButton;
    public Button takePhotoButton;

    [Header("Buttons")]
    public LensButton[] LensButtons;

    [Header("View Camera")] 
    public CinemachineVirtualCamera firstPersonCamera;
    public Camera camera;
    public HDAdditionalCameraData additionalCameraData;
    
    private void Start()
    {
        BindButtons();
        ConfigureLensButtons();
        
        //Set the default camera to 70mm
        SetCameraLens(1);
        
        //Hide the show view finder button if it is already active
        showViewFinderButton.gameObject.SetActive(!viewFinderContainer.gameObject.activeInHierarchy);
    }

    private void BindButtons()
    {
        hideViewFinderButton.onClick.AddListener(ToggleViewFinder);
        showViewFinderButton.onClick.AddListener(ToggleViewFinder);
        takePhotoButton.onClick.AddListener(TakePhoto);
    }

    private void ConfigureLensButtons()
    {
        for (var i = 0; i < LensButtons.Length; i++)
        {
            var index = i;
            LensButtons[i].button.onClick.AddListener(() =>SetCameraLens(index));
            LensButtons[i].button.GetComponentInChildren<TextMeshProUGUI>()?.SetText(LensButtons[i].name);
        }
    }

    private void SetCameraLens(int index)
    {
        if (LensButtons.Length >= 0 || index >= 0)
        {
            SetCameraLens(LensButtons[index]);
            return;
        }
        
        SetCameraLens(LensButtons[0]);
    }

    private void SetCameraLens(LensButton lens)
    {
        Debug.Log($"Set lens: {lens.name}");
        
        camera.fieldOfView = lens.fov;
        firstPersonCamera.m_Lens.FieldOfView = lens.fov;
        additionalCameraData.physicalParameters = lens.physicalParameters;
    }

    private void ToggleViewFinder()
    {
        viewFinderContainer.gameObject.SetActive(!viewFinderContainer.gameObject.activeInHierarchy);
        showViewFinderButton.gameObject.SetActive(!viewFinderContainer.gameObject.activeInHierarchy);
    }

    private void TakePhoto()
    {
        
    }

    [Serializable]
    public class LensButton
    {
        public string name = "New Lens";
        public Button button;
        [Range(1,140)]
        public float fov = 70f;
        public LensSettings lensSettings;
        public HDPhysicalCamera physicalParameters = new HDPhysicalCamera();
    }
}
