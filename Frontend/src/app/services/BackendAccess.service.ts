import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NgForm } from '@angular/forms';
import { BehaviorSubject, map, Observable, timer } from 'rxjs';
import { dealler, MakeADealResponse, stock } from 'src/entities.model';
import { DataContainerService } from './data-container.service';
const timeIntevalSeconds = 10;
const BACKEND_URL = 'http://localhost:5187/';


@Injectable({
  providedIn: 'root',
})

export class BackendAccessService {

  subIsAuth: BehaviorSubject<boolean>;

  isAuth = false;
  
  makeADealResponse!: MakeADealResponse;

  constructor(private http: HttpClient,private dataContainer:DataContainerService) {
    this.subIsAuth = new BehaviorSubject<boolean>(this.isAuth);
    
  }
  
  getAuthValue(): Observable<boolean> {
    return this.subIsAuth.asObservable();
  }
  setAuthValue(newValue: boolean): void {
    this.subIsAuth.next(newValue);
  }

  updateAuthDeallerName(deallerName: string) {
    this.dataContainer.deallerName = deallerName;
    this.dataContainer.logedInDealler = this.http.get<dealler>(
      BACKEND_URL + 'deallers/' + this.dataContainer.deallerName
    );
  }
  updateAuthStatus(currentStatus: boolean) {
    this.isAuth = currentStatus;
  }
  async makeADealAsync(form: NgForm) {
    
    var ret= await this.http.post<MakeADealResponse>(BACKEND_URL+'offers/makeADeal', {
        deallerName: this.dataContainer.deallerName,
        stockName: form.form.controls['StockName'].value,
        wantedPrice: form.form.controls['Price'].value,
        wantedAmount: form.form.controls['Amount'].value,
        type: form.form.controls['OfferType'].value
      });
    
    return ret;
  }
  async onLoginAsync(id: number) {
    var loginResponse=await this.http.get<dealler>(BACKEND_URL+'dealler/' + id);
    loginResponse.subscribe(data=> {
      this.setAuthValue(data!=null)
    })
    return loginResponse;
  }
  
}
