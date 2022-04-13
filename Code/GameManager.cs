using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Animator animUI, animPlayer;
    Player pl;
    Score sc;

    public GameObject mapObject, goalObject, arrowObject, pauseButtonObject, pauseMenuObject, introPanelObject, fallenHeroObject; 
    public int levelId, currentPhase = 0;
    private bool tutorialPlayed = false;
    public float fallTime;

    [System.Serializable]
    public class Escalator
    {
        public int escalatorLevel;
        public string escalatorType;
        public GameObject escalatorObject, startTarget, endTargert;
    }

    public List<Escalator> escalatorList = new List<Escalator>();

    void Start()
    {
        Time.timeScale = 1f;

        pl = FindObjectOfType<Player>();
        sc = this.GetComponent<Score>();
        animUI = this.GetComponent<Animator>();

        animPlayer = pl.GetComponent<Animator>();

        IntroPhase();
    }
    void Update()
    {
        if(currentPhase == 3)
        {
            fallTime += Time.deltaTime;

            if(fallTime > 7.5f)
            {
                FailPhase();
                fallTime = 0;
            }
        }
    }
    public void IntroPhase()
    {
        animUI.Play("Intro", 0);
        animPlayer.Play("Intro", 0);

        sc.UpdateScoreText(0,PlayerPrefs.GetInt("HighScore"));
    }

    public void PreparationPhase()
    {
        currentPhase = 0;
        fallTime = 0;

        levelId = Random.Range(0, 6);

        animUI.Play("Start", 0);
        animPlayer.Play("MoveUp", 0);

        introPanelObject.SetActive(false);
        pauseButtonObject.SetActive(false);
        fallenHeroObject.SetActive(false);

        pl.endTargetLocation = escalatorList[levelId].endTargert;
        pl.GetComponent<Rigidbody>().isKinematic = true;
        pl.transform.position = new Vector3(0f, 7.5f, 0f);
        pl.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));

        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

        SpawnGoal();
        ArrowPosition();
    }
    void TutorialPhase()
    {
        currentPhase = 1;

        Camera.main.transform.position = Vector3.zero;
        pl.transform.position = escalatorList[levelId].startTarget.transform.position;

        arrowObject.transform.position = new Vector3(0, -10, 0);

        if (levelId % 2 == 0)
        {
            pl.transform.rotation = escalatorList[levelId].escalatorObject.transform.rotation;
        }
        else
        {
            pl.transform.rotation = Quaternion.Euler(new Vector3(0, 180 + escalatorList[levelId].escalatorObject.transform.rotation.eulerAngles.y, 0));
        }

        if (!tutorialPlayed)
        {
            animUI.Play("Tutorial");
            tutorialPlayed = true;
        }
        else
        {
            MovePhase();
        }

    }
    void MovePhase()
    {
        currentPhase = 2;

        pauseButtonObject.SetActive(true);

    }
    public void PlayMovingCamera()
    {
        pl.gameObject.transform.position = new Vector3(0, 0, 0);
        pl.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        pl.GetComponent<Rigidbody>().isKinematic = true;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Finish"))
        {
            Destroy(go);
        }

        animPlayer.Play("Moving", 0);
    }

    void ArrowPosition()
    {
        arrowObject.transform.position = new Vector3
        (
            escalatorList[levelId].startTarget.transform.position.x / Mathf.Abs(escalatorList[levelId].startTarget.transform.position.x) * 4,
            escalatorList[levelId].startTarget.transform.position.y,
            escalatorList[levelId].startTarget.transform.position.z / Mathf.Abs(escalatorList[levelId].startTarget.transform.position.z) * 4);
            arrowObject.transform.LookAt(escalatorList[levelId].startTarget.transform
        );
    }
    void SpawnGoal()
    {

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Finish"))
        {
            Destroy(go);
        }

        GameObject goal = Instantiate(goalObject, new Vector3(Random.Range(-4, 4f), -1.4f, Random.Range(-3.5f, 3.5f)), Quaternion.identity);
        goal.transform.parent = mapObject.transform;
    }
    public void ScorePhase()
    {
        currentPhase = 4;
        animUI.Play("Score", 0);
    }
    public void FailPhase()
    {
        currentPhase = 4;

        sc.UpdateScoreText(1,sc.GetScore());
        fallenHeroObject.SetActive(true);
        pauseButtonObject.SetActive(false);
        animUI.Play("Fail", 0);

    }
    public void PauseMenu()
    {
        pauseButtonObject.SetActive(false);
        pauseMenuObject.SetActive(true);

        Time.timeScale = 0f;
    }
    public void ResumeMenu()
    {
        pauseButtonObject.SetActive(true);
        pauseMenuObject.SetActive(false);

        Time.timeScale = 1f;
    }
    public void ResetPhase()
    {
        pauseMenuObject.SetActive(false);
        sc.ClearScore();
        PreparationPhase();
        
        Time.timeScale = 1f;
    }
}