using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyGuy : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float TapForce = 5;
    public float TiltSmooth = 2;
    public Vector3 StartPosition;

    Rigidbody2D _rigidbody;
    Quaternion _clockwiseRotation = Quaternion.Euler(0, 0, -60);
    Quaternion _counterClockwiseRotation = Quaternion.Euler(0, 0, 35);
    Quaternion _levelRotation = Quaternion.Euler(0, 0, 0);
    CanvasManager game;
    float _pressTime = 0;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        game = CanvasManager.Instance;
        _rigidbody.simulated = false;
    }

    private void Update()
    {
        if (game.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            _pressTime = Time.time;
            transform.rotation = Quaternion.Lerp(transform.rotation, _counterClockwiseRotation, .5F);
            _rigidbody.AddForce(new Vector2(0, TapForce), ForceMode2D.Impulse);
        }
        else if (Input.GetMouseButton(0))
        {
            if (Time.time - _pressTime > .25)
            {
                _rigidbody.AddForce(new Vector2(0, -1 * _rigidbody.velocity.y), ForceMode2D.Force);
                transform.rotation = Quaternion.Lerp(transform.rotation, _levelRotation, TiltSmooth * Time.deltaTime);
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _clockwiseRotation, TiltSmooth * Time.deltaTime);
        }

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ScoreZone")
        {
            OnPlayerScored();
        }

        if (collision.gameObject.tag == "DeadZone")
        {
            _rigidbody.simulated = false;
            OnPlayerDied();
        }
    }

    private void OnEnable()
    {
        CanvasManager.OnGameStarted += OnGameStarted;
        CanvasManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        CanvasManager.OnGameStarted -= OnGameStarted;
        CanvasManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void OnGameStarted()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.simulated = true;
    }

    private void OnGameOverConfirmed()
    {
        transform.localPosition = StartPosition;
        transform.rotation = Quaternion.identity;
    }
}