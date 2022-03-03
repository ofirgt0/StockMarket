import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NgForm } from '@angular/forms';
import { BehaviorSubject, map, Observable, timer } from 'rxjs';
import { dealler, stock } from 'src/entities.model';
const timeIntevalSeconds=10;

@Injectable({
  providedIn: 'root'
})
export class ServerAccessService {
  deallers: Observable<dealler[]>;
  stocks: Observable<stock[]>;
  
  isAuth=false;
  private subIsAuth: BehaviorSubject<boolean>;
  deallerName!: string;
  dealler!: dealler;
  constructor(private http: HttpClient) {

    this.subIsAuth= new BehaviorSubject<boolean>(this.isAuth);
    this.deallers=this.http.get<dealler[]>("http://localhost:5187/deallers");
    this.stocks=this.http.get<stock[]>("http://localhost:5187/stocks"); 
    
  }
  getAuthValue(): Observable<boolean> {
    return this.subIsAuth.asObservable();
  }
  setAuthValue(newValue: boolean): void {
    this.subIsAuth.next(newValue);
  }
  
  updateAuthDeallerName(deallerName:string){
    this.deallerName=deallerName;
  }
  updateAuthStatus(currentStatus:boolean)
  {
    this.isAuth=currentStatus;
    console.log("isAuth from service - "+this.isAuth);

  }
  makeADeal(form: NgForm){
    
      console.log(this.http.post<any>('http://localhost:5187/offers/makeADeal',{
        deallerName: this.deallerName,
        stockName:form.form.controls['StockName'].value,
        wantedPrice:form.form.controls['Price'].value,
        wantedAmount:form.form.controls['Amount'].value,
        type:form.form.controls['OfferType'].value
      }).subscribe())
      
  }
  async onLogin(id:number)
  {
    return await this.http.get<dealler>("http://localhost:5187/dealler/"+id);
  }

  initProperties(){
    this.deallers=this.http.get<dealler[]>("http://localhost:5187/deallers");
    this.stocks=this.http.get<stock[]>("http://localhost:5187/stocks"); 
  }
  getDeallers()
  {
    return this.deallers;
  }
  getDeallerByName(){
    return this.http.get<dealler>("http://localhost:5187/stocks/"+this.deallerName);
  }
  async getStocks(){
    await this.stocks.subscribe();
    return this.stocks;
  }
}
