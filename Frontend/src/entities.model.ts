export interface stock {
  id: number;
  name: string;
  currentPrice: number;
  amount: number;
  currentStockAmountInBurse: number;
  percentageDifference: number[];
  imgUrl?: string;
}

export interface dealler {
  id: number;
  name: string;
  moneyAtOpening: number;
  currMoney: number;
  ownedStocks: Array<stock>;
  ownedStocksAmount:number;
}

export interface Offer {
  deallerName: string;
  stockName: string;
  wantedPrice: number;
  wantedAmount: number;
  type: number;
}
