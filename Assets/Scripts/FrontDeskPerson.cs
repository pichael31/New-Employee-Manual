using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDeskPerson : MonoBehaviour
{

    public int employeeID;
    private bool hasInteractedWith = false;

    public delegate void FrontDeskInteractedWith();
    public static event FrontDeskInteractedWith frontDeskInteractedWith;

    // Start is called before the first frame update
    void Start()
    {
        employeeID = Random.Range(10000, 99999);
    }

    public List<string> GetInteractionText(int playerID)
    {
        if (!hasInteractedWith)
        {
            hasInteractedWith = true;
            frontDeskInteractedWith?.Invoke();
            return new List<string> { "Welcome!  You must be the new employee, " + playerID.ToString() + "!", "We're really excited to bring you on board!", "The Boss should be out soon to help you", "Ah! There he is behind you!" };
        }
        else
        {
            return new List<string> { "Hi " + playerID.ToString() + "!  Hope your first day is goin well!" };
        }
    }
}
