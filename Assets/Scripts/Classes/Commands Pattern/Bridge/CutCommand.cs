using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class CutCommand : CommandMono
{
    private bool _finished;
    private bool _failed;
    public override void Reset()
    {
        _finished = false;
        _failed = false;
    }
    public override void Execute()
    {
        if (GameManager.Instance.saw != null)
        {
            Saw saw = GameManager.Instance.saw.GetComponent<Saw>();
            saw.animator.SetTrigger("Animate");
            StartCoroutine(CheckIfAnimationFinished());
        }
        else
        {
            _failed = true;
        }
    }
    public IEnumerator CheckIfAnimationFinished()
    {
        while (_finished == false)
        {
            Saw saw = GameManager.Instance.saw.GetComponent<Saw>();
            if (saw.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                _finished = true;
            }
            yield return new WaitForSeconds(1f);
        }
    }
    public override bool IsFinished => _finished;
    public override bool Failed => _failed;
}
