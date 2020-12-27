using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coworker : MonoBehaviour
{

    public delegate void WorkDoneEventHandler(int id);
    public static event WorkDoneEventHandler workDone;

    public int employeeID;
    public bool isSitting = false;
    public bool isSittingAtDesk = false;
    public bool isWorking = false;

    public List<string> possibleInteractions = new List<string>
    {
        "That's some weather out there, huh!",
        "That's some weather out there, huh!",
        "That's some weather out there, huh!",
        "I can't believe what time it is already!",
        "I can't believe what time it is already!",
        "I can't believe what time it is already!",
        "I can't believe what time it is already!",
        "Big game last night!",
        "Big game last night!",
        "So much work to do today!",
        "So much work to do today!",
        "So much work to do today!",
        "My wife cheated on me and I'm dead inside!",
        "Glad to have you on board",
        "Glad to have you on board",
        "We should grab a beer after work",
        "We should grab a beer after work",
        "This work isn't that bad",
        "Don't worry, you'll get used to it",
        "I hate this job so much, but I can't quit",
        "I'll probably stay late today, just like normal",
        "I'll probably stay late today, just like normal",
        "I'll probably stay late today, just like normal",
        "I'll probably stay late today, just like normal",
        "Wow!  Look at the time!",
        "Sorry, I'm a little busy right now",
        "Sorry, I'm a little busy right now",
        "Can't talk, going for a new record today!",
        "Can't talk, going for a new record today!",
        "This work really helps the day go by",
        "This work really helps the day go by",
        "You're luck to join a team that has a much fun as us!",
        "You're luck to join a team that has a much fun as us!",
        "You really learn to love this job",
        "You really learn to love this job",
        "You really learn to love this job",
        "Who needs work-life balance when work is your life!",
        "Who needs work-life balance when work is your life!",
        "Who needs work-life balance when work is your life!",
    };

    private void Awake()
    {
        employeeID = Random.Range(10000, 99999);
        IEnumerator cr = BashFace();
        StartCoroutine(cr);
    }

    private void Update()
    {

    }

    IEnumerator BashFace()
    {
        while (true)
        {
            float delay = Random.Range(50, 250) / 100;
            isWorking = true;
            yield return new WaitForSeconds(delay);
            Transform startTransform = transform;
            bool partOneFinished = false;
            float angle = 0;
            float bashSpeed = 50f * Time.fixedDeltaTime;
            while (!partOneFinished)
            {
                transform.Rotate(new Vector3(0, 0, 1), bashSpeed);
                angle += bashSpeed;
                if (angle >= 75)
                {
                    partOneFinished = true;
                }
                yield return null;
            }
            bool partTwoFinished = false;
            workDone?.Invoke(employeeID);
            while (!partTwoFinished)
            {
                transform.Rotate(new Vector3(0, 0, 1), -bashSpeed);
                angle -= bashSpeed;

                if (angle <= 0)
                {
                    partTwoFinished = true;
                    transform.Rotate(new Vector3(0, 0, 1), -angle);
                }
                yield return null;
            }
            isWorking = false;
        }
    }

    public void SitDown(Transform chair)
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.position = new Vector3(chair.position.x + 0.4f, 2.1f, chair.position.z);
        transform.Rotate(new Vector3(0, 180, 0));
        chair.GetComponent<Interactable>().isBeingUsed = true;
        isSittingAtDesk = true;
        isSitting = true;
    }

    public string GetInteractionText()
    {
        return possibleInteractions[Random.Range(0, possibleInteractions.Count)];
    }

    public void MoveToMeetingRoom(Transform chair)
    {
        transform.position = new Vector3(chair.position.x, 2.1f, chair.position.z);
        transform.rotation = chair.rotation;
        transform.Rotate(new Vector3(0, 1, 0), -90);
        StopAllCoroutines();
    }
}
