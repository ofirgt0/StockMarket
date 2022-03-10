import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChartManagerService {

  stockNameToShowSubscription: BehaviorSubject<string>;
  
  constructor() { 
    this.stockNameToShowSubscription = new BehaviorSubject<string>("Apple");
  }
  getStockNameValue(): Observable<string> {
    return this.stockNameToShowSubscription.asObservable();
  }
  setStockNameValue(newValue: string): void {
    this.stockNameToShowSubscription.next(newValue);
  }
}
