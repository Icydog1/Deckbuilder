using UnityEngine;

public class Boss : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        transform.position = new Vector3(transform.position.x, transform.position.y, 11);
    }
    public override void Die()
    {
        base.Die();
        levelManager.BossKilled(oneToOnePos);
    }
}
