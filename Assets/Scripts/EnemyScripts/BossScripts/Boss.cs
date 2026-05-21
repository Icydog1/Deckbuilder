using UnityEngine;

public class Boss : Enemy
{
    public override void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 11);
        base.Awake();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void Start()
    {
        base.Start();
    }
    public override void Die()
    {
        base.Die();
        levelManager.BossKilled(oneToOnePos);
    }
}
