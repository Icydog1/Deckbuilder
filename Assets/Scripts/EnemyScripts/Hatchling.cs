using System;
using System.Collections;
using System.Collections.Generic;
public class Hatchling : Enemy
{
    int randomVariationNumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        //initial Movesets
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(10)
            ,() => Attack(6)
            ,() => Block(4)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(10)
            ,() => Attack(4,3)
            ,() => Block(6)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Block(10)
        });
        //First Growth
        moveSets.Add(new List<Func<IEnumerator>> {
            () => ApplyCondition(new Strength(4))
        });
        //second Movesets
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Block(7)
            ,() => ApplyCondition(new Vigor(8))
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(10)
            ,() => Attack(4,1,1,2)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(10)
            ,() => Attack(7,3)
        });
        //Final Growth
        moveSets.Add(new List<Func<IEnumerator>> {
            () => ApplyCondition(new Flight())
            ,() => ApplyCondition(new Strength(4))
            ,() => ApplyCondition(new Speed(4))
        });
        //final Movesets
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(14)
            ,() => Attack(10,3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(22)
            ,() => Attack(7)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(16)
            ,() => Attack(7,2)
        });

        bool stop = false;
        //0% growth chance then 33% then 66% then 100%
        List<int> potentialMoves = new List<int>() { 0, 1, 2};
        initialMoves = new List<int>();
        //while (stop == false)
        for (int i = 0; stop == false; i++)
        {
            int randomNumber = UnityEngine.Random.Range(0, potentialMoves.Count);
            initialMoves.Add(potentialMoves[randomNumber]);
            if (potentialMoves[randomNumber] == 3)
            {
                stop = true;
            }
            if (i == 0)
            {
                potentialMoves.Add(3);
            }
            potentialMoves.RemoveAt(randomNumber);

        }
        stop = false;
        potentialMoves = new List<int>() { 4, 5, 6};
        for (int i = 0; stop == false; i++)
        {
            int randomNumber = UnityEngine.Random.Range(0, potentialMoves.Count);
            initialMoves.Add(potentialMoves[randomNumber]);
            if (potentialMoves[randomNumber] == 7)
            {
                stop = true;
            }
            if (i == 0)
            {
                potentialMoves.Add(7);
            }
            potentialMoves.RemoveAt(randomNumber);

        }
        stop = false;
        potentialMoves = new List<int>() { 8, 9, 10 };
        movesSetOrder = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            int randomNumber = UnityEngine.Random.Range(0, potentialMoves.Count);
            movesSetOrder.Add(potentialMoves[randomNumber]);
            potentialMoves.RemoveAt(randomNumber);

        }

        //randomVariationNumber = UnityEngine.Random.Range(0, 3);
        //if (randomVariationNumber == 0)
        //{
        //    initialMoves = new List<int>() { 0, 1, 3, 5, 7 };
        //    movesSetOrder = new List<int>() { 8, 9, 10 };
        //}
        //if (randomVariationNumber == 1)
        //{
        //    initialMoves = new List<int>() { 1, 2, 3, 6, 4, 5, 7 };
        //    movesSetOrder = new List<int>() { 9, 10, 8 };
        //}
        //if (randomVariationNumber == 2)
        //{
        //    initialMoves = new List<int>() { 2, 3, 4, 6, 5, 7 };
        //    movesSetOrder = new List<int>() { 10, 8, 9 };
        //}
        maxHealth = 26;
        XPValue = 4;

        base.Awake();
    }
}