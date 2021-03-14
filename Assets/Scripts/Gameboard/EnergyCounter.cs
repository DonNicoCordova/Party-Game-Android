using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergyCounter : MonoBehaviour
{
    public TextMeshProUGUI energyText;
    
    private Animator _animator;

    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }
    public void SetEnergy(int energy)
    {
        energyText.text = energy.ToString();
    }
    public void Show()
    {
        _animator.ResetTrigger("hide");
        _animator.SetTrigger("show");
    }
    public void Hide()
    {
        _animator.ResetTrigger("show");
        _animator.SetTrigger("hide");
    }
}
