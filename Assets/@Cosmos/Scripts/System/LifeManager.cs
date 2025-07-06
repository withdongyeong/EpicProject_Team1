public class LifeManager : Singleton<LifeManager>
{
    private int life = 0;
    public int Life => life;

    protected override void Awake()
    {
        base.Awake();
        life = 1;
    }

    public void AddLife(int i)
    {
        life += i;
    }

    public void RemoveLife(int i)
    {
        life -= i;
    }

    public void ResetLifeManager()
    {
        life = 1;
    }
}
