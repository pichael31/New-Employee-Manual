using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkDoneWall : MonoBehaviour
{
    public Text textWall;
    public int numberWorkDone = 0;

    private void Start()
    {
        Coworker.workDone += addWorkDone;
        PlayerController.workDone += addWorkDone;
    }

    private void addWorkDone(int id)
    {
        numberWorkDone += 1;
        //textWall.text = numberWorkDone.ToString();
    }
}
