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
    [SerializeField] private Transform tileContainer;
    [SerializeField] private TextMeshProUGUI combo_txt;

    //Decorate
    [SerializeField] private DecorateController decorate;

    //UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI score_txt;
    [SerializeField] private TextMeshProUGUI score_type_txt;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button playAgain_btn;

    [Header("Effect")]
    [SerializeField] private ParticleSystem perfectParticle;
    [SerializeField] private ParticleSystem greatParticle;

    //Queue left right
    private Queue<GameObject> leftTileQueue = new Queue<GameObject>();
    private Queue<GameObject> rightTileQueue = new Queue<GameObject>();

    //Perfect zone
    [SerializeField] private PerfectZone perfectZone;

    /*
        Tinh so beats trong 1s => BMP/60
        Tinh so giay cua 1 beat => 60/BMP
     */
    private float bpm = 144f; //Cupid Speed up BPM 144 => 1s co 144/60 b
    private float beatInterval;
    private AudioSource musicSource;
    private float musicTime;
    private int beatCounter = 0;
    private float gameStartDeplay = 7f;

    private int score = 0;
    private float timer = 0f;
    private bool isGameOver = false;

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
        playAgain_btn.onClick.AddListener(() => PlayAgain());
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            foreach(Transform child in tileContainer)
            {
                Destroy(child.gameObject);
            }
            gameOverPanel.SetActive(true);
            return;
        }
        if (musicSource != null)
        {
            musicTime = musicSource.time;
        }
        timer += Time.deltaTime;
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
                SpawnTile();
                decorate.SpeedUpAnimation();
                //Debug.Log("C");
            }
            timer -= beatInterval;
        }
    }

    public void OnTileDestroyed(float posY)
    {
        //Debug.Log("Tile destroyed at Y: " + posY);
        ScoreType scoreType = perfectZone.HandleTapAndScore(new Vector2(0, posY));
        HandleCombo(scoreType);
        CalculatorScore(scoreType);
        StartCoroutine(UpdateUI(scoreType));
    }

    private void SpawnTile()
    {
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

        GameObject tile = Instantiate(shortTilePrefab, spawnPos, Quaternion.identity, tileContainer);
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

    private void PlayAgain()
    {
        if (isGameOver)
        {
            isGameOver = false;
            score = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
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

    public void GameOver()
    {
        isGameOver = true;
        musicSource.Stop();
    }

    public IEnumerator UpdateUI(ScoreType scoreType)
    {
        score_type_txt.gameObject.SetActive(true);

        if (scoreType == ScoreType.Perfect)
        {
            score_type_txt.text = "Perfect";
            score_type_txt.color = new Color(1f, 0.784f, 0f, 1f);//Vang cam FFC800
        }
        else if (scoreType == ScoreType.Great)
        {
            score_type_txt.text = "Great";
            score_type_txt.color = Color.green;
        }
        else if(scoreType == ScoreType.Cool)
        {
            score_type_txt.text = "Cool";
            score_type_txt.color = Color.blue;
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

    private void HandleCombo(ScoreType newScoreType)
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
            combo_txt.gameObject.SetActive(true);
            combo_txt.text = "x" + combo;

            if (combo >= 5)
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


    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }
}
