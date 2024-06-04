using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private Scene _currentScenel;
    private Hero _hero;

    private void Start()
    {
        _hero = FindObjectOfType<Hero>();
        _currentScenel = SceneManager.GetActiveScene();
    }

    private void FixedUpdate()
    {
        if (!_hero) SceneManager.LoadScene(_currentScenel.buildIndex);
    }

    private void Update()
    {
    }
}
