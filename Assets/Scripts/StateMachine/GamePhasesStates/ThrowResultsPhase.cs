
internal class ThrowResultsPhase : IState
{
    public ThrowResultsPhase()
    {
    }

    public void Tick()
    {
        if (!GameManager.instance.DiceOnDisplay())
            GameManager.instance.ShowDicesOnCamera();
    }

    public void OnEnter()
    {
        GameManager.instance.ShowMessage("¡Que mala cuea! ajkajskkadj");
    }

    public void OnExit()
    {
    }
}