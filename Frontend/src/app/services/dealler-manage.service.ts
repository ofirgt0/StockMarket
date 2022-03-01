import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpClientModule } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class DeallerManageService{
  
  deallersList: string[] | undefined;
  constructor(private http:HttpClient) { }

  getDeallers() 
  {
    this.http.get<string[]>("http://localhost:5187/api/stockMarket").subscribe(data=>{this.deallersList=data}); 
    return this.deallersList;
  }
}
