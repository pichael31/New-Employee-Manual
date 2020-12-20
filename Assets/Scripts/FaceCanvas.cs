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

    public bool isListening = false;

    private List<string> thingsToHear = new List<string>();

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

    public void SetListTalkingText(List<string> futureTalkingText, GameManager.GameState nextState)
    {
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
        isListening = true;
        yield return new WaitForSeconds(0.3f);
        ResetTalkingText();
    }

    private void Start()
    {
        GameManager.SetGameStateAction += SetMissionText;
        SetMissionText(GameManager.GameState.Start);
    }
}
