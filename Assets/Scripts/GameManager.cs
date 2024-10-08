using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Serializable]
    public class PlayerInfo
    {
        public string playerName;
        public int highScore;
        public int itemAddTimeCount;
        public int multiScoreCount;
        public int ranking;
    }

    [Header("PLAYER DATAS")]
    [SerializeField] private InputField playerNameInput;
    [SerializeField] private PlayerInfo[] playerInfoArray;
    [SerializeField] private int playerIndexArray = 0;
    [SerializeField] private PlayerInfo currentPlayer;
    [SerializeField] private int currentPlayerIndex = -1;

    [Header("GAME PAGES")]
    [SerializeField] private int currentPage;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private GameObject transPage;
    [SerializeField] private bool pageAnim;

    [Header("COVER SCENE")]
    [Space(25)]
    public GameObject errorMessage;

    [Header("MAIN MENU")]
    [Space(25)]
    [SerializeField] private GameObject instructionPage;
    [SerializeField] private GameObject leaderboardPage;
    [SerializeField] private GameObject mainMenuPage;

    [Header("CURRENT PLAYER DATA")]
    [Space(15)]
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text highScoresPlayerText;
    [SerializeField] private Text rankingPlayerText;

    [Header("LEADERBOARD")]
    [Space(15)]
    [SerializeField] private Text[] playerRankingGroup;
    [SerializeField] private Text[] playerRNameGroup;
    [SerializeField] private Text[] playerRScoreGroup;

    [Header("ITEMS")]
    [Space(15)]
    [SerializeField] private GameObject itemTime;
    [SerializeField] private GameObject itemMulti;
    [SerializeField] private int itemTimeCount;
    [SerializeField] private int itemMultiCount;
    [SerializeField] private int itemTimeCountCurrent;
    [SerializeField] private int itemMultiCountCurrent;
    [SerializeField] private bool isItemTimeCountZero = false;
    [SerializeField] private bool isItemMultiCountZero = false;
    [SerializeField] private Text itemTimeCountText;
    [SerializeField] private Text itemMultiCountText;

    [Header("GAME START")]
    [Space(25)]
    public bool gameIsReady;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private Text countdownText;

    [Header("GAME TIME COUNTDOWN")]
    [Space(15)]
    [SerializeField] private Text countdownGameText;
    private int minutes;
    private int seconds;
    [SerializeField] private float elapedTime;
    [SerializeField] private float timeLeft;
    [SerializeField] private float totalTime;
    [SerializeField] private bool startCount;
    [SerializeField] private bool addTimeItem = false;
    [SerializeField] private int addTimeItemSeconds;

    [Header("PAUSE GAME")]
    [Space(15)]
    [SerializeField] private GameObject pausePanel;
    private float previousTimeScale = 1;
    [SerializeField] private bool isPaused;

    [Header("CARDS")]
    [Space(15)]
    [SerializeField] private GameObject[] cardList;
    public GameObject[] cardListChange1;
    public GameObject[] cardListChange2;
    public GameObject[] cardListChange3;
    public RectTransform[] startPos;

    [Header("HOLES")]
    [Space(15)]
    public GameObject[] holeShapes;
    public RectTransform[] startHolePos;
    public bool unablePick = false;

    [Header("SCORES")]
    [Space(15)]
    [SerializeField] private int scores;
    [SerializeField] private Text totalScores;
    [SerializeField] private bool multiplyScoreItem = false;
    [SerializeField] private float multiplyScore;

    //HIGHEST SCORE
    [SerializeField] private Text highestTotalScoreText;

    [Header("PANEL SCORES")]
    [Space(15)]
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private Text ScoresTextPanel;
    [SerializeField] private Text highestTotalScoreTextPanel;
    [SerializeField] private Text rankingScoreTextPanel;

    [Header("SOUND")]
    [Space(15)]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip button;
    [SerializeField] private AudioClip countReady;
    [SerializeField] private AudioClip countGO;
    [SerializeField] private AudioClip panelSound;
    [SerializeField] private AudioClip corrSound;
    [SerializeField] private AudioClip wrongSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        characterLimit();
    }

    public void characterLimit()
    {
        playerNameInput.characterLimit = 6;
    }

    void Update()
    {
        
    }

    public void GoToPage(int pageNum)
    {
        if (!pageAnim)
        {
            StartCoroutine("GoToPageE", pageNum);
        }
    }

    IEnumerator GoToPageE(int pageNum)
    {
        pageAnim = true;

        if (pageNum == 0)
        {

        }
        if (pageNum == 1)
        {
            ResetGame();
        }
        if (pageNum == 2)
        {
            StartCoroutine("GameStartE");
            Debug.Log("Start Game");
        }

        //BLACK TRANS START
        transPage.SetActive(true);
        transPage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1920);
        transPage.GetComponent<RectTransform>().DOAnchorPosY(0, 0.3f);
        yield return new WaitForSeconds(1);
        pages[pageNum].gameObject.SetActive(true);
        transPage.GetComponent<RectTransform>().DOAnchorPosY(-1920, 0.3f);
        pages[currentPage].gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        transPage.SetActive(false);

        currentPage = pageNum;
        pageAnim = false;
    }

    public void SubmitPlayerName()
    {
        string playerName = playerNameInput.text.Trim();
        if (playerName.Length > 0)
        {
            // Proceed to main menu
            OnPlayerNameEntered(playerName);
            StartCoroutine("GoToPageE", 1);
        }
        else
        {
            // Show error message
            StartCoroutine("errorMessageE");
        }
    }

    IEnumerator errorMessageE()
    {
        errorMessage.SetActive(true);
        yield return new WaitForSeconds(1);
        errorMessage.SetActive(false);
    }

    private void OnPlayerNameEntered(string enteredName)
    {
        bool playerExists = false;

        //Check if the player name already exists in the array
        for (int i = 0; i < playerInfoArray.Length; i++)
        {
            if (playerInfoArray[i].playerName == enteredName)
            {
                playerExists = true;
                currentPlayerIndex = i;
                currentPlayer = playerInfoArray[i];
                Debug.Log("Existing player loaded: " + currentPlayer.playerName);
                break;
            }
        }

        if (!playerExists)
        {
            //If the array is empty or the player does not exist, add new player data
            if (playerInfoArray == null || playerInfoArray.Length == 0)
            {
                playerInfoArray = new PlayerInfo[1];
                playerInfoArray[0] = new PlayerInfo
                {
                    playerName = enteredName,
                    highScore = 0,
                    itemAddTimeCount = itemTimeCount,
                    multiScoreCount = itemMultiCount,
                    ranking = 1
                };
                currentPlayerIndex = 0;
                currentPlayer = playerInfoArray[0];
                playerIndexArray = 1;
            }
            else
            {
                Array.Resize(ref playerInfoArray, playerInfoArray.Length + 1);
                playerInfoArray[playerInfoArray.Length - 1] = new PlayerInfo
                {
                    playerName = enteredName,
                    highScore = 0,
                    itemAddTimeCount = itemTimeCount,
                    multiScoreCount = itemMultiCount,
                    ranking = playerInfoArray.Length + 1
                };
                currentPlayerIndex = playerInfoArray.Length - 1;
                currentPlayer = playerInfoArray[currentPlayerIndex];
                playerIndexArray++;
            }

            Debug.Log("New player added: " + currentPlayer.playerName);
        }

        //Clear the input field
        playerNameInput.text = ""; 

        //Add to main menu profile
        playerNameText.text = currentPlayer.playerName;
        highScoresPlayerText.text = currentPlayer.highScore.ToString("0");

        //Set Current Player items
        itemTimeCountCurrent = currentPlayer.itemAddTimeCount;
        itemMultiCountCurrent = currentPlayer.multiScoreCount;

        //Update Current Player Data
        UpdateLeaderboard();
        UpdateCurrentPlayerData(); ;
    }

    public void InstructionPage()
    {
        instructionPage.SetActive(true);
        mainMenuPage.SetActive(false);
    }

    public void LeaderboardPage()
    {
        leaderboardPage.SetActive(true);
        mainMenuPage.SetActive(false);
    }

    public void PageBack()
    {
        instructionPage.SetActive(false);
        leaderboardPage.SetActive(false);
        mainMenuPage.SetActive(true);
    }

    public void activeItemTime()
    {
        if (addTimeItem == false && currentPlayer.itemAddTimeCount != 0)
        {
            itemTime.SetActive(true);
            itemMulti.SetActive(false);
            addTimeItem = true;
            multiplyScoreItem = false;

            currentPlayer.itemAddTimeCount -= 1;

            if (currentPlayer.multiScoreCount < itemMultiCountCurrent)
            {
                currentPlayer.multiScoreCount += 1;
            }
        }
        else
        {
            itemTime.SetActive(false);
            addTimeItem = false;
            multiplyScoreItem = false;

            if (isItemTimeCountZero == false)
            {
                currentPlayer.itemAddTimeCount += 1;
            }
        }

        itemTimeCountText.text = currentPlayer.itemAddTimeCount.ToString("0");
        itemMultiCountText.text = currentPlayer.multiScoreCount.ToString("0");
    }

    public void activeItemMulti()
    {
        if (multiplyScoreItem == false && currentPlayer.multiScoreCount != 0)
        {
            itemTime.SetActive(false);
            itemMulti.SetActive(true);
            addTimeItem = false;
            multiplyScoreItem = true;

            currentPlayer.multiScoreCount -= 1;

            if (currentPlayer.itemAddTimeCount < itemTimeCountCurrent)
            {
                currentPlayer.itemAddTimeCount += 1;
            }
        }
        else
        {
            itemMulti.SetActive(false);
            addTimeItem = false;
            multiplyScoreItem = false;

            if (isItemMultiCountZero == false)
            {
                currentPlayer.multiScoreCount += 1;
            }
        }
        itemTimeCountText.text = currentPlayer.itemAddTimeCount.ToString("0");
        itemMultiCountText.text = currentPlayer.multiScoreCount.ToString("0");
    }


    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void Pause()
    {
        if(Time.timeScale > 0)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            isPaused = true;
            source.PlayOneShot(panelSound);
        }
        else if(Time.timeScale == 0)
        {
            Time.timeScale = previousTimeScale;
            Debug.Log(Time.timeScale);
            isPaused = false;
            pausePanel.SetActive(false);
        }
    }

    IEnumerator GameStartE()
    {
        yield return new WaitForSeconds(1);

        //Countdown to Start Game 
        countdownPanel.gameObject.SetActive(true);
        countdownText.text = "Ready";
        source.PlayOneShot(countReady);
        yield return new WaitForSeconds(2);
        countdownText.text = "Go!!!";
        source.PlayOneShot(countGO);
        yield return new WaitForSeconds(1);
        gameIsReady = true;

        StartCoroutine("countdownGameE");
        countdownPanel.gameObject.SetActive(false);
    }

    IEnumerator countdownGameE()
    {
        if (addTimeItem == true)
        {
            timeLeft = totalTime + addTimeItemSeconds;
        }
        else
        {
            timeLeft = totalTime;
        }

        while (true)
        {
            minutes = Mathf.FloorToInt(timeLeft / 60);
            seconds = Mathf.FloorToInt(timeLeft % 60);
            countdownGameText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
            timeLeft -= 1 * Time.deltaTime;

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                GameEnd();
                Debug.Log("TIME OUT");
                break;
            }

            yield return null;
        }
    }

    public void UpdateScore(bool positive)
    {
        if (gameIsReady == true)
        {
            if (positive)
            {
                source.PlayOneShot(corrSound);

                if (multiplyScoreItem == true)
                {
                    multiplyScore = 1.5f;
                    int multiplyResult = (int)(1000 * multiplyScore);
                    scores += multiplyResult;
                }
                else
                {
                    scores += 1000;
                }
            }
            else
            {
                source.PlayOneShot(wrongSound);
                scores -= 500;
            }
        }

        if (scores <= 0)
        {
            scores = 0;
        }

        totalScores.text = "Score: " + scores.ToString("0");
    }

    public void GameEnd()
    {
        ScoresTextPanel.text = totalScores.text;
        holeShapes[DragCards.instance.holeShapesNum].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        gameIsReady = false;

        //SAVE OR REPLACE HIGHEST SCORE
        if (scores > currentPlayer.highScore)
        {
            currentPlayer.highScore = scores;
        }

        //SCORE
        scorePanel.SetActive(true);
        source.PlayOneShot(panelSound);
        ScoresTextPanel.text = "TOTAL SCORE: " + scores.ToString("0");
        highestTotalScoreText.text = "YOUR HIGHEST SCORE: " + currentPlayer.highScore.ToString("0");
        //highScoresPlayerText.text = currentPlayer.highScore.ToString("0");

        UpdateLeaderboard();
        UpdateCurrentPlayerData();
    }

    void ResetGame()
    {
        Time.timeScale = previousTimeScale;
        
        gameIsReady = false;
        scores = 0;

        scorePanel.SetActive(false);

        timeLeft = totalTime;
        minutes = Mathf.FloorToInt(timeLeft / 60);
        seconds = Mathf.FloorToInt(timeLeft % 60);
        countdownGameText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);

        itemTime.SetActive(false);
        itemMulti.SetActive(false);
        addTimeItem = false;
        multiplyScoreItem = false;

        pausePanel.SetActive(false);

        if (isPaused == true)
        {
            if (currentPlayer.multiScoreCount < itemMultiCountCurrent)
            {
                currentPlayer.multiScoreCount += 1;
            }

            if (currentPlayer.itemAddTimeCount < itemTimeCountCurrent)
            {
                currentPlayer.itemAddTimeCount += 1;
            }
            Debug.Log(currentPlayer.multiScoreCount);
            isPaused = false;
        }
        else
        {
            itemTimeCountCurrent = currentPlayer.itemAddTimeCount;
            itemMultiCountCurrent = currentPlayer.multiScoreCount;

            if (currentPlayer.itemAddTimeCount == 0)
            {
                isItemTimeCountZero = true;
            }

            if (currentPlayer.multiScoreCount == 0)
            {
                isItemMultiCountZero = true;
            }
        }

        itemTimeCountText.text = currentPlayer.itemAddTimeCount.ToString("0");
        itemMultiCountText.text = currentPlayer.multiScoreCount.ToString("0");

        totalScores.text = "Score: " + scores.ToString("0");

        resetCards();

        //RESET
        StopCoroutine("GameStartE");
        StopCoroutine("countdownGameE");
        Debug.Log("Reset");
    }

    public void resetCards()
    {
        cardListChange1[0].SetActive(true);
        cardListChange1[1].SetActive(false);
        cardListChange1[2].SetActive(false);

        cardListChange2[0].SetActive(true);
        cardListChange2[1].SetActive(false);
        cardListChange2[2].SetActive(false);

        cardListChange3[0].SetActive(true);
        cardListChange3[1].SetActive(false);
        cardListChange3[2].SetActive(false);
    }

    private void UpdateLeaderboard()
    {
        // Sort the playerInfoArray by highScore in descending order
        var sortedPlayers = playerInfoArray
            .OrderByDescending(player => player.highScore)
            .ThenBy(player => player.playerName) // To ensure deterministic order in case of ties
            .ToArray();

        // Update the leaderboard UI
        int rank = 1;
        int displayCount = 0;

        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            if (i == 0)
            {
                sortedPlayers[i].ranking = rank;
            }
            else
            {
                if (sortedPlayers[i].highScore == sortedPlayers[i - 1].highScore)
                {
                    sortedPlayers[i].ranking = sortedPlayers[i - 1].ranking;
                }
                else
                {
                    rank++;
                    sortedPlayers[i].ranking = rank;
                }
            }
        }


        for (int i = 0; i < 5; i++)
        {
            if (i < sortedPlayers.Length)
            {
                playerRankingGroup[i].text = sortedPlayers[i].ranking.ToString();
                playerRNameGroup[i].text = sortedPlayers[i].playerName;
                playerRScoreGroup[i].text = sortedPlayers[i].highScore.ToString();
                displayCount++;
            }
            else
            {
                playerRankingGroup[i].text = "";
                playerRNameGroup[i].text = "";
                playerRScoreGroup[i].text = "";
            }
        }

        // Update the original playerInfoArray to maintain the ranks
        for (int i = 0; i < playerInfoArray.Length; i++)
        {
            var player = playerInfoArray[i];
            player.ranking = sortedPlayers.First(p => p.playerName == player.playerName && p.highScore == player.highScore).ranking;
        }
    }


    private void UpdateCurrentPlayerData()
    {
        playerNameText.text = currentPlayer.playerName;
        highScoresPlayerText.text = currentPlayer.highScore.ToString();
        rankingPlayerText.text = currentPlayer.ranking.ToString();

        rankingScoreTextPanel.text = "RANKING: " + currentPlayer.ranking.ToString();
    }

    public void buttonSound()
    {
        source.PlayOneShot(button);
    }
}
