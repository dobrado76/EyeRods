//using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Rods : MonoBehaviour
{
    public Rod template;
    public int count = 100;
    public float minIntensity = 0.1f;
    public float maxIntensity = 1.0f;

    float _minIntensity = -1;
    float _maxIntensity = -1;

    void Start()
    {
        GenerateRods(count);
    }

    void Update()
    {
        if (_minIntensity != minIntensity || _maxIntensity != maxIntensity)
        {
            _minIntensity = minIntensity;
            _maxIntensity = maxIntensity;

            foreach (Transform child in transform)
            {
                Rod rod = child.GetComponent<Rod>();
                if (rod != null)
                {
                    rod.minIntensity = minIntensity;
                    rod.maxIntensity = maxIntensity;
                }
            }
        }
    }

    void DeleteAll()
    {
        List<GameObject> list = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.name != "Rod-Template")
                list.Add(child.gameObject);
        }

        foreach (GameObject go in list)
        {
            DestroyImmediate(go);
        }
    }

    //[Button("Generate")]
    public void Generate()
    {
        GenerateRods(count);
    }

    public void GenerateRods(int count)
    {
        DeleteAll();
        for (int i = 0; i < count; i++)
        {
            Rod rod = Instantiate(template, transform);
            rod.name = "Rod " + i;
            rod.minIntensity = this.minIntensity;
            rod.maxIntensity = this.maxIntensity;
            rod.Init();
            //rod.transform.localPosition = new Vector3(Random.Range(-10.25f, 10.25f), Random.Range(-6.7f, 6.7f));
        }
    }
}
