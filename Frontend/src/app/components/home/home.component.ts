import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { ServerAccessService } from 'src/app/services/server-access.service';
import { stock } from 'src/entities.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  isAuth=false;
  stocks:stock[]=[];

  
  constructor(private accessService:ServerAccessService) {
    
   }

  async ngOnInit(): Promise<void> {
    (await this.accessService.getStocks()).subscribe(resStocks=>{this.stocks=resStocks});
    setInterval(async () => {
      (await this.accessService.getStocks()).subscribe(resStocks=>{this.stocks=resStocks});
      
    }, 1000);
    this.accessService.getAuthValue().subscribe((value) => {
      this.isAuth = value;
    });
      
  }
  getIsAuth(){
    this.isAuth=this.accessService.isAuth;     
  }
}
