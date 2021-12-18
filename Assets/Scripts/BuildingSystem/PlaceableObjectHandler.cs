using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObjectHandler : MonoBehaviour
{
    public List<Coordinate> myCoordinates;
    private MeshRenderer[] myMRs;
    private bool isPreview;
    private List<Material> baseMaterials = new List<Material>();

    private void OnEnable()
    {
        isPreview = true;
        myMRs = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        int i = 0;
        foreach (MeshRenderer mR in myMRs)
        {
            baseMaterials.Add(mR.sharedMaterial);
            MakeTransparent(true, mR, i);
            i++;
        }
    }

    private void MakeTransparent(bool transperent, MeshRenderer mR, int index)
    {
        Material mat = new Material(mR.material);
        Color color = mR.material.color;
        if (transperent)
        {
            color.a = 0.5f;
            mat.SetColor("_Color", color);
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
            mR.sharedMaterial = mat;
        }
        else
        {
            mR.sharedMaterial = baseMaterials[index];
        }

    }

    public void ToogleErrorVisual(bool error)
    {
        Color color;
        if (error)
        {
            color = Color.red;
        }
        else
        {
            color = Color.white;
        }

        if (isPreview)
        {
            color.a = 0.5f;
        }
        else
        {
            color.a = 1f;
        }

        foreach (MeshRenderer mR in myMRs)
        {
            mR.material.color = color;
        }
    }

    public void UnsetTransparency()
    {
        isPreview = false;
        int i = 0;
        foreach (MeshRenderer mR in myMRs)
        {
            MakeTransparent(false, mR, i);
            i++;
        }
    }

    public void OnBuild()
    {
        //TODO Implement Audio and Build Animation
    }

    private void OnDestroy()
    {
        //TODO Implement Audio and Destory Animation
    }
}
