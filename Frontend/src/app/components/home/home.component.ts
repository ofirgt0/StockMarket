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
  
  stocks:stock[]=[];
  constructor(private accessService:ServerAccessService) { }
  
  async ngOnInit(): Promise<void> {
    setInterval(async () => {
      (await this.accessService.getStocks()).subscribe(resStocks=>{this.stocks=resStocks});}, 1000);
      (await this.accessService.getStocks()).subscribe(resStocks=>{this.stocks=resStocks});

    console.log(this.stocks);
  }
    
}
