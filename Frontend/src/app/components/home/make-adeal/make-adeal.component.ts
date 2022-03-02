import { Component, OnInit } from '@angular/core';
import { ServerAccessService } from 'src/app/services/server-access.service';
import { stock,Offer } from 'src/entities.model';
import { NgForm } from '@angular/forms';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-make-adeal',
  templateUrl: './make-adeal.component.html',
  styleUrls: ['./make-adeal.component.css']
})
export class MakeADealComponent implements OnInit {
  stocks:stock[]=[];
  newOffer!: Offer;
  constructor(private accessService:ServerAccessService) { }

  async ngOnInit(): Promise<void> {
    (await this.accessService.getStocks()).subscribe(resStocks=>{this.stocks=resStocks});
  }
  formatLabel(value: number) {
    if (value >= 1000) 
      return Math.round(value / 1000);
    return value;
  }
  formatLabelK(value: number) {
    if (value >= 1000) 
      return Math.round(value / 1000) + 'K';
    return value;
  }
  
  onSubmit(form: NgForm){
    this.accessService.makeADeal(form)
  }

}
