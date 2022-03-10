import { Component, OnInit } from '@angular/core';
import { BackendAccessService } from 'src/app/services/BackendAccess.service';
import { DataContainerService } from 'src/app/services/data-container.service';
import { HoldingsWorth, stockWithAmount } from 'src/entities.model';

@Component({
  selector: 'app-holdings-view',
  templateUrl: './holdings-view.component.html',
  styleUrls: ['./holdings-view.component.css']
})
export class HoldingsViewComponent implements OnInit {

  
  deallerHoldings!:HoldingsWorth;
  stocksWithAmount!:stockWithAmount[];
  constructor(private accessService:BackendAccessService,private dataContainer:DataContainerService) { 
    
  }

  async ngOnInit(): Promise<void> {    
    await this.dataContainer.getDeallerHoldingsWorth().subscribe(resDeallerHoldings=>{
      this.deallerHoldings=resDeallerHoldings; 
    });

    await this.dataContainer.getLogedDealler().subscribe(resDeller=>{
      this.stocksWithAmount= resDeller.ownedStocks;
    }) 
        
  }

}
