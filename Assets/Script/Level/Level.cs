using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Level : MonoBehaviour
{
    public enum GameState
    {
        pregame,
        running,
        victorious,
        defeated
    }
    public GameState state = GameState.pregame;
    int cRound = 0;
    public int secondsPerRound = 60;
    public int maxExecutives = 20;
    float roundBeginTime = 0;

    public ObjectPool bulletpool;
    public GameObject bulletPrefab;
    public GameObject playerPrefab;
    public float cameraDefaultSize = 5f;
    public float cameraZoomedinSize = 1.5f;

    #region Unity Calls
    public static Level main;
    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        Instantiate(playerPrefab);
        RoundBegin();
    }
    private void Update()
    {
        HandleState();
    }
    #endregion
    #region Rounds
    void RoundBegin()
    {
        state = GameState.running;
        roundBeginTime = Time.time;
        PlayerController.main.Restart();
        cam.transform.position = Vector3.zero;
        UpdateLevelBounds();

        ScoreCounter.main.StartCountdown(secondsPerRound);
        ScoreCounter.main.SetTimerVisible(true);
    }
    void RoundEnd()
    {
        cRound++;
        state = GameState.pregame;
    }
    public void RoundProgress(bool triggeredByEvent)
    {
        state = GameState.victorious;

        ScoreCounter.main.StopCountdown();
        ScoreCounter.main.SetTimerVisible(false);

        if (triggeredByEvent)
        {
            EnemyController.main.ClearEnemies();
            SpawnExecs();
            StartCoroutine(SmoothMoveCamera(cam.transform.position, executiveSpawnArea.transform.position, cameraDefaultSize, cameraZoomedinSize, 1));
            UpdateLevelBounds();
        }
    }
    void GameEnded()
    {
        state = GameState.defeated;
    }
    void HandleState()
    {
        switch (state)
        {
            case GameState.pregame:
                RoundBegin();
                break;
            case GameState.running:
                if (Time.time - roundBeginTime > secondsPerRound)
                {
                    GameEnded();
                }
                break;
            case GameState.victorious:
                if (PlayerController.main.fireState >= PlayerController.FireState.fired && projectiles.Count == 0)
                {
                    RoundEnd();
                }
                break;
        }
    }
    #endregion
    #region Bodies
    public List<Body> bodies = new();
    public List<Projectile> projectiles = new();
    public void RegisterBody(Mob m)
    {
        if (m is Body b)
            bodies.Add(b);
        else if (m is Projectile p)
            projectiles.Add(p);
    }
    public void UnregisterBody(Mob m)
    {
        if (m is Body b)
            bodies.Remove(b);
        else if (m is Projectile p)
            projectiles.Remove(p);
    }
    #endregion
    #region Bounds
    public Rect CameraBounds;
    public float camOriginalSize = 20;
    public void UpdateLevelBounds()
    {
        camOriginalSize = Camera.main.orthographicSize;

        if (state == GameState.running)
        {
            Vector2 dims = new Vector2(

                Camera.main.aspect * camOriginalSize,
                camOriginalSize
                );
            CameraBounds = new Rect(-dims, dims * 2);
        }
        else
        {
            Vector2 dims = executiveSpawnArea.transform.localScale / 2;
            CameraBounds = new Rect((Vector2)executiveSpawnArea.transform.localPosition - dims, (Vector2)executiveSpawnArea.transform.localPosition + dims * 2);
        }
    }
    #endregion
    #region Camera

    public Camera cam;

    IEnumerator SmoothMoveCamera(Vector3 start, Vector3 end, float startSize, float endSize, float time)
    {
        for (float t = 0; t < time; t += Time.fixedDeltaTime / time)
        {
            cam.transform.position = Vector3.Lerp(start, end, t);
            cam.orthographicSize = startSize + (endSize - startSize) * t;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion
    #region Executive Area

    public GameObject executiveSpawnArea;
    public EnemyData executiveEnemy;

    void SpawnExecs()
    {
        Vector2 area = executiveSpawnArea.transform.localScale / 2;
        EnemyController.main.SpawnEnemiesInArea(new Rect((Vector2)executiveSpawnArea.transform.localPosition - area, (Vector2)executiveSpawnArea.transform.localPosition + area * 2), GetNumExecutives(), executiveEnemy);
    }
    public int GetNumExecutives()
    {
        return Mathf.FloorToInt((Time.time - roundBeginTime) / secondsPerRound * maxExecutives);
    }

    #endregion
}
