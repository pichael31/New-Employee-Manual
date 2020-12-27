using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FaceCanvas : MonoBehaviour
{
    public Text interactableText;
    public Text talkingText;
    public TextMeshProUGUI missionTMP;
    public Image canvasColor;
    public Text introText;

    public bool isListening = false;

    public List<string> thingsToHear = new List<string>();

    public GameManager gameManager;
    private GameManager.GameState nextGameState;

    public Dictionary<GameManager.GameState, string> missioTextDict = new Dictionary<GameManager.GameState, string>
    {
        {GameManager.GameState.Start, "Talk To Front Desk" },
        {GameManager.GameState.IntroduceToBoss, "Talk To Boss" },
        {GameManager.GameState.FollowBoss, "Follow Boss" },
        {GameManager.GameState.BossShowingHowToWork, "Listen To Boss Again" },
        {GameManager.GameState.IntroduceToCoworkers, "Meet All Your New Coworkers" },
        {GameManager.GameState.WorkBySelf, "Do Work" },
        {GameManager.GameState.Lunch, "Eat someone's Lunch" },
        {GameManager.GameState.MeetingStarts, "Attend Daily Meeting" },
        {GameManager.GameState.MeetingFinished, "Finish Work I Guess" },
        {GameManager.GameState.WorkFinished, "Head Home" },
        {GameManager.GameState.GameAlmostOver, "" },
        {GameManager.GameState.GameOver, "" },
    };

    public List<string> missionStringList = new List<string>();

    public void SetInteractableText(string text)
    {
        interactableText.text = text;
    }

    public void ResetInteractableText()
    {
        interactableText.text = "";
    }

    public void SetTalkingText(string text, GameManager.GameState nextState)
    {
        isListening = true;
        IEnumerator cr = ShowMessage();
        StopCoroutine(cr);
        if (nextState != GameManager.GameState.Null)
        {
            nextGameState = nextState;
        }
        thingsToHear.Add(text);
        talkingText.text = thingsToHear[0];
        thingsToHear.RemoveAt(0);
        StartCoroutine(cr);
    }

    public void NextTalkingText()
    {
        IEnumerator cr = ShowMessage();
        talkingText.text = thingsToHear[0];
        thingsToHear.RemoveAt(0);
        StartCoroutine(cr);
    }

    public void SetListTalkingText(List<string> futureTalkingText, GameManager.GameState nextState = GameManager.GameState.Null, bool canInteract = false)
    {
        if (!canInteract)
        {
            isListening = true;
        }
        if (nextState != GameManager.GameState.Null)
        {
            nextGameState = nextState;
        }
        thingsToHear.AddRange(futureTalkingText);
        NextTalkingText();
    }

    public void ResetTalkingText()
    {
        if (thingsToHear.Count == 0)
        {
            isListening = false;
            talkingText.text = "";
            if (nextGameState != GameManager.GameState.Null)
            {
                gameManager.SetState(nextGameState);
                nextGameState = GameManager.GameState.Null;
            }
        }
        else
        {
            NextTalkingText();
        }
    }

    public void SetMissionText(GameManager.GameState newState)
    {
        string text = "Missions:";
        foreach(string t in missionStringList)
        {
            text += "\n<s>" + t + "</s>";
        }
        text += "\n<b>" + missioTextDict[newState] + "</b>";
        missionStringList.Add(missioTextDict[newState]);
        missionTMP.text = text;
    }

    IEnumerator ShowMessage()
    {
        yield return new WaitForSeconds(3f);
        ResetTalkingText();
    }

    private void Start()
    {
        StartCoroutine("Intro");
        GameManager.SetGameStateAction += SetMissionText;
        SetMissionText(GameManager.GameState.Start);
    }

    IEnumerator Intro()
    {
        canvasColor.color = new Color(0, 0, 0, 1);
        introText.text = "Congratulations on starting your first day at your new job";
        yield return new WaitForSeconds(5);

        introText.text = "Press WASD to move, E to interact";
        yield return new WaitForSeconds(5);

        introText.text = ".";
        GetComponent<AudioSource>().Play();
        while (canvasColor.color.a > 0)
        {
            float a = canvasColor.color.a;
            canvasColor.color = new Color(0, 0, 0, a - 1f * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}
