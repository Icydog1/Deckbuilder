using System.Collections;
using UnityEngine;

public class Boss : Enemy
{
    public override void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 11);
        isBoss = true;
        base.Awake();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void Start()
    {
        base.Start();
    }
    public override IEnumerator Die()
    {
        yield return gameManager.StartCoroutine(base.Die());
        floorManager.BossKilled(oneToOnePos);
    }
}
