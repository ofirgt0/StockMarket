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
                Action = ActionPerformedType.TheTransactionWasFullyExecuted;
            if (quantityRemaining < oldAmount)
                Action = ActionPerformedType.TheTransactionWasPartiallyExecuted;
            else
                Action = ActionPerformedType.TheTransactionWasNotExecuted;

            QuantityRemaining = quantityRemaining;
        }
    }
}