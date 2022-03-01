import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StockManageService {
  stocksList: string[] | undefined;
  constructor(private http:HttpClient) { }

  getStocks() 
  {
    
    this.http.get<string[]>("http://localhost:5187/api/stockMarket").subscribe(data=>{this.stocksList=data}); 
    return this.stocksList;
  }
}
