namespace Backend.Helpers
{
    public class MakeADealResponse
    {
        public int QuantityRemaining{get;set;}
        public ActionPerformedType Action{get;set;}

        public MakeADealResponse(int quantityRemaining, ActionPerformedType action)
        {
            QuantityRemaining = quantityRemaining;
            Action = action;
        }
        public MakeADealResponse(int quantityRemaining, int oldAmount)
        {
            if (quantityRemaining == 0)
                Action = ActionPerformedType.WasFullyExecuted;
            else if (quantityRemaining < oldAmount)
                Action = ActionPerformedType.WasPartiallyExecuted;
            else if (quantityRemaining == oldAmount)
                Action = ActionPerformedType.WasNotExecutedNewOfferHasUploaded;
            else
                Action = ActionPerformedType.WasNotExecutedOfferNotPossible;

            QuantityRemaining = quantityRemaining;
        }
    }
}