using System.Collections.Generic;
using UnityEngine;

public class ActionPreformer : MonoBehaviour
{
    protected List<Figure> actingFigures = new List<Figure>();
    public List<Figure> ActingFigures { get { return actingFigures; } set { actingFigures = value; } }
    protected bool stopPlaying;
    public bool StopPlaying { get { return stopPlaying; } set { stopPlaying = value; } }


    public void StopCommanding()
    {
        foreach (Figure figure in actingFigures)
        {
            figure.Controled = false;
        }
        actingFigures.Clear();
    }
}
