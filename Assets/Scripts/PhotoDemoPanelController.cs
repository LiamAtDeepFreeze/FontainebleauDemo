using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.AzureSky;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utility;

[RequireComponent(typeof(Canvas))]
public class PhotoDemoPanelController : MonoBehaviour
{
    [Header("View Finder")]
    public RectTransform viewFinderContainer;
    public Button hideViewFinderButton;
    public Button showViewFinderButton;
    public Button takePhotoButton;
    public Button toggleOverlay;

    [Header("Time Of Day")] 
    public bool animateTimeOfDay;
    public TextMeshProUGUI timeOfDayDisplay;
    public Slider timeOfDaySlider;
    public AzureSkyController skyController;
    public AzureTimeController timeController;

    [Header("Message Display")] 
    public MessageDisplay messageDisplay;

    [Header("Buttons")]
    public LensButton[] LensButtons;

    [Header("Overlay")] 
    public Color overlayColor = Color.white;
    public Image overlay;
    public Animator flashEffect;

    [Header("View Camera")] 
    public Camera firstPersonCamera;
    public Camera viewPortCamera;
    public List<Camera> cameras = new List<Camera>();
    public HDAdditionalCameraData additionalCameraData;
    private static readonly int AnimationTriggerFlash = Animator.StringToHash("Flash");

    private bool _captureQueued;

    private void Start()
    {
        BindButtons();
        ConfigureLensButtons();
        
        //Set the default camera to 70mm
        SetCameraLens(1);

        if (timeController == null)
        {
            timeOfDaySlider.transform.parent.gameObject.SetActive(false);
        }

        if (overlay != null)
        {
            overlay.color = overlayColor;
        }
        
        //Hide the show view finder button if it is already active
        showViewFinderButton.gameObject.SetActive(!viewFinderContainer.gameObject.activeInHierarchy);
    }

    private void BindButtons()
    {
        hideViewFinderButton.onClick.AddListener(ToggleViewFinder);
        showViewFinderButton.onClick.AddListener(ToggleViewFinder);
        takePhotoButton.onClick.AddListener(TakePhoto);
        toggleOverlay.onClick.AddListener(ToggleOverlay);

        if (timeController != null)
        {
            timeOfDaySlider.maxValue = timeController.dayLength;
            timeOfDaySlider.onValueChanged.AddListener(UpdateTimeOfDay);
        }
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
        timeController.enabled = true;
        //timeController.timeline = value;
        timeController.SetTimeline(value);
        timeController.UpdateTimeSystem();
    }

    private void Update()
    {
        if (timeController == null)
        {
            return;
        }
    
        timeOfDayDisplay.SetText($"Current Time: {Math.Truncate(timeController.timeline).ToString()}:{((timeController.timeline - Math.Truncate(timeController.timeline)) * 60f).ToString("00")}");
        timeOfDaySlider.value = timeController.timeline;

        if (timeController.enabled != animateTimeOfDay)
        {
            timeController.enabled = animateTimeOfDay;
        }
    }

    private void OnPostRender()
    {
        if (_captureQueued)
        {
            _captureQueued = false;
            CaptureToFile();
        }
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
        //Reset button states
        foreach (var lensButton in LensButtons)
        {
            lensButton.button.interactable = true;
        }
        
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

        lens.button.interactable = false;
        
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
        flashEffect.SetTrigger(AnimationTriggerFlash);
        CaptureToFile();
    }

    private void CaptureToFile()
    {
        var renderTexture = viewPortCamera.targetTexture;
        if (renderTexture == null)
        {
            Debug.LogError("There is no active texture set for the current camera");
            ShowPopup("Failed to save photo", "Target texture is null");
            return;
        }

        try
        {
            var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0,0, renderTexture.width, renderTexture.height),0,0);
            texture.Apply();

            RenderTexture.active = null;

            var fileName = DateTime.Now.ToString("yy-MMM-dd_HH-mm-ss");
            var filePath = $"{Application.persistentDataPath}\\Captures\\";
            var fileLocation = $"{filePath}{fileName}.png";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var bytes = texture.EncodeToPNG();
            File.Create(fileLocation);
            File.WriteAllBytes(fileLocation, bytes);
            
            ShowPopup("Capture Saved", fileLocation);
        }
        catch (Exception exception)
        {
            ShowPopup("Failed to save photo", exception.Message);
            throw;
        }
    }

    private void ShowPopup(string message, string submessage = null)
    {
        messageDisplay.ShowMessage(message, submessage);
    }

    private void ToggleOverlay()
    {
        overlay.enabled = !overlay.enabled;
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
