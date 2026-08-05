public class Endless : UIButton
{
    private GameManager gameManager;

    protected override void Awake()
    {
        gameManager = RefrenceStorage.gameManager;
        base.Awake();
    }
    public override void Activate()
    {
        gameManager.StartCoroutine(gameManager.ContinueGame());
    }
}
