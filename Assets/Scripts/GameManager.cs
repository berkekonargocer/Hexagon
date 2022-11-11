using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Managers;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Instance

    public static GameManager Instance;

    #endregion

    #region Fields

    [Header("Game Settings")]
    #region Private Fields

    private float _startGameDelay = 0.2f;

    private bool _isGameStarted = false;
    private bool _isGameOver = false;
    private bool _isPaused = false;

    #endregion

    #region Properties

    public bool IsPaused { get { return _isPaused; } set { _isPaused = value; } }

    #endregion

    [Header("Best Time Settings")]
    #region Private Fields

    private float _bestTime = 0.0f;

    #endregion

    #region Properties

    public float BestTime { get { return _bestTime; } }

    #endregion

    [Header("Shrink Settings")]
    #region Private Fields

    private float _startingShrinkSpeed;

    #endregion
    #region Serialized Fields

    [SerializeField][Range(1.0f, 25.0f)] private float _shrinkSpeed;

    #endregion

    #region Properties

    public float ShrinkSpeed { get { return _shrinkSpeed; } set { _shrinkSpeed = value; } }

    #endregion

    [Header("Tutorial Panel Settings")]
    #region Private Fields

    private CanvasGroup _tutorialPanel;

    #endregion

    [Header("Game Over Panel Animation Settings")]
    #region Private Fields

    private Transform _objectTransform;

    #endregion

    [Header("Music Speed Settings")]
    #region Private Fields

    float _musicTime = 0;

    #endregion

    [Header("Color Variables")]
    #region Private Fields

    private Color _white = Color.white;

    #endregion

    #endregion

    #region Awake
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GetPersonalRecord();
    }

    #endregion

    private void Update()
    {
        SpeedUpTheAudio();

        /// Activate On Windows Build
        if (Input.GetKeyDown(KeyCode.Escape) && _isGameOver == false)
        {
            if (_isPaused == false)
            {
                PauseGame();
            }
            else if (_isPaused == true)
            {
                StartCoroutine(GUIManager.Instance.ResumeGameCountdown());
            }
        }

        if (_isGameOver)
        {
            if (Input.anyKeyDown)
            {
                RestartGame();
            }
        }
    }

    #region Private Methods
    private void RestartGame()
    {
        GUIManager.Instance.YourNewBestText.alpha = 0;
        GUIManager.Instance.ActivateCanvasGroup(GUIManager.Instance.GameOverPanel, false);
        SceneManager.LoadScene(1);
        Time.timeScale = 1.0f;
        ShrinkSpeed = _startingShrinkSpeed;
        AudioManager.Instance.RestartAudio("Game Music");
        _isGameOver = false;
    }

    private void SpeedUpTheAudio()
    {
        if (_isGameStarted == false || _isPaused == true)
            return;
           
        _musicTime += Time.deltaTime;

        if (_musicTime >= 195f)
        {
            AudioManager.Instance.IncrementAudioPitch("Game Music", 0.1f);
            _musicTime = 95;
        }
    }

    private void GetPersonalRecord()
    {
        _bestTime = PlayerPrefs.GetFloat("BestTime", _bestTime);
    }

    private void LoadScene() 
    {
        SceneManager.LoadScene(1);
        Invoke(nameof(OpenTutorialPanel), 0.01f);
    }

    private void OpenTutorialPanel()
    {
        _tutorialPanel = GameObject.FindWithTag("PANELS/ Tutorial Panel").GetComponent<CanvasGroup>();
        GUIManager.Instance.ActivateCanvasGroup(_tutorialPanel, true);
        Time.timeScale = 0;
    }

    #endregion

    #region Public Methods

    public void StartGame()
    {
        Time.timeScale = 1;
        _musicTime = 0;
        GUIManager.Instance.ActivateCanvasGroup(_tutorialPanel, false);
        _startingShrinkSpeed = _shrinkSpeed;
        AudioManager.Instance.PlayAudio("Game Music");
        _isGameStarted = true;
    }

    public void PlayButton()
    {
        Invoke(nameof(LoadScene), _startGameDelay);
        AudioManager.Instance.StopAudio("Menu Music");
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        _isPaused = true;
        GUIManager.Instance.ActivateCanvasGroup(GUIManager.Instance.PauseMenuPanel, true);
        AudioManager.Instance.PauseAudio("Game Music");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        _isPaused = false;
        AudioManager.Instance.PlayAudio("Game Music");
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void ShrinkSpeedUp(float speedUp)
    {
        ShrinkSpeed += speedUp;
    }


    #endregion

    #region Coroutines

    #region Private
    private IEnumerator PersonalRecord()
    {
        if (TimeManager.Instance.CurrentTime > _bestTime)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            _bestTime = TimeManager.Instance.CurrentTime;
            GUIManager.Instance.YourNewBestText.alpha = 1;
            AnimationManager.Instance.ScaleUpAnimation(GUIManager.Instance.YourNewBestText.transform, Vector3.zero, Vector3.one, 0.3f);
            AudioManager.Instance.PlayAudio("Congratulations");
            PlayerPrefs.SetFloat("BestTime", _bestTime);
        }
    }

    #endregion

    #region Public

    public IEnumerator GameOver()
    {
        Time.timeScale = 0.2f;

        yield return new WaitForSecondsRealtime(2.0f);

        Time.timeScale = 0f;
        AudioManager.Instance.StopAudio("Game Music");
        GUIManager.Instance.GameTimeText();
        GUIManager.Instance.ActivateCanvasGroup(GUIManager.Instance.GameOverPanel, true);
        AudioManager.Instance.PlayAudio("Game Over");
        AnimationManager.Instance.ScaleUpAnimation(GUIManager.Instance.GameOverBorderTransform, Vector3.zero, Vector3.one, 1.0f);
        StartCoroutine(nameof(PersonalRecord));
        _isGameOver = true;
    }

    #endregion

    #endregion

}
