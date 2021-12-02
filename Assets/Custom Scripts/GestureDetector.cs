using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour
{
    public float threshold = 0.05f;

    public OVRSkeleton skeleton;
    public List<Gesture> gestures;
    private List<OVRBone> fingerBones = null;

    public bool debugMode = true;

    private bool hasStarted = false;
    private bool hasRecognize = false;
    private bool done = false;

    public UnityEvent notRecognize;

    private Gesture previousGesture;


    void Start()
    {
        StartCoroutine(DelayRoutine(2.5f, Initialize));
    }

    public IEnumerator DelayRoutine(float delay, Action actionToDo)
    {
        yield return new WaitForSeconds(delay);
        actionToDo.Invoke();
    }

    public void Initialize()
    {
        SetSkeleton();

        hasStarted = true;
    }

    public void SetSkeleton()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        previousGesture = new Gesture();
    }

    void Update()
    {
        if (debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            Save();
        }

        if (hasStarted.Equals(true))
        {
            Gesture currentGesture = Recognize();

            hasRecognize = !currentGesture.Equals(new Gesture());

            if (hasRecognize && !currentGesture.Equals(previousGesture))
            {
                Debug.Log("New Gesture Found: " + currentGesture.name);
                previousGesture = currentGesture;
                done = true;
                currentGesture.onRecognized?.Invoke();
            }
            else
            {
                if (done)
                {
                    Debug.Log("Not Recognized");
                    done = false;
                    notRecognize?.Invoke();
                }
            }
        }
    }

    void Save()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBones)
        {
            //finger position relative to root 
            Debug.Log("Setting bone data!");
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerDatas = data;
        gestures.Add(g);
    }

    Gesture Recognize()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach(var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);
                if(distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }

            if(!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }

        return currentGesture;
    }
}
