using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class particleUI : MaskableGraphic
{
    [SerializeField] private ParticleSystemRenderer _particle;
    [SerializeField] private Camera _bakeCamera;
    [SerializeField] bool isVerticalBillBoard = false;
    [SerializeField] float minSize = 2.1f;
    [SerializeField] float maxSize = 2.1f;
    // Update is called once per frame
    void Update()
    {
        SetVerticesDirty();
    }

    protected override void Start()
    {
        if(isVerticalBillBoard)
        {
            _particle.renderMode = ParticleSystemRenderMode.VerticalBillboard;
            _particle.maxParticleSize = maxSize;
            _particle.minParticleSize = minSize;
        }
    }
    protected override void OnPopulateMesh(Mesh mesh)
    {
        mesh.Clear();
        if ( _particle != null && _bakeCamera != null)
        {
            _particle.BakeMesh(mesh, _bakeCamera);
        }
    }

}
