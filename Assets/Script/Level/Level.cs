using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VikingParty;

public partial class Level : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip elevatorMusic;
    public AudioClip actionMusic;
    public AudioClip roundBegin;
    public enum GameState
    {
        pregame,
        waiting,
        running,
        victorious,
        defeated
    }
    public GameState state = GameState.pregame;
    public int currentRound = 0;
    public int secondsPerRound = 60;
    public int maxExecutives = 20;
    float roundBeginTime = 0;
    float penaltyTime;

    public ObjectPool bulletpool;
    public GameObject bulletPrefab;
    public GameObject textPrefab;
    public GameObject playerPrefab;
    public float cameraDefaultSize = 5f;
    public float cameraZoomedinSize = 1.5f;
    public GameObject levelStartPosition;

    #region Unity Calls
    public static Level main;
    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        Instantiate(playerPrefab);
        PlayerController.main.transform.position = executiveSpawnArea.transform.position;
        PreRound();
        if (NotificationWidget.instance != null)
            NotificationWidget.instance.DisplayNotification("NOW HURRY!");
    }
    private void Update()
    {
        HandleState();
    }
    #endregion
    #region Rounds
    void RoundBegin()
    {
        EnemyController.main.ClearEnemies();
        state = GameState.running;
        roundBeginTime = Time.time;

        PlayerController.main.Restart(false);
        cam.transform.position = Vector3.zero;
        cam.orthographicSize = cameraDefaultSize;
        UpdateLevelBounds();
        ResetPenaltyTime();

        ScoreCounter.main.StartCountdown(secondsPerRound - GetRoundTime());
        ScoreCounter.main.SetTimerVisible(true);
        ScoreCounter.main.SetHelperVisible(false);

        if (NotificationWidget.instance != null)
            NotificationWidget.instance.DisplayNotification("Round "+(currentRound+1));
        if (musicSource!=null)
        {
            if (roundBegin!=null)
                PlayerController.main.audioSource.PlayOneShot(roundBegin);
            musicSource.clip = actionMusic;
            musicSource.Play();
        }
    }
    public float GetRoundTime()
    {
        return Time.time - roundBeginTime + penaltyTime;
    }
    void RoundEnd()
    {
        currentRound++;
        if (currentRound > 0 &&  PlayerController.main.Score == PlayerController.main.lastRoundScore)
        {
            GameEnded();
        }
        else
        {
            PreRound();
        }
    }
    void PreRound()
    {
        state = GameState.waiting;
        StartCoroutine(SmoothMoveCamera(cam.transform.position, executiveSpawnArea.transform.position, cameraDefaultSize, cameraZoomedinSize, 0));
        roundBeginTime = Time.time + 3;
        UpdateLevelBounds();

        ScoreCounter.main.SetTimerVisible(false);
        ScoreCounter.main.SetHelperVisible(false);
    }
    public void RoundProgress(bool triggeredByEvent)
    {
        state = GameState.victorious;

        ScoreCounter.main.StopCountdown();
        ScoreCounter.main.SetTimerVisible(false);
        ScoreCounter.main.SetHelperVisible(true);

        if (musicSource != null)
        {
            musicSource.clip = elevatorMusic;
            musicSource.Play();
        }

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
        if (state != GameState.defeated)
        {
            state = GameState.defeated;
            PlayerController.main.Die();
        }
    }
    void HandleState()
    {
        switch (state)
        {
            case GameState.waiting:
                if (Time.time > roundBeginTime)
                {
                    RoundBegin();
                }
                break;
            case GameState.running:
                if (GetRoundTime() > secondsPerRound)
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
        if (time > 0)
            for (float t = 0; t < time; t += Time.fixedDeltaTime / time)
            {
                cam.transform.position = Vector3.Lerp(start, end, t);
                cam.orthographicSize = startSize + (endSize - startSize) * t;
                yield return new WaitForEndOfFrame();
            }
        cam.transform.position = end;
        cam.orthographicSize = endSize;
    }
    #endregion
    #region Executive Area

    public GameObject executiveSpawnArea;

    void SpawnExecs()
    {
        Vector2 area = executiveSpawnArea.transform.lossyScale / 2;
        EnemyController.main.SpawnEnemiesInArea(new Rect((Vector2)executiveSpawnArea.transform.localPosition - area, area * 2), GetNumExecutives());
    }
    public int GetMaxExecutives()
    {
        return Mathf.Max(maxExecutives, PlayerController.main.startingWeapon.ProjectileCount + PlayerController.main.collectedBonuses.pProjectiles);
    }
    public int GetNumExecutives()
    {
        return Mathf.FloorToInt((Time.time - roundBeginTime) / secondsPerRound * GetMaxExecutives());
    }

    #endregion

    public TextEffectController TextEffect(string text, Vector3 position, Color color, float delay = 0, float scale = .1f, string animation = "Float Up")
    {
        GameObject effect = bulletpool.PoolItem(textPrefab);

        if (effect == null)
            return null;
        effect.transform.position = position;
        if (effect.TryGetComponent(out TextEffectController tefX))
        {
            tefX.ChangeTextValue(text);
            tefX.ChangeColor(color);
            tefX.Emit(animation, delay, scale);
            return tefX;
        }
        return null;
    }

    public void ExtendPenaltyTime(float time)
    {
        penaltyTime += time;
        ScoreCounter.main.StartCountdown(secondsPerRound - GetRoundTime());
    }
    public void ResetPenaltyTime()
    {
        penaltyTime = 0;
    }
}
