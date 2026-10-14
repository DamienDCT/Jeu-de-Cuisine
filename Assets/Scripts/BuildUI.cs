using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildUI : MonoBehaviour
{
    public static BuildUI Instance { get; private set; }

    public event EventHandler<int> OnBuildChanged;

    [SerializeField] private BuildSystem buildSystem;
    

    private void Awake()
    {
        Instance = this;
    }

    public void SelectBuild(int buildIndex)
    {
        OnBuildChanged?.Invoke(this, buildIndex);
    }
    
    public void StartBuild()
    {
        buildSystem.EnableBuild();
    }
}
