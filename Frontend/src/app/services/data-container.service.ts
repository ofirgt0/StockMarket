import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { Deal, dealler, stock } from 'src/entities.model';
const BACKEND_URL = 'http://localhost:5187/';
@Injectable({
  providedIn: 'root'
})
export class DataContainerService {
  
  deallers: Observable<dealler[]>;
  logedInDealler!: Observable<dealler>;
  stocks: Observable<stock[]>;
  DeallerDeals: Observable<Deal[]>;
  
  subDeallerName: BehaviorSubject<string>;
  deallerName!: string;
  
  constructor(private http: HttpClient) 
  { 
    this.deallers = this.http.get<dealler[]>(BACKEND_URL + 'deallers');
    this.stocks = this.http.get<stock[]>(BACKEND_URL + 'stocks');
    this.subDeallerName = new BehaviorSubject<string>(this.deallerName);
    this.DeallerDeals = this.http.get<Deal[]>(BACKEND_URL + 'deals/'+this.deallerName);
  }
  getDeallerNameValue(): Observable<string> {
    return this.subDeallerName.asObservable();
  }
  setDeallerNameValue(newValue: string): void {
    this.subDeallerName.next(newValue);
  }

  getLogedInDeallerDeals()
  {
    this.DeallerDeals = this.http.get<Deal[]>(BACKEND_URL + 'deals/'+this.deallerName);
    return this.DeallerDeals;
  }
  getDeallers() 
  {
    return this.deallers;
  }
  getLogedDealler() 
  {
    return this.logedInDealler;
  }
  async getStocks() {
    await this.stocks.subscribe();
    return this.stocks;
  }
}
