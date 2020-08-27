internal class OrderReultsPhase : IState
{

    public OrderReultsPhase()
    {
    }

    public void Tick()
    {
        if (!GameManager.instance.DiceOnDisplay())
            GameManager.instance.ShowDicesOnCamera();
    }

    public void OnEnter()
    {
        GameManager.instance.ShowMessage("Así quedaron y que wea");
    }

    public void OnExit()
    {
        //ORDENAR LISTA
    }
}