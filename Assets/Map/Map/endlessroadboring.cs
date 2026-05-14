using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class endlessroadboring : MonoBehaviour
{
    [SerializeField]
    GameObject sectionPrefab;

    GameObject[] sections = new GameObject[10];

    GameObject[] sectionPool = new GameObject[10];

    Transform playerCarTransform;

    WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    const float sectionLength = 150;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //create sectionPool
        for (int i = 0; i < sectionPool.Length; i++)
        {
            sectionPool[i] = Instantiate(sectionPrefab);
            sectionPool[i].SetActive(false);
        }

        //Add the first sections to the road
        for (int i = 0; i < sections.Length; i++)
        {
            //Create new section
            GameObject newSection = sectionPool[i];

            //Position new section
            newSection.transform.position = new Vector3(sectionLength * i, 0, sectionPool[i].transform.position.z);
            newSection.SetActive(true);

            //Set section in the array
            sections[i] = newSection;
        }
        StartCoroutine(UpdateLessOftenCO());
    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            UpdateSectionPosition();
            yield return waitFor100ms;
        }
    }

    void UpdateSectionPosition()
    {
        for (int i = 0; i < sections.Length; i++)
        {
            //checks if section is too far behind
            if (sections[i].transform.position.x - playerCarTransform.position.x < -sectionLength)
            {
                Vector3 lastSectionPosition = sections[i].transform.position;
                sections[i].SetActive(false);

                //moves a new section into position
                GameObject newSection = sectionPool[i];
                sections[i] =newSection;
                newSection.transform.position = new Vector3(lastSectionPosition.x + sectionLength * sections.Length, 0, lastSectionPosition.z);
                newSection.SetActive(true);
            }
        }
    }
}
