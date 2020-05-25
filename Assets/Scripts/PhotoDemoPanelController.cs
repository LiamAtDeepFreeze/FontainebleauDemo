using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.AzureSky;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class PhotoDemoPanelController : MonoBehaviour
{
    [Header("View Finder")]
    public RectTransform viewFinderContainer;
    public Button hideViewFinderButton;
    public Button showViewFinderButton;
    public Button takePhotoButton;

    [Header("Time Of Day")] 
    public TextMeshProUGUI timeOfDayDisplay;
    public Slider timeOfDaySlider;
    public AzureSkyController skyController;
    public AzureTimeController timeController;

    [Header("Buttons")]
    public LensButton[] LensButtons;

    [Header("View Camera")] 
    public Camera firstPersonCamera;
    public Camera viewPortCamera;
    public List<Camera> cameras = new List<Camera>();
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

        timeOfDaySlider.maxValue = timeController.dayLength;
        timeOfDaySlider.onValueChanged.AddListener(UpdateTimeOfDay);
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

    private void UpdateTimeOfDay(float value)
    {
        //timeController.timeline = value;
        timeController.SetTimeline(value);
        timeController.UpdateTimeSystem();
    }

    private void Update()
    {
        timeOfDayDisplay.SetText($"Current Time: {Math.Truncate(timeController.timeline).ToString()}:{((timeController.timeline - Math.Truncate(timeController.timeline)) * 60f).ToString("00")}");
        timeOfDaySlider.value = timeController.timeline;
    }

    private void ResetCameras()
    {
        foreach (var cam in cameras)
        {
            var hdCamera = HDCamera.GetOrCreate(cam);
            hdCamera.Reset();
            hdCamera.volumetricHistoryIsValid = false;
            hdCamera.colorPyramidHistoryIsValid = false;
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
        
        viewPortCamera.fieldOfView = lens.fov;
        viewPortCamera.sensorSize = lens.sensorSize;
        viewPortCamera.focalLength = lens.focalLength;
        additionalCameraData.physicalParameters = lens.physicalParameters;
        ResetCameras();
    }

    private void ToggleViewFinder()
    {
        viewFinderContainer.gameObject.SetActive(!viewFinderContainer.gameObject.activeInHierarchy);
        showViewFinderButton.gameObject.SetActive(!viewFinderContainer.gameObject.activeInHierarchy);
        ResetCameras();
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
        public Vector2 sensorSize = new Vector2(70,51);
        public float focalLength = 50f;
        public HDPhysicalCamera physicalParameters = new HDPhysicalCamera();
    }
}
