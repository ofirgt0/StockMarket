import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { ServerAccessService } from 'src/app/services/server-access.service';
import { dealler } from 'src/entities.model';


@Component({
  selector: 'app-deallers',
  templateUrl: './deallers.component.html',
  styleUrls: ['./deallers.component.css']
})
export class DeallersComponent implements OnInit {

  deallers:dealler[]=[];

  constructor(private accessService:ServerAccessService) { }
  
  async ngOnInit(): Promise<void> {
    await this.accessService.getDeallers().subscribe(resDealler=>{this.deallers=resDealler});
  }

}
