using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class QualitySelection : MonoBehaviour
{
    private Dropdown dropdown;
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(QualitySettings.names.ToList());
        dropdown.value = QualitySettings.GetQualityLevel();
    }

    public void Change()
    {
        QualitySettings.SetQualityLevel(dropdown.value);
    }
}
