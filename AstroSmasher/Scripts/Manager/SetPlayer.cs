using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerPos;

    [SerializeField] private GameObject[] planets;

    private void Awake()
    {
        GameObject player = Instantiate(planets[SelectPlanetsUI.selectedPlanet], playerPos.transform.position, Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
