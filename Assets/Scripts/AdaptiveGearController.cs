using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AdaptiveGearController : MonoBehaviour
{

    public int teeth = 8;

    [SerializeField] private Sprite toothSprite; //Sprite storage for tooth of Gear
    private float toothScaleFactor = 0.012f; //scale factor for the teeth
    [SerializeField] private GameObject toothHolder; //GameObject for holding all of the Teeth generated
    [SerializeField] private Image gearCenterSprite; //holder for the center of Gear sprite

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RecalcImage();   
    }

    //Regenerate the Gear and its teeth whenever something is changed.
    private void RecalcImage()
    {
        //purge all old teeth from the gameobject
        for(int i = toothHolder.transform.childCount -1; i >= 0; i--) //go backwards, to ensure all are destroyed 
        {
            if (Application.isPlaying)             //if the application is active, destroy them
            {
                Destroy(toothHolder.transform.GetChild(i).gameObject);
            }
            else                                   //if the application is not active, use DestroyImmediate instead
            {
                DestroyImmediate(toothHolder.transform.GetChild(i).gameObject);
            }
        }

        //setup variables used by all of the teeth made
        float toothWidthInPixels = toothSprite == null ? toothSprite.rect.width * toothScaleFactor : 20f; //get the size of the tooth sprite, scaled
        float radius = (teeth * toothWidthInPixels) / (2 * MathF.PI); //calculate radius in radians

        //setup the circle in the middle
        if (gearCenterSprite != null)
        {
            gearCenterSprite.rectTransform.sizeDelta = new Vector2(radius * 2f, radius * 2f);
        }
        else
        {
            throw new Exception("Exception: gearCenterSprite not set for AdaptiveGearController");
        }

        //Good explanation: https://discussions.unity.com/t/how-to-instantiate-objects-in-a-circle-formation-around-a-point/226980/2
        //setup each tooth individually
        for (int i = 0; i < teeth; i++) //for each tooth we need to make,
        {
            //make the game object, and give it its Components
            GameObject go = new GameObject("Gear Tooth" + i);                                           //make a gameobject
            RectTransform rectTransform = go.AddComponent<RectTransform>();                             //make it a RectTransform
            go.transform.SetParent(toothHolder.transform, false);                                       //make it a child of the toothHolder
            Image toothImage = go.AddComponent<Image>();                                                //add an image
            toothImage.sprite = toothSprite;                                                            //make it a tooth
            toothImage.SetNativeSize();                                                                 //scale it to its native size
            rectTransform.localScale = new Vector3(toothScaleFactor, toothScaleFactor, 1f);//scale it to the ScaleFactor

            //set it up in its position around the circle
            var radians = (2 * MathF.PI / teeth) * i;                   //calculate how far around the circle to go
            //triangulate a vector3 in the appropriate direction
            var vertical = Mathf.Sin(radians);                          //one side of the traingle,
            var horizontal = Mathf.Cos(radians);                        //other side of the triangle,
            Vector3 spawnDir = new Vector3(horizontal, vertical, 0);    //combine them to one normalized vector
            rectTransform.anchoredPosition = (spawnDir * radius);           //move in that direction, a radius amount
            rectTransform.localRotation = Quaternion.Euler(0, 0, ((360 / teeth) * i) - 90); //rotate outwards (-90 is the direction the first tooth spawns in)
        }
    }

    #region Teeth Count Changing Functions
    public void IncrementTeeth()
    {
        teeth++;
        RecalcImage();
    }

    public void DecrementTeeth()
    {
        if (teeth > 6)
        { 
            teeth--;
            RecalcImage();
        }
    }
    public void SetTeeth(int t)
    {
        if (t < 6 || t > 999)
        {
            throw new System.Exception("Invalid number of teeth to generate a gear with: " + t.ToString());
        }
        else
        {
            teeth = t;
            RecalcImage();
        }
    }
    #endregion
}
