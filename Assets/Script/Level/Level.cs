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
    float roundBeginTime = 0;

    public ObjectPool bulletpool;
    public GameObject bulletPrefab;
    public GameObject playerPrefab;

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
        ScoreCounter.main.StartCountdown(secondsPerRound);
        roundBeginTime = Time.time;
        PlayerController.main.Restart();
        cam.transform.position = Vector3.zero;
        InitializeBounds();
    }
     void RoundEnd()
    {
        cRound++;
        state = GameState.pregame;
    }
    public void RoundProgress()
    {
        state = GameState.victorious;
        EnemyController.main.ClearEnemies();
        StartCoroutine(SmoothMoveCamera(cam.transform.position,executiveSpawnArea.transform.position, 1));
        InitializeBounds();
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
    public void InitializeBounds()
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
            Vector2 dims =  executiveSpawnArea.transform.localScale;
            CameraBounds =  new Rect((Vector2)executiveSpawnArea.transform.localPosition -dims, (Vector2)executiveSpawnArea.transform.localPosition + dims * 2);
        }
    }
    #endregion
    #region Camera

    public Camera cam;

    IEnumerator SmoothMoveCamera(Vector3 start, Vector3 end, float time)
    {
        for (float t = 0; t< time; t+= Time.fixedDeltaTime / time)
        {
            cam.transform.position = Vector3.Lerp(start, end, t);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion
    #region Executive Area

    public GameObject executiveSpawnArea;
    public EnemyData executiveEnemy;

    public int GetNumExecutives()
    {
        return Mathf.FloorToInt( (Time.time - roundBeginTime) / secondsPerRound * 5);
    }

    #endregion
}
