using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    public Dictionary<int, int> employeeWorkDict = new Dictionary<int, int>();
    public GameManager.GameState gameState;
    public PlayerController player;

    public Interactable bossOfficeDoor;
    public Interactable meetingRoomDoor;

    private List<Vector3> bossMoveToDuringIntro = new List<Vector3>
    {
        new Vector3(3, 1.8f, 3),
        new Vector3(42, 1.8f, 3),
        new Vector3(42, 1.8f, 23),
        new Vector3(28, 1.8f, 23),
    };

    private List<Vector3> bossMoveShowWork = new List<Vector3>
    {
        new Vector3(27, 1.8f, 40.5f)
    };

    private List<Vector3> moveToPeak = new List<Vector3>
    {
        new Vector3(31, 1.8f, 38)
    };

    public bool isMoving = false;

    private List<string> floorNames = new List<string>
    {
        "Ball Pit",
        "Bull pit",
        "Dog Pound",
        "Lions Den",
        "Factory Floor",
        "Sweat Shop",
        "Bicep House",
        "Octagon",
        "Symmantics Dome",
        "Dragon's Cave",
        "Scenic Overlook",
        "Holy Site",
        "Cut in the Woods",
        "Grindhouse",
        "Shadow Realm",

    };

    // Start is called before the first frame update
    void Start()
    {
        Coworker.workDone += AddWorkDone;
        PlayerController.workDone += AddWorkDone;
        GameManager.SetGameStateAction += SetGameState;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1);
        }
    }

    private void AddWorkDone(int id)
    {
        if (!employeeWorkDict.ContainsKey(id))
        {
            employeeWorkDict.Add(id, 0);
        }
        employeeWorkDict[id] += 1;
    }

    public void SetGameState(GameManager.GameState state)
    {
        gameState = state;
        if (state == GameManager.GameState.FollowBoss)
        {
            IEnumerator cr = MoveBoss(bossMoveToDuringIntro);
            StartCoroutine(cr);
        }
        if (state == GameManager.GameState.BossShowingHowToWork)
        {
            IEnumerator cr = MoveBoss(bossMoveShowWork);
            StartCoroutine(cr);
        }
        if (state == GameManager.GameState.IntroduceToCoworkers)
        {
            IEnumerator cr = MoveBoss(moveToPeak);
            StartCoroutine(cr);
            CrackOfficeDoor();
        }
        if (state == GameManager.GameState.MeetingStarts)
        {

        }
    }

    public string ToEarlyText()
    {
        return "Who are you?  Please speak with the front desk";
    }

    public List<string> IntroduceToBoss(int id)
    {
        return new List<string> { "Hi emplyee " + id.ToString(), "Welcome to your new family", "Here, we work hard, play hard", "You already know what we do, so follow me" };
    }

    public List<string> ShowHowToWork()
    {
        return new List<string> {"As you know, you'll be entering data using cranial inputation", "Try for yourself by sitting down", "Then press E on your workstation"};
    }

    public string DontMindMe()
    {
        return "Don't mind me, get back to work!";
    }

    public List<string> ExplainFloor()
    {
        int count = floorNames.Count - 1;
        List<string> randFloorNames = new List<string>();
        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, floorNames.Count - 1);
            randFloorNames.Add("The " + floorNames[rand]);
            floorNames.RemoveAt(rand);
        }

        List<string>  returnString = new List<string> { "This is where all the magic happens", "This is the main floor", "We've got a couple nicknames for it though" };
        returnString.AddRange(randFloorNames);
        returnString.Add(randFloorNames[2]);
        returnString.Add("Did I say that one already? Haha!");
        returnString.Add("We like to have fun here");
        returnString.Add("This is your workstation over here");
        returnString.Add("I'll show you what you'll be doing");
        return returnString;
    }

    IEnumerator MoveBoss(List<Vector3> positions)
    {
        isMoving = true;
        float moveSpeed = 10f;
        foreach (Vector3 position in positions)
        {
            isMoving = true;
            Quaternion targetRotation = Quaternion.LookRotation(position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1);
            Vector3 direction = transform.position - position;
            direction = direction.normalized;
            while (isMoving)
            {
                transform.Translate(Vector3.forward * Mathf.Min(moveSpeed * Time.deltaTime, Vector3.Distance(transform.position, position)));
                if (Vector3.Distance(transform.position, position) < 0.1)
                {
                    transform.position = position;
                    isMoving = false;
                }
                yield return null;
            }
        }
    }

    private void CrackOfficeDoor()
    {
        bossOfficeDoor.transform.Rotate(new Vector3(0, 1, 0), -30);
    }

    public void OpenDoorsForMeeting()
    {
        bossOfficeDoor.transform.Rotate(new Vector3(0, 1, 0), 165);
        meetingRoomDoor.transform.Rotate(new Vector3(0, 1, 0), -135);
    }
}
