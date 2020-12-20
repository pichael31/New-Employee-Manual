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
}
