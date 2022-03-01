import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { ServerAccessService } from 'src/app/services/server-access.service';
import { stock } from 'src/entities.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  stocksSub = new Subject<stock[]>();
  stocksObs = this.stocksSub.asObservable();
  stocks:stock[]=[];
  constructor(private accessService:ServerAccessService) { }
  
  ngOnInit(): void {
    setInterval(() => {
      this.accessService.getStocks().subscribe(resStocks=>{this.stocks=resStocks});}, 1000);
    this.accessService.getStocks().subscribe(resStocks=>{this.stocks=resStocks});

    console.log(this.stocks);
  }
    
}
