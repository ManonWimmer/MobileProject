using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour
{
    [SerializeField] private powerButton _button;

    [SerializeField] private Transform _abilityContent;
    [SerializeField] private GameObject _prefabAbility;

    [SerializeField] private Transform _debuffContent;
    [SerializeField] private GameObject _prefabDebuff;

    public void Attack()
    {
        _button.GetEndTrun();
    }

    public void AddDebuff()
    {
        GameObject buttonGO = Instantiate(_prefabDebuff, transform.position, Quaternion.identity);

        buttonGO.transform.SetParent(_debuffContent);
    }

    public void AddAbility()
    {
        GameObject buttonGO = Instantiate(_prefabAbility, transform.position, Quaternion.identity);

        buttonGO.transform.SetParent(_abilityContent);
    }

    public void LauchConffeti()
    {
        List<ParticleSystem> conffeti = new List<ParticleSystem>();
        GameObject[] conffetis = GameObject.FindGameObjectsWithTag("conffeti");

        foreach (GameObject conff in conffetis)
        {
            if (conff != gameObject)
            {
                ParticleSystem particleSystem = conff.GetComponent<ParticleSystem>();
                conffeti.Add(particleSystem);
            }
        }

        foreach (ParticleSystem conff in conffeti)
        {
            conff.Play();
        }
    }

}
