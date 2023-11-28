
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TextManager : MonoBehaviour 
{
    [SerializeReference]
    public List<TextDecorator> decorator;

    public int test;

    [SerializeField]
    [DisplayOnly]
    protected TMP_Text textMesh;

    private void Start() {
        this.textMesh = GetComponent<TMP_Text>();
    }    
}