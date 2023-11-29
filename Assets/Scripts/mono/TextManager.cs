
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TextManager : MonoBehaviour 
{
    [SerializeReference]
    public TextDecorator decorator;

    public int test;

    [SerializeField]
    [DisplayOnly]
    protected TMP_Text textMesh;

    private void Start() {
        this.textMesh = GetComponent<TMP_Text>();
    }

    private void Update() {
        this.textMesh.ForceMeshUpdate();
        if(decorator == null)
            return;
        Mesh newMesh = this.decorator.Render(this.textMesh);
        this.textMesh.canvasRenderer.SetMesh(newMesh);
    }    
}