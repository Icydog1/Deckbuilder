using UnityEngine;

public class WizardCard : Card
{
    public WizardCard(int baseRarity, int initialTopCost, int initialBottomCost, int initialTopManaCost = 0, int initialBottomManaCost = 0) : base(baseRarity, initialTopCost, initialBottomCost)
    {
        topManaCost = initialTopManaCost;
        bottomManaCost = initialBottomManaCost;
    }

    public override void AttemptToPlayTop()
    {
        if (playerControler.TopEnergy >= topCost && playerControler.Mana >= topManaCost)
        {
            isTopPlayed = true;
            playerControler.TopEnergy -= topCost;
            playerControler.Mana -= topManaCost;
            StartCoroutine(SetPlayed());
        }
        else
        {
            PlayFailed();
        }
    }
    public override void AttemptToPlayBottom()
    {
        if (playerControler.BottomEnergy >= bottomCost && playerControler.Mana >= bottomManaCost)
        {
            isBottomPlayed = true;
            playerControler.BottomEnergy -= bottomCost;
            playerControler.Mana -= bottomManaCost;
            StartCoroutine(SetPlayed());
        }
        else
        {
            PlayFailed();
        }
    }
}


