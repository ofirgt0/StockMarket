import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, timer } from 'rxjs';
import { dealler, stock } from 'src/entities.model';
const timeIntevalSeconds=10;

@Injectable({
  providedIn: 'root'
})
export class ServerAccessService {
  deallers: Observable<dealler[]>;
  stocks: Observable<stock[]>;
  dealler!: dealler;
  isAuth=false;
  constructor(private http: HttpClient) {
    this.deallers=this.http.get<dealler[]>("http://localhost:5187/deallers");
    this.stocks=this.http.get<stock[]>("http://localhost:5187/stocks"); 

    timer(0, 1000*timeIntevalSeconds).pipe( 
      map(() => { 
        this.initProperties(); 
        
        this.stocks.subscribe(resStocks=>{console.log(resStocks)});
      }) 
    ).subscribe(); 
    
  }

  updateAuthStatus(currentStatus:boolean)
  {
    this.isAuth=currentStatus;
  }

  async onLogin(id:number)
  {
    await this.http.get<dealler>("http://localhost:5187/dealler/"+id).subscribe(d=>{this.dealler=d});
    return this.dealler!=null;
  }

  initProperties(){
    this.deallers=this.http.get<dealler[]>("http://localhost:5187/deallers");
    this.stocks=this.http.get<stock[]>("http://localhost:5187/stocks"); 
  }
  getDeallers()
  {
    return this.deallers;
  }
  getStocks(){
    return this.stocks;
  }
}
