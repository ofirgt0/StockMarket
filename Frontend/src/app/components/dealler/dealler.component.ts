import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrService } from 'ngx-toastr';
import { BackendAccessService } from 'src/app/services/BackendAccess.service';
import { Deal, dealler } from 'src/entities.model';
import { Observable } from 'rxjs';
import { DataContainerService } from 'src/app/services/data-container.service';



@Component({
  selector: 'app-dealler',
  templateUrl: './dealler.component.html',
  styleUrls: ['./dealler.component.css']
})
export class DeallerComponent implements OnInit {
  deallerObs!: Observable<dealler>;
  dealler!: dealler;
  deallerDeals:Deal[]=[];
  noDeals=false;
  constructor(private accessService:BackendAccessService,private dataContainer:DataContainerService) { 
    accessService
  }

  async ngOnInit(): Promise<void> {    
    await this.dataContainer.getLogedDealler().subscribe(resDealler=>{this.dealler=resDealler});
    await this.dataContainer.getLogedInDeallerDeals().subscribe(resDeallerDeals=>{this.deallerDeals=resDeallerDeals;});
    // setInterval(async () => {
    //   (await this.dataContainer.getLogedInDeallerDeals().subscribe(resDeallerDeals=>{this.deallerDeals=resDeallerDeals;}));
    //   this.noDeals=(this.deallerDeals.length==0);
    // }, 10000);
    
  }

}
