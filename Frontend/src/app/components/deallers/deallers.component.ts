import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { BackendAccessService } from 'src/app/services/BackendAccess.service';
import { DataContainerService } from 'src/app/services/data-container.service';
import { dealler } from 'src/entities.model';


@Component({
  selector: 'app-deallers',
  templateUrl: './deallers.component.html',
  styleUrls: ['./deallers.component.css']
})
export class DeallersComponent implements OnInit {

  deallers:dealler[]=[];

  constructor(private accessService:BackendAccessService, private dataContainer:DataContainerService) { }
  
  async ngOnInit(): Promise<void> {
    await this.dataContainer.getDeallers().subscribe(resDealler=>{this.deallers=resDealler});
  }

}
