using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public delegate void GameEvent();
    public static event GameEvent OnGameStarted;
    public static event GameEvent OnGameOverConfirmed;
    public static CanvasManager Instance;

    public GameObject StartPage;
    public GameObject GameOverPage;
    public GameObject CountdownPage;
    public Text ScoreText;

    int _score = 0;
    bool _gameOver = true;

    public bool GameOver => _gameOver;
    public int Score => _score;

    public void ConfirmGameOver()
    {
        OnGameOverConfirmed?.Invoke();
        ScoreText.text = "0";
        SetPageState(PageState.Start);
    }

    public void StartGame()
    {
        SetPageState(PageState.Countdown);
    }

    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }

    private void Start()
    {
        SetPageState(PageState.Start);
    }

    private void Awake()
    {
        Instance = this;
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                CountdownPage.SetActive(false);
                break;
            case PageState.Start:
                StartPage.SetActive(true);
                GameOverPage.SetActive(false);
                CountdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                StartPage.SetActive(false);
                GameOverPage.SetActive(true);
                CountdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                StartPage.SetActive(false);
                GameOverPage.SetActive(false);
                CountdownPage.SetActive(true);
                break;
        }
    }

    void OnEnable()
    {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        FlyGuy.OnPlayerScored += OnPlayerScored;
        FlyGuy.OnPlayerDied += OnPlayerDied;
    }

    void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        FlyGuy.OnPlayerScored -= OnPlayerScored;
        FlyGuy.OnPlayerDied -= OnPlayerDied;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted?.Invoke();
        _score = 0;
        _gameOver = false;
    }

    private void OnPlayerScored()
    {
        _score++;
        ScoreText.text = _score.ToString();
    }

    private void OnPlayerDied()
    {
        _gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (_score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", _score);
        }
        SetPageState(PageState.GameOver);
    }
}