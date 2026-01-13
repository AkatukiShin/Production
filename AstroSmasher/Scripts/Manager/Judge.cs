using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Judge : MonoBehaviour
{
    [SerializeField]
    public static int enemyKillCount = 0;

    private void Start() {
        enemyKillCount = 0;
    }
    // Update is called once per frame
    void Update()
    {
        
        if (enemyKillCount >= 5)
        {
            SceneManager.LoadScene("WinScene");
        }
    }
}
