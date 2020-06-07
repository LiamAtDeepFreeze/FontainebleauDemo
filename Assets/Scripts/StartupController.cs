using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartupController : MonoBehaviour
{
    public Camera startupCamera;
    public CanvasGroup canvasGroup;
    public Slider progressDisplay;
    public TextMeshProUGUI progressCounter;
    public float fadeOutTime = 1f;
    public float startDelay = 2f;

    private float _progress;
    private AsyncOperation _sceneOperation;
    
    private void Start()
    {
        progressDisplay.minValue = 0f;
        progressDisplay.maxValue = 100f;
        Invoke(nameof(LoadScene), startDelay);
    }

    private void LoadScene()
    {
        _sceneOperation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        StartCoroutine(UpdateSceneProgress());
    }

    private IEnumerator UpdateSceneProgress()
    {
        while (!_sceneOperation.isDone)
        {
            _progress = _sceneOperation.progress * 100f;
            progressDisplay.value = _progress;
            progressCounter.SetText($"{Mathf.RoundToInt(_progress).ToString()}%");

            yield return null;
        }

        var startTime = Time.time + fadeOutTime;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.time / startTime);
        }

        SceneManager.UnloadSceneAsync(0);
    }
}
