using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class particleUI : MaskableGraphic
{
    [SerializeField] private ParticleSystemRenderer _particle;
    [SerializeField] private Camera _bakeCamera;

    // Update is called once per frame
    void Update()
    {
        SetVerticesDirty();
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
