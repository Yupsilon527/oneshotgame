using System.Collections.Generic;
using UnityEngine;

public partial class Level : MonoBehaviour
{
    #region Time
    public float enemySpeed = 1f;
    public float gameTime = 0f;
    #endregion
    public enum GameState
    {
        pregame,
        running,
        victorious,
        defeated
    }
    GameState state = GameState.pregame;
    int cRound = 0;
    public int secondsPerRound = 60;

    public ObjectPool bulletpool;
    public GameObject bulletPrefab;
    public GameObject playerPrefab;

    #region Unity Calls
    public static Level main;
    private void Awake()
    {
        main = this;
        InitializeBounds();
    }
    private void Start()
    {
        Instantiate(playerPrefab);
        RoundBegin();
    }
    private void Update()
    {
        gameTime += Time.deltaTime * enemySpeed;
        HandleCamera();

    }
    #endregion
    #region Rounds
    void RoundBegin()
    {
        state = GameState.running;
        ScoreCounter.main.StartCountdown(secondsPerRound);
    }
    #endregion
    #region Bodies
    public List<Body> players;
    public List<Body> bodies;

    public void InitBodies()
    {
        bodies = new List<Body>();
        players = new List<Body>();
    }
    public void RegisterBody(Body b)
    {
        ScoreCounter.main.nBadies++;
        if (!b.IsPlayerControlled())
            bodies.Add(b);
        else
            players.Add(b);
    }
    public void UnregisterBody(Body b)
    {
        ScoreCounter.main.nBadies--;
        bodies.Remove(b);
    }
    public Body[] GetAllBodies()
    {
        return bodies.ToArray();
    }
    #endregion
    #region Bounds
    public Rect CameraBounds;
    public float camOriginalSize = 20;
    public void InitializeBounds()
    {
        camOriginalSize = Camera.main.orthographicSize;
        Vector2 dims = new Vector2(

            Camera.main.aspect * camOriginalSize,
            camOriginalSize
            );
        CameraBounds = new Rect(-dims, dims * 2);
        IssueCameraOrder(new Vector3(0, 0, camOriginalSize), -1);
    }
    #endregion
    #region Camera

    public Camera cam;
    public Vector3 CameraOrder = Vector3.zero;
    public Vector3 CameraOrigin = Vector3.zero;
    public Vector2 CameraTime = Vector2.zero;


    void ClearOrder()
    {
        CameraOrder = new Vector3(-1, 1, -1);
        CameraTime = Vector2.zero;
    }

    public void HandleCamera()
    {
        if (CameraTime.x + CameraTime.y > Time.time)
        {
            float delta = 1f / (CameraTime.x) * Time.deltaTime;
            if (CameraOrder.x != -1 && CameraOrder.y != 1)
            {
                cam.transform.position = new Vector3(
                    cam.transform.position.x + (CameraOrder.x - CameraOrigin.x) * delta,
                    cam.transform.position.y + (CameraOrder.y - CameraOrigin.y) * delta,
                    cam.transform.position.z);
            }
            if (CameraOrder.z != -1)
            {
                cam.orthographicSize = cam.orthographicSize + (CameraOrder.z - CameraOrigin.z) * delta;
            }
        }
        else
        {
            SnapCamera();
        }
    }

    public float GetCameraTime()
    {
        return .2f;
    }

    public void SnapCamera()
    {
        if (CameraOrder.x != -1 && CameraOrder.y != 1)
        {
            cam.transform.position = new Vector3(CameraOrder.x, CameraOrder.y, cam.transform.position.z);
        }

        if (CameraOrder.z != -1)
        {
            cam.orthographicSize = CameraOrder.z;
        }
        ClearOrder();
    }

    public void ScaleCamera(float Size)
    {
        IssueCameraOrder(new Vector3(CameraOrder.x, CameraOrder.y, Size), GetCameraTime());
    }

    public void IssueCameraOrder(Vector3 position)
    {
        IssueCameraOrder(position, GetCameraTime());
    }

    public void IssueCameraOrder(Vector3 position, float time)
    {
        CameraOrigin = new Vector3(transform.position.x, transform.position.y, cam.orthographicSize);
        CameraOrder = position;
        CameraTime = new Vector2(time, Time.time);

        float H = 1;
        if (CameraOrder.z > 0)
        {
            CameraOrder.z = Mathf.Min(Mathf.Max(CameraOrder.z, 5), Mathf.Min(CameraBounds.width, CameraBounds.height));
            H *= CameraOrder.z;
        }
        else
        {
            H *= cam.orthographicSize;
        }
        float W = H * cam.aspect;


        CameraOrder.x = Mathf.Clamp(CameraOrder.x, CameraBounds.xMin + W, CameraBounds.xMax - W);
        CameraOrder.y = Mathf.Clamp(CameraOrder.y, CameraBounds.yMin + H, CameraBounds.yMax - H);

        if (time < 0)
        { SnapCamera(); }
    }
    #endregion
}
