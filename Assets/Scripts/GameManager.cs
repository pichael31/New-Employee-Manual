using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public Boss boss;
    public List<GameObject> coworkerDesks;
    public List<GameObject> meetingRoomDesks;
    public List<Coworker> coworkers;
    public GameObject coworkerPrefab;

    public List<int> coworkersToTalkTo = new List<int>();

    public delegate void SetGameState(GameState state);
    public static event SetGameState SetGameStateAction;

    public enum GameState
    {
        Null,
        Start,
        IntroduceToBoss,
        FollowBoss,
        BossShowingHowToWork,
        IntroduceToCoworkers,
        WorkBySelf,
        Lunch,
        MeetingStarts,
        GetCoffee,
        ActualMeeting,
        MeetingFinished,
        WorkFinished,
        GameAlmostOver,
        GameOver,
    }

    public GameState gameState = GameState.Start;


    // Start is called before the first frame update
    void Start()
    {
        CreateCoworkers();
        FrontDeskPerson.frontDeskInteractedWith += TalkToFrontDesk;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void SetState(GameState state)
    {
        SetGameStateAction?.Invoke(state);
        gameState = state;
    }

    private void CreateCoworkers()
    {
        foreach (GameObject desk in coworkerDesks)
        {
            foreach (Transform deskPart in desk.transform)
            {
                if (deskPart.tag == "Chair")
                {
                    GameObject coworkerObject = Instantiate(coworkerPrefab);
                    Coworker coworker = coworkerObject.GetComponent<Coworker>();
                    coworkersToTalkTo.Add(coworker.employeeID);
                    coworker.SitDown(deskPart);
                    coworkers.Add(coworker);
                }
            }
        }
    }

    public void TalkedToCoworker(int id)
    {
        if (coworkersToTalkTo.Contains(id))
        {
            coworkersToTalkTo.Remove(id);
        }
        if (coworkersToTalkTo.Count == 0)
        {
            SetState(GameState.WorkBySelf);
        }
    }

    public void TalkToFrontDesk()
    {
        boss.transform.position = new Vector3(3, 1.8f, 15);
        boss.transform.Rotate(new Vector3(0, 180, 0));
    }

    public void MoveCoworkersToMeeetingRoom()
    {
        for (int i = 0; i <= coworkers.Count; i++)
        {
            Transform chair = meetingRoomDesks[i].transform;
            if (i == coworkers.Count)
            {
                boss.transform.position = new Vector3(chair.position.x, 2.1f, chair.position.z);
                boss.transform.localScale.Set(1.2f, 1.2f, 1.2f);
                boss.OpenDoorsForMeeting();
            }
            else
            {
                coworkers[i].MoveToMeetingRoom(chair);
            }
        }
    }

    IEnumerator FallAsleepThenWakeUp()
    {
        float time = 0;
        while (player.faceCanvas.canvasColor.color.a < 1 && time < 15 * 3)
        {
            time += Time.deltaTime;
            yield return null;
        }
        float startSpeed = 1f;
        while (player.faceCanvas.canvasColor.color.a < 1)
        {
            player.FallAsleep(startSpeed);
            startSpeed += 0.5f;
        }
        player.faceCanvas.StopAllCoroutines();
        player.faceCanvas.thingsToHear = new List<string>();
        player.faceCanvas.ResetTalkingText();
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < coworkers.Count; i++)
        {
            GameObject desk = coworkerDesks[i];
            foreach (Transform deskPart in desk.transform)
            {
                if (deskPart.tag == "Chair")
                {
                    coworkers[i].SitDown(deskPart);
                    coworkers[i].StartCoroutine("BashFace");
                }
            }
        }
        boss.transform.position = new Vector3(31, 1.8f, 38);
        boss.transform.localScale.Set(1.2f, 1.8f, 1.2f);
        if (gameState != GameState.MeetingFinished)
        {
            SetState(GameState.MeetingFinished);
        }
        Vector3 rotateTo = new Vector3(0, player.transform.rotation.eulerAngles.y, 0);
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(rotateTo), 1);
        float transparency = player.faceCanvas.canvasColor.color.a;
        while (transparency > 0)
        {
            transparency -= 0.5f;
            player.faceCanvas.canvasColor.color = new Color(0, 0, 0, transparency);
        }
    }
}
