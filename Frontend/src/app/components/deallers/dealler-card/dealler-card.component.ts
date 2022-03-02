import { Component, OnInit } from '@angular/core';
import { ServerAccessService } from 'src/app/services/server-access.service';
import { dealler } from 'src/entities.model';

@Component({
  selector: 'app-dealler-card',
  templateUrl: './dealler-card.component.html',
  styleUrls: ['./dealler-card.component.css']
})
export class DeallerCardComponent implements OnInit {
  deallers:dealler[]=[];

  constructor(private accessService:ServerAccessService) { }

  async ngOnInit(): Promise<void> {
    await this.accessService.getDeallers().subscribe(resDealler=>{this.deallers=resDealler});
  }
  

}
