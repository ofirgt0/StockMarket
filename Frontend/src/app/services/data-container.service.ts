import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { Deal, dealler, HoldingsWorth, stock } from 'src/entities.model';
const BACKEND_URL = 'http://localhost:5187/';
@Injectable({
  providedIn: 'root',
})
export class DataContainerService {
  deallers: Observable<dealler[]>;
  logedInDealler!: Observable<dealler>;
  stocks: Observable<stock[]>;
  DeallerDeals: Observable<Deal[]>;
  DeallerHoldings: Observable<HoldingsWorth>;

  subDeallerName: BehaviorSubject<string>;
  deallerName!: string;

  constructor(private http: HttpClient) {
    this.subDeallerName = new BehaviorSubject<string>(this.deallerName);

    this.deallers = this.http.get<dealler[]>(BACKEND_URL + 'deallers');
    this.stocks = this.http.get<stock[]>(BACKEND_URL + 'stocks');

    this.DeallerDeals = this.http.get<Deal[]>(
      BACKEND_URL + 'deals/' + this.deallerName
    );
    this.DeallerHoldings = this.http.get<HoldingsWorth>(
      BACKEND_URL + 'stocks/worth/' + this.deallerName
    );
  }

  getDeallerNameValue(): Observable<string> {
    return this.subDeallerName.asObservable();
  }

  setDeallerNameValue(newValue: string): void {
    this.subDeallerName.next(newValue);
  }

  getLogedInDeallerDeals() {
    this.DeallerDeals = this.http.get<Deal[]>(
      BACKEND_URL + 'deals/' + this.deallerName
    );
    return this.DeallerDeals;
  }

  getDeallers() {
    return this.deallers;
  }

  getLogedDealler() {
    return this.logedInDealler;
  }

  async getStocksAsync() {
    await this.stocks.subscribe();
    return this.stocks;
  }

  getDeallerHoldingsWorth() {
    this.DeallerHoldings = this.http.get<HoldingsWorth>(
      BACKEND_URL + 'stocks/worth/' + this.deallerName
    );
    return this.DeallerHoldings;
  }

  // getNewDealsNumber(){
  //   var counter=0;
  //   this.DeallerDeals.subscribe(deals=>{deals.map(deal=>(deal.dealTime))})
  //   while()
  // }
}
