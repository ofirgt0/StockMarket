namespace Backend.Helpers
{
    public class MakeADealResponse
    {
        int QuantityRemaining;
        ActionPerformedType Action;

        public MakeADealResponse(int quantityRemaining, ActionPerformedType action)
        {
            QuantityRemaining = quantityRemaining;
            Action = action;
        }
        public MakeADealResponse(int quantityRemaining, int oldAmount)
        {
            if (quantityRemaining == 0)
                Action = ActionPerformedType.WasFullyExecuted;
            if (quantityRemaining < oldAmount)
                Action = ActionPerformedType.WasPartiallyExecuted;
            else
                Action = ActionPerformedType.WasNotExecuted;

            QuantityRemaining = quantityRemaining;
        }
    }
}