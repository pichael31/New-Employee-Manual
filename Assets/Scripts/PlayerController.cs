using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement
    public CharacterController controller;
    public FaceCanvas faceCanvas;
    public GameManager gameManager;
    public float moveSpeed = 10f;
    public int playerID;
    public GameManager.GameState gameState;
    Camera cam;
    public bool isSitting = false;
    public bool isBashing = false;
    public Interactable focus;
    private int countWorkDone = 0;

    public delegate void WorkDoneEventHandler(int id);
    public static event WorkDoneEventHandler workDone;

    private List<List<string>> firstTimeBashingString = new List <List<string>>
    {
        new List<string>{"Nice!  Just Like that!"},
        new List<string>{"Wow!  You're really getting the hang of this"},
        new List<string>{"One More"},
        new List<string>{"Well done.  Why don't you introduce yourself to your coworkers.", "Make sure to talk to all of them.", "We really got some characters working here",  "After that, try to knock at least 10 more jobs out before lunch", "Then we have our daily post lunch meeting"}
    };

    public Dictionary<string, System.Action> intereractDictionary = new Dictionary<string, System.Action>()
    {
    };

    public void CreateInteractionDict()
    {
        intereractDictionary.Add("Chair", SitDown);
        intereractDictionary.Add("Keyboard", BashFaceAgainstKeyboard);
        intereractDictionary.Add("Monitor", BashFaceAgainstKeyboard);
        intereractDictionary.Add("", StandUp);
        intereractDictionary.Add("Door", OpenDoor);
        intereractDictionary.Add("Coworker", TalkToCoworker);
        intereractDictionary.Add("Front Desk Person", BeIntroduced);
        intereractDictionary.Add("Boss", TalkToBoss);
        intereractDictionary.Add("Fridge", EatLunch);
    }

    public string interactableText()
    {
        string interactType;
        if (focus != null)
        {
            interactType = focus.interactType;
        }
        else
        {
            interactType = "";
        }

        if (interactType == "Chair")
        {
            if (!isSitting)
            {
                if (!focus.isBeingUsed)
                {
                    return "Sit Down";
                }
            }
        }

        if (interactType == "Keyboard" || interactType == "Monitor")
        {
            if (isSitting)
            {
                return "Do Work";
            }
        }

        if (interactType == "")
        {
            if (isSitting)
            {
                if (gameState == GameManager.GameState.MeetingStarts)
                {
                    return "Stay Awake";
                }
                if (cam.GetComponent<MouseLook>().notActioning)
                {
                    return "Stand Up";
                }
            }
        }

        if (interactType == "Door")
        {
            if (!focus.isCurrentlyUnuseable)
            {
                if (focus.isBeingUsed)
                {
                    return "Close Door";
                }
                else
                {
                    return "Open Door";
                }
            }
        }

        if (interactType == "Coworker")
        {
            return "Talk to employee " + focus.GetComponentInParent<Coworker>().employeeID.ToString();
        }

        if (interactType == "Front Desk Person")
        {
            return "Talk to employee " + focus.GetComponentInParent<FrontDeskPerson>().employeeID.ToString();
        }

        if (interactType == "Boss")
        {
            return "Talk to Boss";
        }

        else if (interactType == "Fridge")
        {
            if (gameState == GameManager.GameState.Lunch)
            {
                return "Eat someone's lunch";
            }
            else
            {
                return "Eat snack";
            }
        }
        return "";
    }


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
        CreateInteractionDict();
        playerID = Random.Range(10000, 99999);
        GameManager.SetGameStateAction += SetGameState;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameManager.MoveCoworkersToMeeetingRoom();
            gameManager.SetState(GameManager.GameState.MeetingStarts);
        }
        if (gameState == GameManager.GameState.MeetingStarts && isSitting)
        {
            FallAsleep();
        }

        if (!isSitting && !faceCanvas.isListening)
        {
            MovePlayer();
        }
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        UnsetFocus();
        if (Physics.Raycast(ray, out hit, 4))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                SetFocus(interactable);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isBashing && !faceCanvas.isListening)
            {
                Interact();
            }
        }

        if (transform.position.y > 1.8)
        {
            transform.position = new Vector3(transform.position.x, 1.8f, transform.position.z);
        }
    }

    private void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void Interact()
    {
        if (interactableText() != "")
        {
            if (focus != null)
            {
                intereractDictionary[focus.interactType]();
            }
            else
            {
                intereractDictionary[""]();
            }
        }
    }

    void SetFocus(Interactable newFocus)
    {
        focus = newFocus;
        faceCanvas.SetInteractableText(interactableText());
    }

    void UnsetFocus()
    {
        focus = null;
        faceCanvas.SetInteractableText(interactableText());
    }

    void SitDown()
    {
        isSitting = true;
        transform.position = new Vector3(focus.transform.position.x, 0.9f, focus.transform.position.z);
    }

    void StandUp()
    {
        if (gameState != GameManager.GameState.MeetingStarts)
        {
            isSitting = false;
            transform.position = new Vector3(transform.position.x, 1.8f, transform.position.z);
            Vector3 move = transform.right * 0.1f + transform.forward * 0.1f;
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
        else
        {
            WakeBackUp();
        }
    }

    void EatLunch()
    {
        if (gameState == GameManager.GameState.Lunch)
        {
            gameManager.SetState(GameManager.GameState.MeetingStarts);
            gameManager.MoveCoworkersToMeeetingRoom();
        }
    }

    void OpenDoor()
    {
        if (!focus.isCurrentlyUnuseable)
        {
            if (!focus.isBeingUsed)
            {
                focus.transform.Rotate(new Vector3(0, 1, 0), 135f);
                focus.isBeingUsed = true;
            }
            else
            {
                focus.transform.Rotate(new Vector3(0, 1, 0), -135f);
                focus.isBeingUsed = false;
            }
        }
    }

    void TalkToCoworker()
    {
        faceCanvas.SetTalkingText(focus.GetComponentInParent<Coworker>().GetInteractionText(), GameManager.GameState.Null);
        if (gameState == GameManager.GameState.IntroduceToCoworkers)
        {
            gameManager.TalkedToCoworker(focus.GetComponentInParent<Coworker>().employeeID);
        }
    }

    void BeIntroduced()
    {
        faceCanvas.SetListTalkingText(focus.GetComponentInParent<FrontDeskPerson>().GetInteractionText(playerID), GameManager.GameState.IntroduceToBoss);
    }

    void TalkToBoss()
    {
        Boss boss = focus.GetComponentInParent<Boss>();
        if (!boss.isMoving)
        {
            if (gameState == GameManager.GameState.Start)
            {
                faceCanvas.SetTalkingText(boss.ToEarlyText(), GameManager.GameState.Null);
            }
            else if (gameState == GameManager.GameState.IntroduceToBoss)
            {
                faceCanvas.SetListTalkingText(boss.IntroduceToBoss(playerID), GameManager.GameState.FollowBoss);
            }
            else if (gameState == GameManager.GameState.FollowBoss)
            {
                faceCanvas.SetListTalkingText(boss.ExplainFloor(), GameManager.GameState.BossShowingHowToWork);
            }
            else if (gameState == GameManager.GameState.BossShowingHowToWork)
            {
                faceCanvas.SetListTalkingText(boss.ShowHowToWork(), GameManager.GameState.Null);
            }
            else
            {
                faceCanvas.SetTalkingText(boss.DontMindMe(), GameManager.GameState.Null);
            }
        }
    }

    void BashFaceAgainstKeyboard()
    {
        if (isSitting)
        {
            StartCoroutine("BashFace");
        }
    }

    IEnumerator BashFace()
    {
        Transform startTransform = transform;
        isBashing = true;
        cam.GetComponent<MouseLook>().notActioning = false;
        float startDistance = Vector3.Distance(transform.position, focus.transform.position);
        bool partOneFinished = false;
        float angle = 0;
        float bashSpeed = 10f * Time.fixedDeltaTime;
        while (!partOneFinished)
        {
            controller.transform.Rotate(new Vector3(1, 0, 0), bashSpeed);
            angle += bashSpeed;
            if (angle >= 55)
            {
                partOneFinished = true;
            }
            yield return null;
        }
        bool partTwoFinished = false;
        workDone?.Invoke(playerID);
        while (!partTwoFinished)
        {
            controller.transform.Rotate(new Vector3(1, 0, 0), -bashSpeed);
            angle -= bashSpeed;

            if (angle <= 0)
            {
                partTwoFinished = true;
            }
            yield return null;
        }
        isBashing = false;
        cam.GetComponent<MouseLook>().notActioning = true;
        countWorkDone += 1;
        if (countWorkDone >= 14 && gameState == GameManager.GameState.WorkBySelf)
        {
            gameManager.SetState(GameManager.GameState.Lunch);
        }
        if (gameState == GameManager.GameState.BossShowingHowToWork)
        {
            listenToBossShowWork();
        }
    }

    private void listenToBossShowWork()
    {
        if (firstTimeBashingString.Count > 1)
        {
            faceCanvas.SetListTalkingText(firstTimeBashingString[0], GameManager.GameState.Null);
        }
        else
        {
            faceCanvas.SetListTalkingText(firstTimeBashingString[0], GameManager.GameState.IntroduceToCoworkers);
        }
        firstTimeBashingString.RemoveAt(0);
    }

    public void SetGameState(GameManager.GameState state)
    {
        gameState = state;
    }

    private void FallAsleep()
    {
        float transparency = faceCanvas.canvasColor.color.a;
        faceCanvas.canvasColor.color = new Color(0, 0, 0, transparency + 0.03f * Time.deltaTime);
        transform.Rotate(new Vector3(1, 0, 0), 3f * Time.deltaTime);
    }

    private void WakeBackUp()
    {
        float transparency = faceCanvas.canvasColor.color.a;
        faceCanvas.canvasColor.color = new Color(0, 0, 0, Mathf.Max(transparency - 0.5f * Time.deltaTime, 0));
        Vector3 rotateTo = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotateTo), 0.8f);
    }
}
