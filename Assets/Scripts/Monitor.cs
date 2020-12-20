using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : MonoBehaviour
{
    public List<Material> workMaterialList;
    private MeshRenderer meshRenderer;

    //private void Start()
    //{
    //    meshRenderer = GetComponent<MeshRenderer>();
    //    IEnumerator cr = parseMaterialstoScreen();
    //    StartCoroutine(cr);
    //}

    //IEnumerator parseMaterialstoScreen()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(Random.Range(0, 150) / 100);
    //        foreach (Material workMaterial in workMaterialList)
    //        {
    //            Material[] newMaterials = new Material[meshRenderer.materials.Length];
    //            newMaterials[0] = workMaterial;
    //            newMaterials[1] = meshRenderer.materials[1];
    //            newMaterials[2] = meshRenderer.materials[2];
    //            meshRenderer.materials = newMaterials;
    //            yield return new WaitForSeconds(0.5f);
    //        }
    //    }
    //}
}
