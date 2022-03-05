import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { DataContainerService } from 'src/app/services/data-container.service';
import { BackendAccessService } from 'src/app/services/BackendAccess.service';
import { stock } from 'src/entities.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  isAuth=false;
  stocks:stock[]=[];

  
  constructor(private dataContainerService:DataContainerService,
    private backendAccess:BackendAccessService) {
    
   }

  async ngOnInit(): Promise<void> {
    (await this.dataContainerService.getStocks()).subscribe(resStocks=>{this.stocks=resStocks});
    setInterval(async () => {
      (await this.dataContainerService.getStocks()).subscribe(resStocks=>{this.stocks=resStocks});
      
    }, 1000);
    this.backendAccess.getAuthValue().subscribe((value) => {
      this.isAuth = value;
    });
      
  }

}
