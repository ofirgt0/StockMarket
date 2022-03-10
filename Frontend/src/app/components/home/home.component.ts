import { Component, OnInit } from '@angular/core';
import { DataContainerService } from 'src/app/services/data-container.service';
import { BackendAccessService } from 'src/app/services/BackendAccess.service';
import { stock } from 'src/entities.model';
import { Chart, registerables } from 'chart.js';
import { ChartManagerService } from 'src/app/services/chart-manager.service';
import { NavigationEnd, Router } from '@angular/router';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  isAuth=false;
  stocks:stock[]=[];
  mySubscription: any;

  constructor(private dataContainerService:DataContainerService,private router: Router,
    private backendAccess:BackendAccessService, private chartService:ChartManagerService) {
      this.router.routeReuseStrategy.shouldReuseRoute = function () {
        return false;
      };
      this.mySubscription = this.router.events.subscribe((event) => {
        if (event instanceof NavigationEnd) {
          this.router.navigated = false;
        }
      });
   }

  async ngOnInit(): Promise<void> {
    (await this.dataContainerService.getStocksAsync()).subscribe( resStocks => { this.stocks = resStocks });
    setInterval(async () => {
      (await this.dataContainerService.getStocksAsync()).subscribe(resStocks=>{this.stocks=resStocks});
    }, 1000);

    this.backendAccess.getAuthValue().subscribe((value) => {this.isAuth = value;});
    Chart.register(...registerables);
  }
  showChart(stockName:string){
    this.chartService.setStockNameValue(stockName);
  }
  
}
