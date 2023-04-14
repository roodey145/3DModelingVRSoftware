public enum InteractionsEnum : byte
{
    ScaleUp = 0,
    ScaleDown,
    Grab,
    Move,
    Rotate, 
    Extrude,
    LoopCut,
    Bevel,
    Confirm
}


public static class InteractionsEnumExtensions
{
    public static string name(this InteractionsEnum interactionsEnum)
    {
        switch (interactionsEnum)
        {
            case InteractionsEnum.ScaleUp:
                return "ScaleUp";
            case InteractionsEnum.ScaleDown:
                return "ScaleDown";
            case InteractionsEnum.Grab:
                return "Grab";
            case InteractionsEnum.Move:
                return "Move";
            case InteractionsEnum.Rotate:
                return "Rotate";
            case InteractionsEnum.Extrude:
                return "Extrude";
            case InteractionsEnum.LoopCut:
                return "LoopCut";
            case InteractionsEnum.Bevel:
                return "Bevel";
            case InteractionsEnum.Confirm:
                return "Confirm";
            default:
                return "";
        }
    }
}
