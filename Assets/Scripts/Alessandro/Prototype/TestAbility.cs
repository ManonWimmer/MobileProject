using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


//[CreateAssetMenu(menuName = "AbilitiesAless/TestAbility")]
public class TestAbility : MonoBehaviour, IAbilities
{

    // Les valeurs ne devraient pas être définies ici, elles 
    // doivent être recupérée d'un scriptable object
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private Image _icon;
    [SerializeField] private IAbilities.Type _type;
    [SerializeField] private int _cost = 1;
    [SerializeField] scriptablePower _data;
    public string Name { get => _name; set => _name = "ravioli beam";}
    public string Description { get => _description; set => _description = "italian power";}
    public Image Icon { get => _icon; set => _icon = null; }
    public IAbilities.Type AbilityType { get => _type; set => _type = IAbilities.Type.ACTIVE; }
    public int Cost { get => _cost; set => _cost = 1; }
    //public scriptablePower Data { get => _data; set => null; }

    public void AbilityUse()
    {
        throw new NotImplementedException();
    }
}
