using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    //Tile
    [SerializeField] private GameObject shortTilePrefab;
    [SerializeField] private GameObject longTilePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TextMeshProUGUI combo_txt;

    //Decorate
    [SerializeField] private DecorateController decorate;

    //UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI score_txt;
    [SerializeField] private TextMeshProUGUI score_type_txt;
    [SerializeField] private TextMeshProUGUI gameStatus_txt;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Button playAgain_btn;
    [SerializeField] private Button back_btn;

    [Header("Effect")]
    [SerializeField] private ParticleSystem perfectParticle;
    [SerializeField] private ParticleSystem greatParticle;

    //List star
    [SerializeField] private List<GameObject> starList = new List<GameObject>();

    //Queue left right
    private Queue<GameObject> leftTileQueue = new Queue<GameObject>();
    private Queue<GameObject> rightTileQueue = new Queue<GameObject>();

    [SerializeField] private GameObject tileContainer;
    [SerializeField] private PerfectZone perfectZone;

    /*
        Tinh so beats trong 1s => BMP/60
        Tinh so giay cua 1 beat => 60/BMP
     */
    private float bpm = 144f; //Cupid Speed up BPM 144 => 1s co 144/60 b
    private float beatInterval;
    private float musicTime;
    private int beatCounter = 0;
    private float gameStartDeplay = 7f;
    private float fullSongDuration = 145f; // 2:25p = 145s

    private int score = 0;
    private float timer = 0f;
    private bool isGameOver = false;

    //Au
    private AudioSource musicSource;
    [SerializeField] private List<AudioClip> musicClipList = new List<AudioClip>();

    //System Combo
    private int combo = 0;
    private ScoreType? lastScoreType = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
        musicSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        beatInterval = 60f / bpm;
        timer = 0f;
        musicSource.Play();

        if(playAgain_btn != null && back_btn != null)
        {
            playAgain_btn.onClick.AddListener(() => PlayAgain());
            back_btn.onClick.AddListener(() => BackMenu());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver && Time.timeSinceLevelLoad >= fullSongDuration + 3f)
        {
            Win();
        }

        if (isGameOver)
        {
            gamePanel.SetActive(true);
            return;
        }
        if (musicSource != null)
        {
            musicTime = musicSource.time;
        }

        timer += Time.deltaTime;

        HandleBeatSpawning();

        StarSystem();
    }
    public void OnTileDestroyed(float posY)
    {
        //Debug.Log("Tile destroyed at Y: " + posY);
        ScoreType scoreType = perfectZone.HandleTapAndScore(new Vector2(0, posY));//Lay gia trị score type sau khi tap

        UpdateCombo(scoreType);//Xu ly combo score

        CalculatorScore(scoreType);//Tinh so diem

        StartCoroutine(UpdateUI(scoreType));
    }

    private void HandleBeatSpawning()
    {
        while (timer >= beatInterval)
        {
            beatCounter++;

            if (musicTime < gameStartDeplay)
            {
                if (beatCounter % 6 == 0)
                {
                    SpawnTile();
                    //Debug.Log("A");
                }
            }
            else if (musicTime >= gameStartDeplay && musicTime < 60f)
            {
                if (beatCounter % 2 == 0)
                {
                    SpawnTile();
                    //Debug.Log("B");
                }
            }
            else
            {
                decorate.SpeedUpAnimation();
                SpawnTile();
                //Debug.Log("C");
            }
            timer -= beatInterval;
        }
    }

    private void StarSystem()
    {
        if (isGameOver) return;
        if (starList.Count == 0) return;

        float t = Time.timeSinceLevelLoad;

        // Star 1
        if (t >= 48f)
            starList[0].GetComponent<StarProgressController>().ChangeAnimation(StarState.Full);
        else if (t >= 24f)
            starList[0].GetComponent<StarProgressController>().ChangeAnimation(StarState.Half);
        else
            starList[0].GetComponent<StarProgressController>().ChangeAnimation(StarState.Idle);

        // Star 2
        if (t >= 96f)
            starList[1].GetComponent<StarProgressController>().ChangeAnimation(StarState.Full);
        else if (t >= 72f)
            starList[1].GetComponent<StarProgressController>().ChangeAnimation(StarState.Half);
        else
            starList[1].GetComponent<StarProgressController>().ChangeAnimation(StarState.Idle);

        // Star 3
        if (t >= 144f)
            starList[2].GetComponent<StarProgressController>().ChangeAnimation(StarState.Full);
        else if (t >= 120f)
            starList[2].GetComponent<StarProgressController>().ChangeAnimation(StarState.Half);
        else
            starList[2].GetComponent<StarProgressController>().ChangeAnimation(StarState.Idle);
    }


    private void SpawnTile()
    {
        TilePoolManager.Instance.SpawnTileByPattern(spawnPoint, leftTileQueue, rightTileQueue);
    }
    public bool IsFirstTile(GameObject tile, bool isLeftSide)
    {
        if (isLeftSide)//Peek kiem tra phan tu dau danh sach nhung khong xoa, Dequeue thi lay va xoa pt dau danh sach
            return leftTileQueue.Count > 0 && leftTileQueue.Peek() == tile;
        else
            return rightTileQueue.Count > 0 && rightTileQueue.Peek() == tile;
    }

    public void RemoveFirstTile(bool isLeftSide)
    {
        if (isLeftSide && leftTileQueue.Count > 0)
            leftTileQueue.Dequeue();
        else if (!isLeftSide && rightTileQueue.Count > 0)
            rightTileQueue.Dequeue();
    }


    public void CalculatorScore(ScoreType scoreType)
    {
        score += scoreType switch
        {
            ScoreType.Perfect => 3,
            ScoreType.Great => 2,
            ScoreType.Cool =>1,
            _ => 1
        };

        decorate.Highlight();
        score_txt.text = score.ToString();
    }

    public IEnumerator UpdateUI(ScoreType scoreType)
    {
        score_type_txt.gameObject.SetActive(true);

        if (scoreType == ScoreType.Perfect)
        {
            score_type_txt.text = "Perfect";
            perfectParticle.Play();
            greatParticle.Stop();
            score_type_txt.color = new Color(1f, 0.784f, 0f, 1f);//Vang cam FFC800
        }
        else if (scoreType == ScoreType.Great)
        {
            greatParticle.Play();
            perfectParticle.Stop();
            score_type_txt.text = "Great";
            score_type_txt.color = Color.blue;
        }
        else if(scoreType == ScoreType.Cool)
        {
            greatParticle.Stop();
            perfectParticle.Stop();
            score_type_txt.text = "Cool";
            score_type_txt.color = Color.green;
        }
        else
        {
            yield return null;
        }

        if (combo >= 2)
        {
            combo_txt.text = "x" + combo;
            combo_txt.gameObject.SetActive(true);
        }
        else
        {
            combo_txt.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);

        score_type_txt.gameObject.SetActive(false);
        combo_txt.gameObject.SetActive(false);
    }

    private void UpdateCombo(ScoreType newScoreType)
    {
        if (newScoreType != ScoreType.Perfect)
        {
            combo = 0;
            lastScoreType = null;
            combo_txt.gameObject.SetActive(false);
            return;
        }

        if (lastScoreType == ScoreType.Perfect)
        {
            combo++;
        }
        else
        {
            combo = 1;
        }

        lastScoreType = newScoreType;

        if (combo >= 2)
        {
            musicSource.PlayOneShot(musicClipList[0]);
            combo_txt.gameObject.SetActive(true);
            combo_txt.text = "x" + combo;

            if (combo >= 4)
            {
                int bonus = 2;
                score += bonus;
            }
        }
        else
        {
            combo_txt.gameObject.SetActive(false);
        }
    }

    //Game over or win

    public void GameOver()
    {
        isGameOver = true;
        gameStatus_txt.text = "Game Over";
        tileContainer.gameObject.SetActive(false);
        score_type_txt.gameObject.SetActive(false);
        combo_txt.gameObject.SetActive(false);
        musicSource.Stop();
        musicSource.PlayOneShot(musicClipList[3]);
    }

    public void Win()
    {
        isGameOver = true;
        gameStatus_txt.text = "You Win";
        tileContainer.gameObject.SetActive(false);
        score_type_txt.gameObject.SetActive(false);
        combo_txt.gameObject.SetActive(false);
        musicSource.Stop();
        musicSource.PlayOneShot(musicClipList[2]);

    }


    //On click
    private void PlayAgain()
    {
        if (isGameOver)
        {
            isGameOver = false;
            score = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void BackMenu()
    {
        if (isGameOver)
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    //Getter, setter
    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }
}

























/*
// Login ch app dung Object pooling
    private void SpawnTile()
    {
        TilePoolManager.Instance.SpawnTileByPattern(spawnPoint, leftTileQueue, rightTileQueue);

        /*
        float minX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float maxX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        int slotCount = 4;
        float totalWidth = maxX - minX;
        float slotWidth = totalWidth / slotCount;

        List<int> availableSlots = new List<int> { 0, 1, 2, 3 };

        bool spawnTwoTiles = Random.value < 0.05f;//95% spawn 1 tile, 5% spawn 2 tile

        if (spawnTwoTiles)
        {
            int leftSlot = Random.Range(0, 2);   // 0 hoac 1
            int rightSlot = Random.Range(2, 4);  // 2 hoac 3

            SpawnTileAtSlot(leftSlot, minX, slotWidth);
            SpawnTileAtSlot(rightSlot, minX, slotWidth);
        }
        else//Neu chi spawn 1 thi 0 1 2 3
        {
            int slot = Random.Range(0, 4);
            SpawnTileAtSlot(slot, minX, slotWidth);
        }
    }

private void SpawnTileAtSlot(int slot, float minX, float slotWidth)
{
    float slotCenterX = minX + slotWidth * slot + slotWidth / 2;
    Vector3 spawnPos = new Vector3(slotCenterX, spawnPoint.position.y, spawnPoint.position.z);

    //GameObject tile = Instantiate(shortTilePrefab, spawnPos, Quaternion.identity, tileContainer);

    GameObject tile = TilePoolManager.Instance.GetTileFromPool();
    tile.transform.position = spawnPos;
    //tile.transform.SetParent(tileContainer);

    ShortTile shortTile = tile.GetComponent<ShortTile>();
    if (shortTile != null)
    {
        shortTile.IsLeftSide = (slot < 2);
        if (shortTile.IsLeftSide)
            leftTileQueue.Enqueue(tile);
        else
            rightTileQueue.Enqueue(tile);
    }
}
*/