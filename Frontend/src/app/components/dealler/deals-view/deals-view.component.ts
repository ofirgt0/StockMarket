import { Component, OnInit } from '@angular/core';
import { BackendAccessService } from 'src/app/services/BackendAccess.service';
import { DataContainerService } from 'src/app/services/data-container.service';
import { Deal, dealler } from 'src/entities.model';

@Component({
  selector: 'app-deals-view',
  templateUrl: './deals-view.component.html',
  styleUrls: ['./deals-view.component.css']
})
export class DealsViewComponent implements OnInit {

  dealler!: dealler;
  deallerDeals:Deal[]=[];
  noDeals=false;
  constructor(private accessService:BackendAccessService,private dataContainer:DataContainerService) { 
    accessService
  }

  async ngOnInit(): Promise<void> {    
    await this.dataContainer.getLogedDealler().subscribe(resDealler=>{this.dealler=resDealler});
    await this.dataContainer.getLogedInDeallerDeals().subscribe(resDeallerDeals=>{this.deallerDeals=resDeallerDeals;});
    
    setInterval(async () => {this.noDeals=(this.deallerDeals.length==0)}, 5000);
    
  }
}
