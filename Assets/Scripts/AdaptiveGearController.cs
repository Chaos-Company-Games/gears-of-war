using NUnit.Framework;
using System;
using System.Collections.Generic;
//using UnityEditor.iOS;
//using UnityEditor.PackageManager;
//using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
//Controller for each individual gear
public class AdaptiveGearController : MonoBehaviour
{

    public float teeth = 8;
    private bool hasAbilitySlots = false;
    List<int> abilitySlots; //keeps track of what slots the gear has, in an easy to track list. This'll make it easy to tell what slots exist in this gear.
    List<AbilitySocketController> abilitySocketControllers; //keeps track of the ability socket controllers made by this.
    private float abilityWait = 0f; //waits between ability triggers

    AbilitySocketController abilitySocketControllerPrefab; //prefab holder

    private float radius;

    private bool spins = true; //boolean storage for spinning (probably always true, w/e, just in case)
    public static float spinFactor = 4f; //multiplies output speed, controlling speed of all gears
    public SpinDir spinDir = SpinDir.Clockwise;
    public static float pitch = 0.98f; //pitch value for the tooth, to adjust the pixel calc of the center of the tooth. Helps big gears stay in lockstep.

    [SerializeField] private Sprite toothSprite; //Sprite storage for tooth of Gear
    [SerializeField] private Sprite majorToothSprite; //Sprite storage for special tooth
    private float toothScaleFactor = 0.012f; //scale factor for the teeth
    [SerializeField] private GameObject toothHolder; //GameObject for holding all of the Teeth generated
    [SerializeField] private Image gearCenterSprite; //holder for the center of Gear sprite
    private void Awake()
    {
        abilitySocketControllerPrefab = Resources.Load<AbilitySocketController>("AbilitySocket");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RecalcImage();
    }

    private void Update()
    {
        //spin speed of gear
        //more teeth == slower, so divide by number of teeth
        //spinFactor controls overall speed
        if (spins)
        {
            float spin; //calc spin based upon spin dir
            float toothPortion = 360f / teeth;
            if (spinDir == SpinDir.Clockwise)
                spin = toothPortion * spinFactor * Time.deltaTime;
            else
                spin = toothPortion * spinFactor * Time.deltaTime * -1;
            toothHolder.transform.Rotate(0f, 0f, spin, Space.Self);
            //Debug.Log(Math.Abs(toothHolder.transform.localRotation.eulerAngles.z));

            //if we're spinning past an ability, trigger it
            if (hasAbilitySlots)
            {
                if(abilityWait > 0f)
                {
                    //Debug.Log(abilityWait);
                    abilityWait = abilityWait - MathF.Abs(spin);
                }
                else
                {
                    foreach (AbilitySocketController ability in abilitySocketControllers)
                    {
                        if(ability.ability != null)
                        {
                            float loc = Math.Abs(toothHolder.transform.localRotation.eulerAngles.z);
                            if (loc < 0f)
                            {
                                loc = loc + 180f;
                            }
                            //Debug.Log(loc);
                            float targetLoc = ability.tooth * toothPortion; //calc location of ability
                                                                            //Debug.Log(targetLoc + " | " + (targetLoc - 5f) + " | " + (targetLoc + 5f) + " | " + loc);
                            if (targetLoc - 1f <= loc && targetLoc + 1f >= loc)
                            {
                                //Debug.Log("Near ability");
                                PlayerHealth.instance.DoAbility(ability.ability, (int)teeth);
                                abilityWait = 2f;
                                StartCoroutine(ability.Flash());
                            }
                        }
                    }
                }
            }
        }
    }

    //Regenerate the Gear and its teeth and abilities whenever something is changed.
    private void RecalcImage()
    {
        //purge all old teeth from the gameobject
        for(int i = toothHolder.transform.childCount -1; i >= 0; i--) //go backwards, to ensure all are destroyed 
        {
            if (Application.isPlaying)             //if the application is active, destroy them
            {                                      //destroy doesn't work in editor
                Destroy(toothHolder.transform.GetChild(i).gameObject);
            }
            else                                   //if the application is not active, use DestroyImmediate instead
            {                                      //destroyimmediate doesn't work in active play
                DestroyImmediate(toothHolder.transform.GetChild(i).gameObject);
            }
        }

        //setup variables used by all of the teeth made
        float toothWidthInPixels = toothSprite == null ? toothSprite.rect.width * toothScaleFactor : 20f; //get the size of the tooth sprite, scaled
        radius = (teeth * toothWidthInPixels) / (2.0f * (float)MathF.PI); //calculate radius in radians

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
            float radians = (2f * MathF.PI / (float)teeth) * i;                   //calculate how far around the circle to go
            //triangulate a vector3 in the appropriate direction
            float vertical = Mathf.Sin(radians);                          //one side of the traingle,
            float horizontal = Mathf.Cos(radians);                        //other side of the triangle,
            Vector3 spawnDir = new Vector3(horizontal, vertical, 0);      //combine them to one normalized vector
            rectTransform.anchoredPosition = (spawnDir * radius * pitch);           //move in that direction, a radius amount, modulated by the pitch of the tooth.
            rectTransform.localRotation = Quaternion.Euler(0, 0, ((360f / (float)teeth) * i) - 90f); //rotate outwards (-90 is the direction the first tooth spawns in). Quaternions use degrees, not radians

            //if there is an ability core here, place it
            if (hasAbilitySlots && abilitySlots.Contains(i)) //this is why this list exists, alongside the abilitySocketControllers list. it's way more indexable, makes it easy to tell if there's a slot that's supposed to be here.
            {
                AbilitySocketController socketController = abilitySocketControllers.Find(x => x.tooth == i); //find the abilitySocketController for this tooth
                socketController.GetComponent<RectTransform>().anchoredPosition = (spawnDir * radius * .8f); //place it slightly behind the tooth
            }

            if (i == 0)
            {
                toothImage.sprite = majorToothSprite;
                rectTransform.localScale = new Vector3(toothScaleFactor * 2f, toothScaleFactor * 2f, 1f);
            }
        }
    }

    //Radius is calculated in RecalcImage, and just making that public will work, theoretically.
    //However, if the load order of the functions fucks up, it'll bug out in weird ways. This garuntees that the Radius is always correct when it's needed.
    public float GetRadius()
    {
        float toothWidthInPixels = toothSprite == null ? toothSprite.rect.width * toothScaleFactor : 20f; //get the size of the tooth sprite, scaled
        return ((teeth * toothWidthInPixels) / (2.0f * (float)MathF.PI)); //calculate radius in radians
    }

    public void FlipSpin()
    {
        if (spinDir == SpinDir.Clockwise)
            spinDir = SpinDir.CounterClockwise;
        else if (spinDir == SpinDir.CounterClockwise)
            spinDir = SpinDir.Clockwise;
        else
            throw new Exception("bad spindir");
    }

    public void SetSpin(SpinDir temp)
    {
        spinDir = temp;
    }

    public void EnableButtons()
    {
        if (hasAbilitySlots)
        {
            foreach (AbilitySocketController controller in abilitySocketControllers)
            {
                controller.manageButtonStatus(true);
            }
        }
    }

    public void DisableButtons()
    {
        if (hasAbilitySlots)
        {
            foreach (AbilitySocketController controller in abilitySocketControllers)
            {
                controller.manageButtonStatus(false);
            }
        }
    }

    public void EnableEmptySockets()
    {
        if (hasAbilitySlots)
        {
            foreach (AbilitySocketController controller in abilitySocketControllers)
            {
                controller.enableIfEmpty();
            }
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

    public void SetTeethAndSlots(int t, List<int> ac)
    {
        if (t < 6 || t > 999)
        {
            throw new System.Exception("Invalid number of teeth to generate a gear with: " + t.ToString());
        }
        else
        {
            teeth = t;
            if (ac.Count > 0)
            {
                //Debug.Log("this gear has ability cores, number:" + ac.Count);
                abilitySlots = ac;
                abilitySocketControllers = new List<AbilitySocketController>();
                for (int i = 0; i < ac.Count; i++)
                {
                    AbilitySocketController temp = Instantiate(abilitySocketControllerPrefab); //make the socket
                    temp.transform.SetParent(this.transform); //give it a parent
                    temp.setUp(this);
                    temp.tooth = ac[i]; //tell it what tooth it's attached to
                    abilitySocketControllers.Add(temp); //add it to the list
                }
                hasAbilitySlots = true;
            }
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

public enum SpinDir
{
    Clockwise = 1,
    CounterClockwise = -1
}