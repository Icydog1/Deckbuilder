public static class Global
{
    public static int Clamp(int value, int min = Var.minValue)
    {
        //ignore if special value
        if (value == Var.infinityValue || value == Var.nullValue || value == Var.doesNotExistValue || value == Var.changeingValue)
        {
            return value;
        }
        //else if (!canBeNegative && value < 0)
        //{
        //    return 0;
        //}

        //set to minumum value if value is below it
        else if (value < min)
        {
            return min;
        }
        //sets value to bounds of game if outside of them
        else if (value > Var.maxValue)
        {
            return Var.maxValue;
        }
        //returns base value
        else
        {
            return value;
        }
    }
}
