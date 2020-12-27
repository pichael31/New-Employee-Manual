using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monitor : MonoBehaviour
{
    public List<Sprite> workImageList;
    private Image monitorImage;

    private void Start()
    {
        monitorImage = GetComponentInChildren<Image>();
        IEnumerator cr = parseImagestoScreen();
        StartCoroutine(cr);
    }

    IEnumerator parseImagestoScreen()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, 150) / 100);
            foreach (Sprite workImage in workImageList)
            {
                monitorImage.sprite = workImage;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
